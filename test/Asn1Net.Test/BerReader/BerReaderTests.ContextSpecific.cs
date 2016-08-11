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
        [InlineData("A0 03 02 01 02", 0, true, "02 01 02")]
        [InlineData("82 0A 67 69 74 68 75 62 2E 63 6F 6D", 2, false, "67 69 74 68 75 62 2E 63 6F 6D")]
        [InlineData("82 0E 77 77 77 2E 67 69 74 68 75 62 2E 63 6F 6D", 2, false, "77 77 77 2E 67 69 74 68 75 62 2E 63 6F 6D")]
        [InlineData("BF 64 03 02 01 02", 100, true, "02 01 02")]
        [InlineData("BF 87 68 03 02 01 02", 1000, true, "02 01 02")]
        [InlineData("BF 86 8D 20 03 02 01 02 ", 100000, true, "02 01 02")]
        [Trait("CONTEXT SPECIFIC", "CONTEXT SPECIFIC")]
        [Theory]
        public void ReadContextSpecific(string valueToTest, int tagNumber, bool constructed, string innerContent)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            var contentEncoded = Helpers.GetExampleBytes(innerContent);
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1ContextSpecific = asn1Obj.First() as Asn1ContextSpecific;
                Assert.NotNull(asn1ContextSpecific);

                //var res = Enumerable.SequenceEqual(contentEncoded, asn1ContextSpecific.Content);
                //Assert.True(res);

                Assert.True(constructed == asn1ContextSpecific.Constructed);
                Assert.True(tagNumber == asn1ContextSpecific.Asn1Tag);
            }
        }
    }
}
