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
        [InlineData("1C 64 00 00 00 61 00 00 00 62 00 00 00 63 00 00 00 64 00 00 00 2B 00 00 01 3E 00 00 01 61 00 00 01 0D 00 00 01 65 00 00 01 7E 00 00 00 FD 00 00 00 E1 00 00 00 ED 00 00 00 E9 00 00 00 FA 00 00 00 E4 00 00 00 F4 00 00 00 A7 00 00 01 48 00 00 00 F3 00 00 01 55 00 00 00 61 00 00 00 62 00 00 00 63 00 00 00 64 ",
            "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        [Trait("UNIVERSAL STRING", "UNIVERSAL STRING")]
        [Theory]
        public void ReadUniversalString(string valueToTest, string correctResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1UniversalString = asn1Obj.First() as Asn1UniversalString;
                Assert.NotNull(asn1UniversalString);

                Assert.True(asn1UniversalString.Content == correctResult);
            }
        }
    }
}
