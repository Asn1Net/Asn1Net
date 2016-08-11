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

    /// <summary>
    /// Implementation of ASN.1 SEQUENCE
    /// </summary>
    public class Asn1Sequence : Asn1Object<List<Asn1ObjectBase>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Sequence"/> class.
        /// Preferably used when encoding SEQUENCE.
        /// </summary>
        /// <param name="asn1Class">Class of given Asn.1 object.</param>
        /// <param name="content">Content to be encoded.</param>
        public Asn1Sequence(Asn1Class asn1Class, List<Asn1ObjectBase> content)
            : base(asn1Class, true, (int)Asn1Type.Sequence, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Sequence"/> class.
        /// Preferably used when reading SEQUENCE.
        /// </summary>
        /// <param name="asn1Class">Class of given Asn.1 object.</param>
        /// <param name="content">BER encoded value in a Stream.</param>
        public Asn1Sequence(Asn1Class asn1Class, SubStream content)
            : base(asn1Class, true, (int)Asn1Type.Sequence, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            var contentBytes = new List<byte>();
            foreach (var item in this.Content)
            {
                contentBytes.AddRange(item.Write());
            }

            var resBytes = new List<byte>();
            resBytes.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            resBytes.AddRange(DerWriter.WriteLength(contentBytes, (Asn1Type)this.Asn1Tag));
            resBytes.AddRange(contentBytes);

            return resBytes.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Read the value of the given node as list of children in the sequence.
        /// </summary>
        /// <returns>>Value of the given node as list of children in the sequence.</returns>
        internal override List<Asn1ObjectBase> ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);

            // use inner reader to parse content of SEQUENCE
            var internalReader = new BerReader(content);
            List<Asn1ObjectBase> children;
            internalReader.Read(out children);

            content.SetLength(content.Position);

            return children;
        }
    }
}
