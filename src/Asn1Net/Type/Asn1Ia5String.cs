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
    using System.Text;

    /// <summary>
    /// Implementation of ASN.1 IA5 STRING
    /// </summary>
    public class Asn1Ia5String : Asn1Object<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Ia5String"/> class. Preferably used when encoding IA5 STRING.
        /// </summary>
        /// <param name="content">Content to be encoded.</param>
        public Asn1Ia5String(string content)
            : base(Asn1Class.Universal, false, (int)Asn1Type.Ia5String, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Ia5String"/> class. Preferably used when reading IA5 STRING.
        /// </summary>
        /// <param name="content">BER encoded value in a Stream.</param>
        internal Asn1Ia5String(SubStream content)
            : base(Asn1Class.Universal, false, (int)Asn1Type.Ia5String, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            var res = new List<byte>();
            var val = Encoding.ASCII.GetBytes(this.Content);

            res.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            res.AddRange(DerWriter.WriteLength(val, (Asn1Type)this.Asn1Tag));
            res.AddRange(val);

            return res.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var tagAndLength = base.ToString();

            return FormattableString.Invariant($"{tagAndLength} - {this.Content}");
        }

        /// <summary>
        /// Read the value of the given node as Ia5 string.
        /// </summary>
        /// <returns>Value of the given node as Ia5 string.</returns>
        internal override string ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);

            var rawValue = content.ReadToEnd().ToArray();
            var resp = Encoding.UTF8.GetString(rawValue, 0, rawValue.Length);
            return resp;
        }
    }
}
