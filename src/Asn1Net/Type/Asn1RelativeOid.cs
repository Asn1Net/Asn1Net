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
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Implementation of ASN.1 RELATIVE OID
    /// </summary>
    public class Asn1RelativeOid : Asn1Object<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1RelativeOid"/> class.
        /// Preferably used when encoding RELATIVE OID.
        /// The encoding of a relative object identifier value shall be primitive.
        /// </summary>
        /// <param name="content">Content to be encoded.</param>
        public Asn1RelativeOid(string content)
            : base(Asn1Class.Universal, false, (int)Asn1Type.RelativeOid, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1RelativeOid"/> class.
        /// Preferably used when reading RELATIVE OID.
        /// The encoding of a relative object identifier value shall be primitive.
        /// </summary>
        /// <param name="content">BER encoded value in a Stream.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        internal Asn1RelativeOid(SubStream content, bool constructed)
            : base(Asn1Class.Universal, constructed, (int)Asn1Type.RelativeOid, content)
        {
        }

        /// <summary>
        /// Parse Oid value from Byte[]. More information here <![CDATA[http://www.itu.int/rec/dologin_pub.asp?lang=e&id=T-REC-X.690-200811-I!!PDF-E&type=items]]>
        /// </summary>
        /// <param name="oid">Oid value in byte array.</param>
        /// <returns>Oid in string format</returns>
        public static string ParseEncodedOid(byte[] oid)
        {
            if (oid == null)
            {
                throw new ArgumentNullException(nameof(oid));
            }

            var oidValues = new List<int>();

            // Each subidentifier is represented as a series of (one or more) octets. Bit 8 of each octet indicates whether it is the last in
            // the series: bit 8 of the last octet is zero; bit 8 of each preceding octet is one. Bits 7 to 1 of the octets in the series
            // collectively encode the subidentifier. Conceptually, these groups of bits are concatenated to form an unsigned binary
            // number whose most significant bit is bit 7 of the first octet and whose least significant bit is bit 1 of the last octet. The
            // subidentifier shall be encoded in the fewest possible octets, that is, the leading octet of the subidentifier shall not have
            // the value 80 HEX
            int current = 0;
            for (int i = 0; i < oid.Length; i++)
            {
                current = (current << 7) | oid[i] & 0x7F;

                // check if last byte
                if ((oid[i] & 0x80) == 0)
                {
                    oidValues.Add(current);
                    current = 0;
                }
            }

            // join values of oid with comma
            var oidValue = string.Join(".", oidValues);
            return oidValue;
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            if (Constructed)
                throw new FormatException("The encoding of a relative object identifier value shall be primitive.");

            var resBytes = new List<byte>();
            var oidParts = this.Content.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(p => Convert.ToInt32(p, CultureInfo.InvariantCulture)).ToArray();

            var res = new List<byte>(oidParts.Length);
            DerWriterUtils.ParseSubIdentifiers(oidParts, res);

            resBytes.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            resBytes.AddRange(DerWriter.WriteLength(res, (Asn1Type)this.Asn1Tag));
            resBytes.AddRange(res);

            return resBytes.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var tagAndLength = base.ToString();

            return FormattableString.Invariant($"{tagAndLength} - {this.Content}");
        }

        /// <summary>
        /// Read the value of the given node as Relative oid.
        /// </summary>
        /// <returns>Value of the given node as Relative Oid.</returns>
        internal override string ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);

            var rawValue = new byte[content.Length];
            for (int i = 0; i < content.Length; i++)
            {
                rawValue[i] = (byte)content.ReadByte();
            }

            return ParseEncodedOid(rawValue);
        }
    }
}
