﻿/*
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

        private static StringBuilder str = new StringBuilder();
        private static void Print(List<Asn1ObjectBase> asn1Obj, int deep = 0)
        {
            foreach (var item in asn1Obj)
            {
                str.AppendLine(new string('\t', deep) + item.ToString());

                var seq = item as Asn1Sequence;
                if (seq != null)
                {
                    Print(seq.Content, deep + 1);
                    continue;
                }

                var set = item as Asn1Set;
                if (set != null)
                {
                    Print(set.Content, deep + 1);
                }

                var contextSpecific = item as Asn1ContextSpecific;
                if (contextSpecific != null)
                {
                    using (var ms = new MemoryStream(contextSpecific.Content.ToArray()))
                    {
                        List<Asn1ObjectBase> asn1ObjInner;
                        new BerReader(ms).Read(out asn1ObjInner);

                        Assert.NotNull(asn1ObjInner);

                        Print(asn1ObjInner, deep + 1);
                    }
                }

                //var octetString = item as Asn1OctetString;
                //if (octetString != null)
                //{
                //    using (var ms = new MemoryStream(octetString.Content))
                //    {
                //        List<Asn1ObjectBase> asn1ObjInner;
                //        new BerReader(ms).Read(out asn1ObjInner);

                //        Assert.NotNull(asn1ObjInner);

                //        Print(asn1ObjInner, deep + 1);
                //    }
                //}
            }
        }
    }
}
