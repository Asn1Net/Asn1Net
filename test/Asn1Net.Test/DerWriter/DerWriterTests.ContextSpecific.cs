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
        [InlineData("A0 03 02 01 02", 0, true, "02 01 02")]
        [InlineData("82 0A 67 69 74 68 75 62 2E 63 6F 6D", 2, false, "67 69 74 68 75 62 2E 63 6F 6D")]
        [InlineData("82 0E 77 77 77 2E 67 69 74 68 75 62 2E 63 6F 6D", 2, false, "77 77 77 2E 67 69 74 68 75 62 2E 63 6F 6D")]
        [InlineData("BF 64 03 02 01 02", 100, true, "02 01 02")]
        [InlineData("BF 87 68 03 02 01 02", 1000, true, "02 01 02")]
        [InlineData("BF 86 8D 20 03 02 01 02 ", 100000, true, "02 01 02")]
        [Trait("CONTEXT SPECIFIC", "CONTEXT SPECIFIC")]
        [Theory]
        public void WriteContextSpecific(string correctResultHex, int tagNumber, bool constructed, string innerContent)
        {
            var encoded = Helpers.GetExampleBytes(correctResultHex);
            var contentEncoded = Helpers.GetExampleBytes(innerContent);
            using (var ms = new MemoryStream())
            {
                var asn1ContextSpecific = new Asn1ContextSpecific(constructed, tagNumber, contentEncoded.ToList());
                new DerWriter(ms).Write(asn1ContextSpecific);

                var res = Enumerable.SequenceEqual(encoded, ms.ToArray());
                Assert.True(res);
            }
        }
    }
}
