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
        [InlineData("0C 29 61 62 63 64 2B C4 BE C5 A1 C4 8D C5 A5 C5 BE C3 BD C3 A1 C3 AD C3 A9 C3 BA C3 A4 C3 B4 C2 A7 C5 88 C3 B3 C5 95 61 62 63 64",
            "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        [Trait("UTF8 STRING", "UTF8 STRING")]
        [Theory]
        public void ReadUtf8String(string valueToTest, string correctResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1Utf8String = asn1Obj.First() as Asn1Utf8String;
                Assert.NotNull(asn1Utf8String);

                Assert.True(asn1Utf8String.Content == correctResult);
            }
        }
    }
}
