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
using System.Text;
using Net.Asn1.Type;
using Xunit;

namespace Net.Asn1.Writer.Tests
{
    [Trait("_BerReader Tests", "_BerReader Tests")]
    [Trait("_ALL", "_ALL")]
    public partial class BerReaderTests
    {
        [Trait("HEX", "HEX")]
        [Fact]
        public void A()
        {

            var a = "01 01 FF";
            var encoded = Helpers.GetExampleBytes(a);
            var b = encoded.ToHex();
        }


        [Trait("SEQUENCE", "SEQUENCE")]
        [Fact]
        public void ReadSequence1()
        {
            var encoded = File.ReadAllBytes(@"d:\work\estoolkit vzorky ber\Signature-C-B-LTA-10..req");
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                Assert.NotNull(asn1Obj);

                Print(asn1Obj);

                var res = str.ToString();
            }
        }

        private static StringBuilder str = new StringBuilder();
        private static void Print(List<Asn1ObjectBase> asn1Obj, int deep = 0)
        {
            foreach (var item in asn1Obj)
            {
                str.AppendLine(new string('\t',deep) + item.ToString());

                var seq = item as Asn1Sequence;
                if (seq != null)
                {
                    Print(seq.Content, deep+1);
                    continue;
                }

                var set = item as Asn1Set;
                if (set != null)
                {
                    Print(set.Content, deep+1);
                }
            }
        }
    }
}
