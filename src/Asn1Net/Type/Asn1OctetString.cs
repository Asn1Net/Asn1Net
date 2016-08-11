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
    /// Implementation of ASN.1 OCTET STRING
    /// </summary>
    public class Asn1OctetString : Asn1Object<byte[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1OctetString"/> class.
        /// Preferably used when encoding OCTET STRING.
        /// </summary>
        /// <param name="content">Content to be encoded.</param>
        public Asn1OctetString(byte[] content)
            : base(Asn1Class.Universal, false, (int)Asn1Type.OctetString, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1OctetString"/> class.
        /// Preferably used when reading OCTET STRING.
        /// </summary>
        /// <param name="content">BER encoded value in a Stream.</param>
        internal Asn1OctetString(SubStream content)
           : base(Asn1Class.Universal, false, (int)Asn1Type.OctetString, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            var res = new List<byte>();
            res.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            res.AddRange(DerWriter.WriteLength(this.Content, (Asn1Type)this.Asn1Tag));
            res.AddRange(this.Content);

            return res.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var tagAndLength = base.ToString();

            return FormattableString.Invariant($"{tagAndLength} - {this.Content.ToHex()}");
        }

        /// <summary>
        /// Read the value of the given node as Octet string.
        /// </summary>
        /// <returns>Value of the given node as Octet string.</returns>
        internal override byte[] ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);

            var rawValue = content.ReadToEnd().ToArray();

            return rawValue;
        }
    }
}
