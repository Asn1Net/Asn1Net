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
        [InlineData("09 01 40", double.PositiveInfinity)]
        [InlineData("09 01 41", double.NegativeInfinity)]
        [InlineData("09 01 42", double.NaN)]
        // [InlineData("09 01 43", -0.0d)] - can not identify minus zero
        [InlineData("09 00", 0)]
        [InlineData("09 08 03 33 31 34 2E 45 2D 32", 3.14)]
        [InlineData("09 0A 03 31 35 36 32 35 2E 45 2D 35", 0.15625)]
        [InlineData("09 0A 03 31 30 30 30 31 2E 45 2B 30", 10001)]
        [Trait("REAL", "REAL")]
        [Theory]
        public void WriteReal(string correctResultHex, double valueToTest)
        {
            var encoded = Helpers.GetExampleBytes(correctResultHex);

            using (var ms = new MemoryStream())
            {
                var asn1Obj = new Asn1Real(valueToTest);
                new DerWriter(ms).Write(asn1Obj);

                var res = Enumerable.SequenceEqual(encoded, ms.ToArray());
                Assert.True(res);
            }
        }
    }
}
