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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Net.Asn1.Type;
using Xunit;

namespace Net.Asn1.Writer.Tests
{
    public partial class BerReaderTests
    {
        [InlineData("17 0D 31 34 30 34 30 38 30 30 30 30 30 30 5A ", "140408000000")]
        [InlineData("17 0D 31 36 30 34 31 32 31 32 30 30 30 30 5A", "160412120000")]
        [Trait("UTC TIME", "UTC TIME")]
        [Theory]
        public void ReadUtcTime(string valueToTest, string correctResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            var datetime = DateTimeOffset.ParseExact(correctResult, "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1UtcTime = asn1Obj.First() as Asn1UtcTime;
                Assert.NotNull(asn1UtcTime);

                Assert.True(asn1UtcTime.Content == datetime);
            }
        }

        [InlineData("17 0D 31 34 30 34 30 38 30 30 30 30 30 30 5A ", "140408000000Z")]
        [InlineData("17 0D 31 36 30 34 31 32 31 32 30 30 30 30 5A", "160412120000Z")]
        [Trait("UTC TIME", "UTC TIME")]
        [Theory]
        public void ReadUtcTimeFromString(string valueToTest, string correctResult)
        {
            var encoded = Helpers.GetExampleBytes(valueToTest);
            var datetime = Helpers.ConvertFromUniversalTime(correctResult);

            using (var ms = new MemoryStream(encoded))
            {
                List<Asn1ObjectBase> asn1Obj;
                new BerReader(ms).Read(out asn1Obj);

                var asn1UtcTime = asn1Obj.First() as Asn1UtcTime;
                Assert.NotNull(asn1UtcTime);

                Assert.True(asn1UtcTime.Content == datetime);
            }
        }
    }
}
