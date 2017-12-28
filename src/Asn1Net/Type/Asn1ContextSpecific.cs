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

namespace Net.Asn1.Type
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Implementation of ASN.1 Context Specific
    /// </summary>
    public class Asn1ContextSpecific : Asn1Object<List<byte>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1ContextSpecific"/> class. Preferably used when encoding Context Specific.
        /// </summary>
        /// <param name="constructed">Constructed/Primitive bit of identifier octet.</param>
        /// <param name="tagNumber">Tag number of Asn.1 object.</param>
        /// <param name="content">Content to be encoded.</param>
        public Asn1ContextSpecific(bool constructed, int tagNumber, List<byte> content)
            : base(Asn1Class.ContextSpecific, constructed, tagNumber, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1ContextSpecific"/> class. Preferably used when reading Context Specific.
        /// </summary>
        /// <param name="constructed">Constructed/Primitive bit of identifier octet.</param>
        /// <param name="tagNumber">Tag number of Asn.1 object.</param>
        /// <param name="content">BER encoded value in a Stream.</param>
        internal Asn1ContextSpecific(bool constructed, int tagNumber, SubStream content)
            : base(Asn1Class.ContextSpecific, constructed, tagNumber, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            // because getter of content already tried to parse the value
            var contentBytes = this.Content;

            var resBytes = new List<byte>();
            resBytes.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            resBytes.AddRange(DerWriter.WriteLength(contentBytes, (Asn1Type)this.Asn1Tag));
            resBytes.AddRange(contentBytes);

            return resBytes.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Content == null)
            {
                return FormattableString.Invariant($"{base.ToString()} - {this.Content.ToHex()}");
            }
            else
            {
                return base.ToString();
            }
        }

        /// <summary>
        /// Read the value of the given node as list of children in the set.
        /// </summary>
        /// <returns>>Value of the given node as list of children in the set.</returns>
        internal override List<byte> ReadEncodedValue()
        {
            if (this.RawContent != null)
            {
                this.RawContent.Seek(0, SeekOrigin.Begin);
                return new List<byte>(this.RawContent.ReadToEnd());
            }

            return null;
        }
    }
}
