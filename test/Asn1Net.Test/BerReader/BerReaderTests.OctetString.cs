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
        [InlineData("04 09 | 2b 06 01 04 01 82 37 15 14", "2b 06 01 04 01 82 37 15 14")]
        [InlineData("04 01 00", "00")]
        [Trait("OCTET STRING", "OCTET STRING")]
        [Theory]
        public void ReadOctetString(string valueToTest, string correctResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            var encodedCorrectValue = Helpers.GetExampleBytes(correctResult);
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1OctetString = asn1Obj.First() as Asn1OctetString;
                Assert.NotNull(asn1OctetString);

                var res = Enumerable.SequenceEqual(encodedCorrectValue, asn1OctetString.Content);
                Assert.True(res);
            }
        }
    }
}
