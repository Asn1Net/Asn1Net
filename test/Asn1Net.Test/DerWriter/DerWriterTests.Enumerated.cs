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

using System.IO;
using System.Linq;
using Net.Asn1.Type;
using Xunit;

namespace Net.Asn1.Writer.Tests
{
    public partial class DerWriterTests
    {
        /// <summary>
        /// Test enum
        /// </summary>
        public enum TestEnum
        {
            OptionOne = 1,
            OptionTwo = 2
        }

        [InlineData("0a 01 01", TestEnum.OptionOne)]
        [InlineData("0a 01 02", TestEnum.OptionTwo)]
        [Trait("ENUMERATED", "ENUMERATED")]
        [Theory]
        public void WriteEnumerated(string correctResultHex, TestEnum valueToTest)
        {
            var encoded = Helpers.GetExampleBytes(correctResultHex);

            using (var ms = new MemoryStream())
            {
                var asn1Enumerated = new Asn1Enumerated(new System.Numerics.BigInteger((int)valueToTest));
                new DerWriter(ms).Write(asn1Enumerated);

                var res = Enumerable.SequenceEqual(encoded, ms.ToArray());
                Assert.True(res);
            }
        }

    }
}
