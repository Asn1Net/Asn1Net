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
using Net.Asn1.Type;
using Xunit;
using FluentAssertions;

namespace Net.Asn1.Writer.Tests
{
    public partial class BerReaderTests
    {
        public static IEnumerable<object[]> SetData
        {
            get
            {
                yield return new object[] { "31 0D 05 00 06 09 2A 86 48 86 F7 0D 01 01 0B", new List<Asn1ObjectBase> {
                    new Asn1Set(Asn1Class.Universal, new List<Asn1ObjectBase> () {
                        new Asn1Null(),
                        new Asn1ObjectIdentifier("1.2.840.113549.1.1.11")
                    })
                } };

                yield return new object[] { "31 00", new List<Asn1ObjectBase>
                {
                    new Asn1Set(Asn1Class.Universal, new List<Asn1ObjectBase> ())
                } };

                yield return new object[] { "31 0B 30 09 06 03 55 04  06 13 02 55 53", new List<Asn1ObjectBase> {
                    new Asn1Set(Asn1Class.Universal, new List<Asn1ObjectBase>() {
                        new Asn1Sequence(Asn1Class.Universal, new List<Asn1ObjectBase> () {
                            new Asn1ObjectIdentifier("2.5.4.6"),
                            new Asn1PrintableString("US")
                        })
                    })
                } };
            }
        }

        [MemberData(nameof(SetData))]
        [Trait("SET", "SET")]
        [Theory]
        public void ReadSet(string valueToTest, List<Asn1ObjectBase> expectedResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                asn1Obj.ShouldBeEquivalentTo(expectedResult, options => options
                    .AllowingInfiniteRecursion()
                    .IncludingProperties()
                    .IncludingAllDeclaredProperties()
                    .IncludingAllRuntimeProperties()
                    );
            }
        }
    }
}
