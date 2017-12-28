/*
 *  Copyright 2012-2017 The Asn1Net Project
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
/*
 *  Written for the Asn1Net project by:
 *  Peter Polacko <peter.polacko+asn1net@gmail.com>
 */

namespace Net.Asn1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Implements sub stream on top of base stream.
    /// </summary>
    public class SubStream : Stream
    {
        private readonly long startingPosition;

        private long length;
        private Stream baseStream;
        private long position;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubStream"/> class.
        /// </summary>
        /// <param name="baseStream">Base stream.</param>
        /// <param name="length">Length of sub stream.</param>
        public SubStream(Stream baseStream, long length)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException(nameof(baseStream));
            }

            if (length > baseStream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (baseStream.CanSeek == false)
            {
                throw new NotSupportedException("Only seek enabled streams are supported.");
            }

            this.baseStream = baseStream;
            this.length = length;
            this.startingPosition = baseStream.Position;
            this.position = 0;
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get
            {
                return this.baseStream.CanRead;
            }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get
            {
                return this.baseStream.CanSeek;
            }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                return this.length;
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            if (this.Length == 0)
            {
                return -1;
            }

            var tmpBuff = new byte[1];
            var read = this.Read(tmpBuff, 0, 1);
            if (read <= 0)
            {
                return -1;
            }

            return tmpBuff[0];
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(FormattableString.Invariant($"{offset} is out of range of current buffer."));
            }

            if (count > this.Length)
            {
                count = Convert.ToInt32(this.length - this.position);
            }

            if (this.Position >= this.Length)
            {
                return 0;
            }

            // TODO: BER undefined length support => need to find 2 EOC

            // reset base stream to current position
            this.Seek(this.position, SeekOrigin.Begin);

            var bytesRead = this.baseStream.Read(buffer, 0, count);
            if (bytesRead != 0)
            {
                this.Position += bytesRead;
            }

            return bytesRead;
        }

        /// <summary>
        /// Reads data to the end of current stream.
        /// </summary>
        /// <returns>Read byte[].</returns>
        public IEnumerable<byte> ReadToEnd()
        {
            var result = new List<byte>();
            byte[] buff = new byte[1024];
            int bytesRead = 0;
            do
            {
                bytesRead = this.Read(buff, 0, buff.Length);
                result.AddRange(buff.Take(bytesRead));
            }
            while (bytesRead == buff.Length);

            return result;
        }

        /// <inheritdoc/>
        /// <remarks>Throws if base stream is not seek enabled.</remarks>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this.CanSeek)
            {
                long newPosition = 0;
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        newPosition = offset;
                        break;
                    case SeekOrigin.Current:
                        newPosition = this.position + offset;
                        break;
                    case SeekOrigin.End:
                        newPosition = this.length + offset;
                        break;
                    default:
                        throw new NotSupportedException(FormattableString.Invariant($"Given origin [{origin}] is not supported."));
                }

                this.baseStream.Seek(this.startingPosition + newPosition, SeekOrigin.Begin);
                this.position = newPosition;
                return this.position;
            }

            throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        /// <remarks>Does no support setting of length.</remarks>
        public override void SetLength(long value)
        {
            this.length = value;
        }

        /// <inheritdoc/>
        /// <remarks>Does no support write operations.</remarks>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }
    }
}
