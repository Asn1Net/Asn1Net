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
        [InlineData("0D 04 C2 7B 03 02", "8571.3.2")]
        [Trait("RELATIVE OID", "RELATIVE OID")]
        [Theory]
        public void ReadRelativeOid(string valueToTest, string correctResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1RelativeOid = asn1Obj.First() as Asn1RelativeOid;
                Assert.NotNull(asn1RelativeOid);

                Assert.Equal(asn1RelativeOid.Content, correctResult);
            }
        }
    }
}
