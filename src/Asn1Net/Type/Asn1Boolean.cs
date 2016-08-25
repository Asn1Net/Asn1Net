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
    using System.IO;

    /// <summary>
    /// Implementation of ASN.1 BOOLEAN
    /// </summary>
    public class Asn1Boolean : Asn1Object<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Boolean"/> class. Preferably used when encoding BOOLEAN.
        /// The encoding of a boolean value shall be primitive.
        /// </summary>
        /// <param name="content">Content to be encoded.</param>
        public Asn1Boolean(bool content)
            : base(Asn1Class.Universal, false, (int)Asn1Type.Boolean, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Boolean"/> class from BER encoded value. Preferably used when reading BOOLEAN.
        /// The encoding of a boolean value shall be primitive.
        /// </summary>
        /// <param name="content">BER encoded value in a Stream.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        internal Asn1Boolean(SubStream content, bool constructed)
             : base(Asn1Class.Universal, constructed, (int)Asn1Type.Boolean, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            return this.Content ? new byte[] { 0x01, 0x01, 0xFF } : new byte[] { 0x01, 0x01, 0x00 };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var tagAndLength = base.ToString();

            return FormattableString.Invariant($"{tagAndLength} - {this.Content}");
        }

        /// <summary>
        /// Read the value of the given node as Boolean.
        /// </summary>
        /// <returns>>Value of the given node as Boolean.</returns>
        internal override bool ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);
            if (content.Length != 1)
            {
                throw new FormatException("Boolean ASN.1 type should have single octet value.");
            }

            // BER Encoding rules
            // If the boolean value is FALSE the octet shall be zero.
            return content.ReadByte() != 0x00;
        }
    }
}
