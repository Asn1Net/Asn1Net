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
using Xunit;
using FluentAssertions;

namespace Net.Asn1.Writer.Tests
{
    [Trait("_ALL", "_ALL")]
    [Trait("_SUB STREAM", "_SUB STREAM")]
    public class SubStreamTests
    {
        [Fact(DisplayName = "Reading")]
        public void ReadTest()
        {
            var encoded = Helpers.GetExampleBytes("31 0D 05 00 06 09 2A 86 48 86 F7 0D 01 01 0B");
            using (var ms = new MemoryStream(encoded))
            {
                var sub1 = new SubStream(ms, 3);
                ms.Seek(3, SeekOrigin.Begin);

                var sub2 = new SubStream(ms, 5);
                ms.Seek(5, SeekOrigin.Current);

                var sub3 = new SubStream(ms, 4);
                ms.Seek(4, SeekOrigin.Current);

                var sub4 = new SubStream(ms, 3);
                ms.Seek(3, SeekOrigin.End);

                var val1 = new byte[3];
                sub1.Read(val1, 0, 3);

                //ComparisonResult result = compareLogic.Compare(new byte[] { 0x31, 0x0d, 0x05 }, val1);
                //Assert.True(result.AreEqual, result.DifferencesString);
                val1.ShouldBeEquivalentTo(new byte[] { 0x31, 0x0d, 0x05 }, options => options.AllowingInfiniteRecursion());

                var val2 = new byte[5];
                sub2.Read(val2, 0, 5);

                //result = compareLogic.Compare(new byte[] { 0x00, 0x06, 0x09, 0x2A, 0x86 }, val2);
                //Assert.True(result.AreEqual, result.DifferencesString);
                val2.ShouldBeEquivalentTo(new byte[] { 0x00, 0x06, 0x09, 0x2A, 0x86 }, options => options.AllowingInfiniteRecursion());

                var val3 = new byte[4];
                sub3.Read(val3, 0, 4);

                //result = compareLogic.Compare(new byte[] { 0x48, 0x86, 0xF7, 0x0D }, val3);
                //Assert.True(result.AreEqual, result.DifferencesString);
                val3.ShouldBeEquivalentTo(new byte[] { 0x48, 0x86, 0xF7, 0x0D }, options => options.AllowingInfiniteRecursion());


                var val4 = new byte[3];
                sub4.Read(val4, 0, 3);

                //result = compareLogic.Compare(new byte[] { 0x01, 0x01, 0x0B }, val4);
                //Assert.True(result.AreEqual, result.DifferencesString);
                val4.ShouldBeEquivalentTo(new byte[] { 0x01, 0x01, 0x0B }, options => options.AllowingInfiniteRecursion());


                sub1.Seek(0, SeekOrigin.Begin);
                val1 = new byte[3];
                sub1.Read(val1, 0, 3);
                //result = compareLogic.Compare(new byte[] { 0x31, 0x0d, 0x05 }, val1);
                //Assert.True(result.AreEqual, result.DifferencesString);
                val1.ShouldBeEquivalentTo(new byte[] { 0x31, 0x0d, 0x05 }, options => options.AllowingInfiniteRecursion());


                sub1.Seek(-2, SeekOrigin.End);

                val1 = new byte[2];
                sub1.Read(val1, 0, 2);
                //result = compareLogic.Compare(new byte[] { 0x0d, 0x05 }, val1);
                //Assert.True(result.AreEqual, result.DifferencesString);

                val1.ShouldBeEquivalentTo(new byte[] { 0x0d, 0x05 }, options => options.AllowingInfiniteRecursion());
            }
        }

        [Fact(DisplayName = "Seeking test")]
        public void SeekTest()
        {
            var encoded = Helpers.GetExampleBytes("31 0D 05 00 06 09 2A 86 48 86 F7 0D 01 01 0B");
            using (var ms = new MemoryStream(encoded))
            {
                ms.Seek(3, SeekOrigin.Begin);

                var sub1 = new SubStream(ms, 3);

                // ORIGIN BEGIN
                // zero offset
                sub1.Seek(0, SeekOrigin.Begin);
                Assert.True(sub1.Position == 0);

                // positive offset
                sub1.Seek(1, SeekOrigin.Begin);
                Assert.True(sub1.Position == 1);

                // ORIGIN END
                // negative offset
                sub1.Seek(-1, SeekOrigin.End);
                Assert.True(sub1.Position == sub1.Length - 1);

                // zero offset
                sub1.Seek(0, SeekOrigin.End);
                Assert.True(sub1.Position == sub1.Length);

                // positive offset
                sub1.Seek(1, SeekOrigin.End);
                Assert.True(sub1.Position > sub1.Length);

                // ORIGIN CURRENT
                // negative offset
                sub1.Seek(-2, SeekOrigin.Current);
                Assert.True(sub1.Position < sub1.Length);

                // zero offset
                var lastPosition = sub1.Position;
                sub1.Seek(0, SeekOrigin.Current);
                Assert.True(sub1.Position == lastPosition);

                // positive offset
                sub1.Seek(sub1.Length * 3, SeekOrigin.Current);
                Assert.True(sub1.Position > sub1.Length);
            }
        }

    }
}
