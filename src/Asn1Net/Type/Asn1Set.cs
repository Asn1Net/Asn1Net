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
    using System.Linq;

    /// <summary>
    /// Implementation of ASN.1 SET
    /// </summary>
    public class Asn1Set : Asn1Object<List<Asn1ObjectBase>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Set"/> class.
        /// Preferably used when encoding SET.
        /// The encoding of a set value shall be constructed.
        /// </summary>
        /// <param name="asn1Class">Class of given Asn.1 object.</param>
        /// <param name="content">Content to be encoded.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        public Asn1Set(Asn1Class asn1Class, List<Asn1ObjectBase> content, bool constructed = true)
            : base(asn1Class, constructed, (int)Asn1Type.Set, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Set"/> class.
        /// Preferably used when reading SET.
        /// The encoding of a set value shall be constructed.
        /// </summary>
        /// <param name="asn1Class">Class of given Asn.1 object.</param>
        /// <param name="content">BER encoded value in a Stream.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        internal Asn1Set(Asn1Class asn1Class, SubStream content, bool constructed)
            : base(asn1Class, constructed, (int)Asn1Type.Set, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            if (Constructed == false)
                throw new FormatException("The encoding of a sequence value shall be constructed.");

            var contentBytes = new List<byte>();
            var comparer = new SetComparer();

            var orderedContent = this.Content.OrderBy(p => p, comparer);

            foreach (var item in orderedContent)
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
        /// Read the value of the given node as list of children in the set.
        /// </summary>
        /// <returns>Value of the given node as list of children in the set.</returns>
        internal override List<Asn1ObjectBase> ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);
            var internalReader = new BerReader(content);
            List<Asn1ObjectBase> children;
            internalReader.Read(out children);

            return children;
        }

        private class SetComparer : IComparer<Asn1ObjectBase>
        {
            public int Compare(Asn1ObjectBase x, Asn1ObjectBase y)
            {
                if (x == null)
                {
                    throw new ArgumentNullException(nameof(x));
                }

                if (y == null)
                {
                    throw new ArgumentNullException(nameof(y));
                }

                // The encodings of the component values of a set value shall appear in an order determined by their tags as specified
                // in 8.6 of Rec. ITU - T X.680 | ISO / IEC 8824 - 1.

                // those elements or alternatives with universal class tags shall appear first, followed by those with
                // application class tags, followed by those with context-specific tags, followed by those with private class
                // tags;
                var xVal = x.Asn1Class == Asn1Class.Universal ? x.Asn1Tag : x.Asn1Tag * (int)x.Asn1Class;
                var yVal = y.Asn1Class == Asn1Class.Universal ? y.Asn1Tag : y.Asn1Tag * (int)y.Asn1Class;

                return xVal.CompareTo(yVal);
            }
        }
    }
}
