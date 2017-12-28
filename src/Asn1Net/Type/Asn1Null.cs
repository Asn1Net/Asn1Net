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
    /// <summary>
    /// Implementation of ASN.1 NULL
    /// </summary>
    public class Asn1Null : Asn1ObjectBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Null"/> class.
        /// The encoding of a null value shall be primitive. 
        /// </summary>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        public Asn1Null(bool constructed = false)
            : base(Asn1Class.Universal, constructed, (int)Asn1Type.Null)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            if (Constructed)
                throw new System.FormatException("The encoding of a null value shall be primitive.");

            return new byte[] { 0x05, 0x00 };
        }
    }
}
