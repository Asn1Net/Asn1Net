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
        public void ReadEnumerated(string valueToTest, TestEnum correctResult)
        {
            // TODO: upravit Reader tak, aby vedel zapisat do podanej instancie a tym padom aj skontrolovat, ze to ma dobru strukturu

            var encoded = Helpers.GetExampleBytes(valueToTest);
            
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1Enumerated = asn1Obj.First() as Asn1Enumerated;
                Assert.NotNull(asn1Enumerated);
                
                Assert.True(asn1Enumerated.Content == new System.Numerics.BigInteger((int)correctResult));
            }
        }

    }
}
