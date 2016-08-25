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

namespace Net.Asn1.Type
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Implementation of ASN.1 REAL
    /// </summary>
    public class Asn1Real : Asn1Object<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Real"/> class.
        /// Preferably used when encoding REAL. The encoding of a real value shall be primitive.
        /// </summary>
        /// <param name="content">Content to be encoded.</param>
        public Asn1Real(double content)
            : base(Asn1Class.Universal, false, (int)Asn1Type.Real, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1Real"/> class.
        /// Preferably used when reading REAL. The encoding of a real value shall be primitive.
        /// </summary>
        /// <param name="content">BER encoded value in a Stream.</param>
        /// <param name="constructed">Flag if type is constructed or primitive.</param>
        internal Asn1Real(SubStream content, bool constructed)
            : base(Asn1Class.Universal, constructed, (int)Asn1Type.Real, content)
        {
        }

        /// <inheritdoc/>
        public override byte[] Write()
        {
            var res = new List<byte>();
            byte[] valBytes = null;

            // SpecialValues
            if (this.Content == 0)
            {
                // If the real value is the value plus zero, there shall be no contents octets in the encoding.
                return new byte[] { 0x09, 0x00 };
            }
            else if (double.IsPositiveInfinity(this.Content))
            {
                valBytes = new byte[] { 0x40 };
            }
            else if (double.IsNegativeInfinity(this.Content))
            {
                valBytes = new byte[] { 0x41 };
            }
            else if (double.IsNaN(this.Content))
            {
                valBytes = new byte[] { 0x42 };
            }
            else if (this.Content == -0.0d)
            {
                valBytes = new byte[] { 0x43 };
            }
            else
            {
                // ToString to find out how many decimal digits value has
                // to achieve a format that looks like this ####.E-0
                // TrimStart will take care of doubles line 0.12314
                // Then remove full stop and we will have number of # to add to format string
                var tmpVal = this.Content.ToString(CultureInfo.InvariantCulture.NumberFormat).TrimStart('0').Replace(".", string.Empty);
                var format = FormattableString.Invariant($"{new string('#', tmpVal.Length)}\\..E+0");

                // Everything was taken care of with format string to meet ITU X690 specification for REAL type.
                var val = this.Content.ToString(format, CultureInfo.InvariantCulture.NumberFormat);

                var valEncoded = Encoding.ASCII.GetBytes(val);
                valBytes = new byte[valEncoded.Length + 1];
                valBytes[0] = 0x03; // base 10 encoding
                valEncoded.CopyTo(valBytes, 1);
            }

            res.AddRange(DerWriter.WriteTag(this.Asn1Class, this.Asn1Tag, this.Constructed));
            res.AddRange(DerWriter.WriteLength(valBytes, (Asn1Type)this.Asn1Tag));
            res.AddRange(valBytes);

            return res.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var tagAndLength = base.ToString();

            return FormattableString.Invariant($"{tagAndLength} - {this.Content}");
        }

        /// <summary>
        /// Read the value of the given node as Real.
        /// </summary>
        /// <returns>Value of the given node as Real.</returns>
        internal override double ReadEncodedValue()
        {
            if (this.RawContent == null)
            {
                throw new ArgumentException("Stream that contains encoded value is null.");
            }

            var content = this.RawContent;
            content.Seek(0, SeekOrigin.Begin);

            if (content.Length > 0)
            {
                var rawValue = content.ReadToEnd().ToArray();

                var firstOctet = rawValue[0];

                // check bit 8 and bit 7
                var encodingType = firstOctet & 192;

                switch (encodingType)
                {
                    // binary encoding
                    case 128:
                        return ExtractRealFromBinaryEncoding(firstOctet, rawValue);

                    // decimal encoding
                    case 0:
                        return ExtractRealFromDecimalEncoding(rawValue);

                    // SpecialRealValue
                    case 64:
                        return ExtractRealFromSpecialValueEncoding(firstOctet, rawValue);

                    default:
                        throw new NotSupportedException("Unknown encoding type.");
                }
            }

            return 0;
        }

        /// <summary>
        /// Extract Real value from binary encoded data.
        /// </summary>
        /// <param name="firstOctet">Information octet with base, mantissa and exponent information.</param>
        /// <param name="rawValue">Raw encoded value.</param>
        /// <returns>Extracted value.</returns>
        private static double ExtractRealFromBinaryEncoding(byte firstOctet, byte[] rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException("rawValue");
            }

            // M = S × N × 2^F
            // 0 ≤ F < 4
            // S = +1 or –1

            // bit 7
            var bit7 = firstOctet & 64;
            var sign = bit7 == 64 ? -1 : +1;

            // bits 6 to 5
            var valueOfBase = 2;
            switch ((firstOctet & 48) >> 4)
            {
                case 0:
                    valueOfBase = 2;
                    break;
                case 1:
                    valueOfBase = 8;
                    break;
                case 2:
                    valueOfBase = 16;
                    break;
                default:
                    throw new NotSupportedException("Encoded value of base reserved for future use.");
            }

            // bits 4 to 3
            var ff = (firstOctet & 12) >> 2;

            var exponentLength = (firstOctet & 3) + 1;
            var exponentValueStartIdx = 1;
            if (exponentLength == 3)
            {
                exponentValueStartIdx = 2;
                exponentLength = (int)rawValue[1];
            }

            // read exponent
            var subtrahendIntegerBytes = new byte[exponentLength];
            subtrahendIntegerBytes[exponentLength - 1] = 128;

            if (exponentLength > sizeof(long))
            {
                throw new PlatformNotSupportedException(
                    "Length of exponent is greater than current used integer type on the platform.");
            }

            long baseInteger = 0;
            var subtrahendInteger = 0;

            for (var i = 0; i < exponentLength; i++, exponentValueStartIdx++)
            {
                baseInteger = (baseInteger << 8) | rawValue[exponentValueStartIdx];
                subtrahendInteger = (subtrahendInteger << 8) | subtrahendIntegerBytes[i];
            }

            var exponent = (baseInteger & ~subtrahendInteger) - subtrahendInteger;

            // read N to compute mantissa
            var n = 0;
            for (var i = 1 + exponentLength; i < rawValue.Length; i++)
            {
                n = (n << 8) | rawValue[i];
            }

            var mantissa = sign * n * Math.Pow(2, ff);
            var res = mantissa * Math.Pow(valueOfBase, exponent);
            return res;
        }

        /// <summary>
        /// Extract Real value from decimal encoded data.
        /// </summary>
        /// <param name="rawValue">Raw encoded value.</param>
        /// <returns>Extracted value.</returns>
        private static double ExtractRealFromDecimalEncoding(byte[] rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException("rawValue");
            }

            var raw = new byte[rawValue.Length - 1];
            Array.Copy(rawValue, 1, raw, 0, rawValue.Length - 1);

            var base10Value = double.Parse(Encoding.UTF8.GetString(raw, 0, raw.Length), CultureInfo.InvariantCulture.NumberFormat);
            return base10Value;
        }

        /// <summary>
        /// Extract Real value from special encoded data.
        /// </summary>
        /// <param name="firstOctet">Information octet with base, mantissa and exponent information.</param>
        /// <param name="rawValue">Raw encoded value.</param>
        /// <returns>Extracted value.</returns>
        private static double ExtractRealFromSpecialValueEncoding(byte firstOctet, byte[] rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException("rawValue");
            }

            if (rawValue.Length > 1)
            {
                throw new FormatException("All SpecialRealValues MUST be encoded by just one information octet, without octets for mantissa end exponent.");
            }

            switch (firstOctet & 0x43)
            {
                case 0x40:
                    return double.PositiveInfinity;
                case 0x41:
                    return double.NegativeInfinity;
                case 0x42:
                    return double.NaN;
                case 0x43:
                    return -1 * 0.0d; // minus zero
                default:
                    throw new NotSupportedException("Special value reserved for future use.");
            }
        }
    }
}
