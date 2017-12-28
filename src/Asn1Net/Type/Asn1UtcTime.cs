﻿/*
 *  Copyright 2012-2017 The Asn1Net Project
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

namespace Net.Asn1.Type
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementation of ASN.1 UTC TIME
    /// </summary>
    public class Asn1UtcTime : Asn1Object<DateTimeOffset>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1UtcTime"/> class.
        /// Preferably used when encoding UTC TIME.
        /// </summary>
        /// <param name="content">Content to be encoded.</param>
        public Asn1UtcTime(DateTimeOffset content)
            : base(Asn1Class.Universal, false, (int)Asn1Type.UtcTime, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1UtcTime"/> class.
        /// Preferably used when reading UTC TIME.
        /// </summary>
        /// <param name="content">BER encoded value in a Stream.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        internal Asn1UtcTime(SubStream content, bool constructed)
            : base(Asn1Class.Universal, constructed, (int)Asn1Type.UtcTime, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            if (Constructed)
                throw new FormatException("The encoding of the TIME type shall be primitive.");

            var utcDateTime = this.Content.ToUniversalTime();
            var dateTimeString = utcDateTime.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);

            dateTimeString += "Z";
            var res = Encoding.UTF8.GetBytes(dateTimeString);

            var resBytes = new List<byte>();
            resBytes.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            resBytes.AddRange(DerWriter.WriteLength(res, (Asn1Type)this.Asn1Tag));
            resBytes.AddRange(res);

            return resBytes.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var tagAndLength = base.ToString();

            return FormattableString.Invariant($"{tagAndLength} - {this.Content}");
        }

        /// <summary>
        /// Read the value of the given node as DateTimeOffset.
        /// </summary>
        /// <returns>Value of the given node as DateTimeOffset.</returns>
        internal override DateTimeOffset ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);

            var rawValue = content.ReadToEnd().ToArray();

            var resp = Encoding.UTF8.GetString(rawValue, 0, rawValue.Length);
            return ConvertFromUniversalTime(resp);
        }

        /// <summary>
        /// Converts string value to DateTimeOffset. String value is representing UTCTime according to ITU-T X.680 recommendation
        /// </summary>
        /// <param name="time">UTCTime as string value.</param>
        /// <returns>Value converted to DateTimeOffset.</returns>
        private static DateTimeOffset ConvertFromUniversalTime(string time)
        {
            var utcTimeRegex = new Regex(@"^(?<date>\d{6})(?<hour>([01][0-9])|(2[0-3]))(?<minute>[0-5][0-9])?(?<second>[0-5][0-9])Z$", RegexOptions.ECMAScript);
            var match = utcTimeRegex.Match(time);
            if (match.Success == false)
            {
                throw new FormatException("Time was not in correct format according to UTCTime ITU-T X.690 spec.");
            }

            var newTime = time.Replace("Z", "+0000");

            var res = DateTimeOffset.ParseExact(newTime, "yyMMddHHmmsszzz", CultureInfo.InvariantCulture);
            return res;
        }
    }
}
