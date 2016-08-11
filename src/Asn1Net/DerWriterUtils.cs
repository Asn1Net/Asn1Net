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

namespace Net.Asn1
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Implementation of helper methods
    /// </summary>
    public static class DerWriterUtils
    {
        /// <summary>
        /// Hex string lookup table.
        /// </summary>
        private static readonly string[] HexStringTable = new string[]
        {
    "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
    "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
    "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
    "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
    "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
    "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
    "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
    "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
    "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
    "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
    "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
    "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
    "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
    "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
    "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
    "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
        };

        /// <summary>
        /// Returns a hex string representation of an array of bytes.
        /// </summary>
        /// <param name="value">The array of bytes.</param>
        /// <typeparam name="T">Has to be an Enumerable of byte</typeparam>
        /// <returns>A hex string representation of the array of bytes.</returns>
        public static string ToHex<T>(this T value)
            where T : IEnumerable<byte>
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (value != null)
            {
                foreach (byte b in value)
                {
                    stringBuilder.Append(HexStringTable[b]);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Parses subidentifiers of object identifier or relative identifier.
        /// </summary>
        /// <param name="oidParts">Split parts of object or relative identifier.</param>
        /// <param name="res">Result where to write encoded values.</param>
        internal static void ParseSubIdentifiers(int[] oidParts, List<byte> res)
        {
            if (oidParts == null)
            {
                throw new ArgumentNullException(nameof(oidParts));
            }

            if (res == null)
            {
                throw new ArgumentNullException(nameof(res));
            }

            for (var i = 0; i < oidParts.Length; i++)
            {
                SevenBitEncode(oidParts[i], res);
            }
        }

        /// <summary>
        /// Encodes value according to section 8.1.2.4.2 of ITU-T X.690.
        /// </summary>
        /// <param name="valueToEncode">Value to encode.</param>
        /// <param name="res">Result where to write encoded values.</param>
        internal static void SevenBitEncode(int valueToEncode, List<byte> res)
        {
            if (res == null)
            {
                throw new ArgumentNullException(nameof(res));
            }

            var itemToWorkWith = valueToEncode;
            var localRes = new List<byte>();
            for (int i = 0; i < sizeof(int); i++)
            {
                if (itemToWorkWith == 0)
                {
                    break;
                }

                localRes.Add((byte)(itemToWorkWith | 0x80));
                itemToWorkWith = itemToWorkWith >> 7;
            }

            localRes[0] = (byte)(localRes[0] &= 0x7f);
            localRes.Reverse();
            res.AddRange(localRes);
        }

        /// <summary>
        /// Encodes length according to ITU-T X.690.
        /// </summary>
        /// <param name="valueToEncode">Length value to encode.</param>
        /// <returns>Encoded length of ASN.1 node.</returns>
        internal static byte[] EncodeLength(int valueToEncode)
        {
            if (valueToEncode <= 127)
            {
                return new byte[1] { (byte)valueToEncode };
            }

            var itemToWorkWith = valueToEncode;

            var res = new List<byte>(sizeof(int) + 1);

            for (int i = 0; i < res.Capacity; i++)
            {
                if (itemToWorkWith == 0)
                {
                    break;
                }

                res.Add((byte)itemToWorkWith);
                itemToWorkWith = itemToWorkWith >> 8;
            }

            res.Add((byte)(res.Count | 0x80));
            res.Reverse();
            return res.ToArray();
        }
    }
}
