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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Net.Asn1.Type;
using Xunit;

namespace Net.Asn1.Writer.Tests
{
    public partial class BerReaderTests
    {
        [InlineData("09 01 40", double.PositiveInfinity)]
        [InlineData("09 01 41", double.NegativeInfinity)]
        [InlineData("09 01 42", double.NaN)]
        // [InlineData("09 01 43", -0.0d)] - can not identify minus zero
        // base 10
        [InlineData("09 00", 0)]
        [InlineData("09 08 03 33 31 34 2E 45 2D 32", 3.14)]
        [InlineData("09 0A 03 31 35 36 32 35 2E 45 2D 35", 0.15625)]
        [InlineData("09 0A 03 31 30 30 30 31 2E 45 2B 30", 10001)]
        // base 2
        [InlineData("09 05 02 33 2E 31 34", 3.14)]
        [InlineData("09 03 80 FB 05", 0.15625)]
        [InlineData("09 03 90 FE 0A", 0.15625)]
        [InlineData("09 03 AC FE 05", 0.15625)]
        [Trait("REAL", "REAL")]
        [Theory]
        public void ReadReal(string valueToTest, double correctResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);

            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1Real = asn1Obj.First() as Asn1Real;
                Assert.NotNull(asn1Real);

                Assert.Equal(asn1Real.Content, correctResult);
            }
        }
    }
}
