/*
 *  Copyright 2012-2016 The Asn1Net Project
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

namespace Net.Asn1.Type
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Implementation of ASN.1 BIT STRING
    /// </summary>
    public class Asn1BitString : Asn1Object<byte[]>
    {
        /// <summary>
        /// Unused bits at the last byte of bit string should be cleared. This dictionary will help to do so.
        /// </summary>
        private static readonly Dictionary<int, byte> CleaningMatrix = new Dictionary<int, byte>
        {
            [0] = 0xFF, // 1111 1111
            [1] = 0xFE, // 1111 1110
            [2] = 0xFC, // 1111 1100
            [3] = 0xF8, // 1111 1000
            [4] = 0xF0, // 1111 0000
            [5] = 0xE0, // 1110 0000
            [6] = 0xC0, // 1100 0000
            [7] = 0x80, // 1000 0000
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1BitString"/> class. Preferably used when encoding bit string.
        /// The encoding of a bit string value shall be either primitive or constructed at the option of the sender.
        /// NOTE – Where it is necessary to transfer part of a bit string before the entire bit string is available, the constructed encoding is used.
        /// </summary>
        /// <param name="content">Content to be encoded.</param>
        /// <param name="unusedBits">Unused bits. Last X bits of Content will be cleared.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        public Asn1BitString(byte[] content, int unusedBits, bool constructed = false)
            : base(Asn1Class.Universal, constructed, (int)Asn1Type.BitString, content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (unusedBits < 0 || unusedBits > 7)
            {
                throw new ArgumentException(FormattableString.Invariant($"{unusedBits} shall be in the range zero to seven."));
            }

            if (unusedBits == 0 && content.Length > 1 && (content.Last() & 1) != 1)
            {
                throw new ArgumentException(FormattableString.Invariant($"{content} should not have any trailing zeros"));
            }

            this.UnusedBits = unusedBits;

            if (content.Length > 1 && unusedBits > 0)
            {
                // clean last byte
                this.Content[content.Length - 1] &= CleaningMatrix[unusedBits];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1BitString"/> class. Preferably used when reading bit string.
        /// The encoding of a bit string value shall be either primitive or constructed at the option of the sender. 
        /// NOTE – Where it is necessary to transfer part of a bit string before the entire bit string is available, the constructed encoding is used.
        /// </summary>
        /// <param name="content">Content of given ASN.1 node as stream.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        internal Asn1BitString(SubStream content, bool constructed)
             : base(Asn1Class.Universal, constructed, (int)Asn1Type.BitString, content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
        }

        /// <summary>
        /// Gets unused bits from 0 to 7. Last X bits Of Content will be cleared.
        /// </summary>
        public int UnusedBits { get; private set; }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            var resBytes = new List<byte>();
            resBytes.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            resBytes.AddRange(DerWriter.WriteLength(this.Content, (Asn1Type)this.Asn1Tag));

            // The contents octets for the primitive encoding shall contain an initial octet followed by zero, one or more subsequent octets.
            if (Constructed == false)
            {
                resBytes.Add((byte)this.UnusedBits);
            }

            resBytes.AddRange(this.Content);

            return resBytes.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var tagAndLength = base.ToString();

            return FormattableString.Invariant($"{tagAndLength} - UnusedBits:{this.UnusedBits} - {this.Content}");
        }

        /// <summary>
        /// Read the value of the given node as Byte[].
        /// </summary>
        /// <returns>Value of the given node as Byte[].</returns>
        internal override byte[] ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);
            var unusedBitsEncoded = content.ReadByte();
            if (unusedBitsEncoded < 0 || unusedBitsEncoded > 7)
            {
                throw new ArgumentException(FormattableString.Invariant($"{unusedBitsEncoded} shall be in the range zero to seven."));
            }

            this.UnusedBits = unusedBitsEncoded;

            byte[] result = new byte[0];

            // because first byte was the encoded unused bits
            if (this.RawContent.Length > 1)
            {
                var remainingData = content.ReadToEnd();

                // check if last byte is correctly cleaned according to unused bits value.
                var cleanedLastByte = (byte)(remainingData.Last() & CleaningMatrix[this.UnusedBits]);
                if (cleanedLastByte != remainingData.Last())
                {
                    throw new FormatException("There was more bits set in the last byte than it should be.");
                }

                result = remainingData.ToArray();
            }

            return result.ToArray();
        }
    }
}
