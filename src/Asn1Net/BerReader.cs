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
    using System.IO;
    using Type;

    /// <summary>
    /// BER reader implementation.
    /// </summary>
    public class BerReader
    {
        private Stream inputStream = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BerReader"/> class.
        /// </summary>
        /// <param name="inputStream">ASN.1 objects in a stream to parse.</param>
        public BerReader(Stream inputStream)
        {
            this.inputStream = inputStream;
        }

        /// <summary>
        /// Reads one node as ASN.1 object.
        /// </summary>
        /// <typeparam name="T">Type of objects to parse. Basically it has to be Asn1BaseObject</typeparam>
        /// <param name="foundObject">Parsed ASN.1 Object.</param>
        /// <returns>Itself.</returns>
        public BerReader ReadOne<T>(out T foundObject)
            where T : Asn1ObjectBase
        {
            var nodeStartOffset = this.inputStream.Position;

            // get first byte - Identifier octet
            var firstByte = this.inputStream.ReadByte();

            // break the cycle when stream ends
            if (firstByte == -1)
            {
                // TODO: return or throw?
                // end of stream
                foundObject = null;
                return this;
            }

            // parse type of ASN.1 node
            Asn1Class asn1Class = ReadAsn1Class(firstByte);
            var constructed = (firstByte & 32) == 32;

            // if first byte is not enough to get ASN1 tag, we will read more from input stream
            int tag = ReadAsn1Tag(firstByte, this.inputStream);

            // read length - Length octet(s) as many as needed
            long length = this.ReadLength();
            long substreamLength;

            // TODO: not sure if it's correct. Need to stop parsing because length of node greater than stream length
            if (length > this.inputStream.Length)
            {
                substreamLength = 0;
                foundObject = null;
                return this;
            }

            substreamLength = length;
            if (length == -1)
            {
                substreamLength = this.inputStream.Length;
            }

            // read content - Value octets
            var contentStream = new SubStream(this.inputStream, substreamLength);

            // make an instance of exact ASN.1 object
            foundObject = MakeInstanceOfAsn1Object<T>(nodeStartOffset, asn1Class, constructed, tag, contentStream);

            return this;
        }

        /// <summary>
        /// Read whole input stream and parse data as list of ASN.1 objects
        /// </summary>
        /// <typeparam name="T">Type of objects to parse. Basically it has to be Asn1BaseObject</typeparam>
        /// <param name="foundObjects">Parsed ASN.1Objects.</param>
        /// <returns>Itself.</returns>
        public BerReader Read<T>(out List<T> foundObjects)
            where T : Asn1ObjectBase
        {
            foundObjects = new List<T>();

            while (true)
            {
                T foundObject;

                this.ReadOne(out foundObject);
                if (foundObject == null)
                {
                    // end of stream
                    break;
                }

                // add object to result
                foundObjects.Add(foundObject);
            }

            return this;
        }

        private static T MakeInstanceOfAsn1Object<T>(long nodeStartOffset, Asn1Class asn1Class, bool constructed, int tag, SubStream content)
            where T : Asn1ObjectBase
        {
            T foundObject;
            Asn1ObjectBase shouldBeType = null;
            if (asn1Class == Asn1Class.ContextSpecific)
            {
                shouldBeType = new Asn1ContextSpecific(constructed, tag, content);
            }
            else
            {
                switch ((Asn1Type)tag)
                {
                    case Asn1Type.Eoc:
                        shouldBeType = new Asn1Eoc();
                        break;
                    case Asn1Type.Boolean:
                        shouldBeType = new Asn1Boolean(content);
                        break;
                    case Asn1Type.Integer:
                        shouldBeType = new Asn1Integer(content);
                        break;
                    case Asn1Type.BitString:
                        shouldBeType = new Asn1BitString(content);
                        break;
                    case Asn1Type.OctetString:
                        shouldBeType = new Asn1OctetString(content);
                        break;
                    case Asn1Type.Null:
                        shouldBeType = new Asn1Null();
                        break;
                    case Asn1Type.ObjectIdentifier:
                        shouldBeType = new Asn1ObjectIdentifier(content);
                        break;
                    case Asn1Type.Real:
                        shouldBeType = new Asn1Real(content);
                        break;
                    case Asn1Type.Enumerated:
                        shouldBeType = new Asn1Enumerated(content);
                        break;
                    case Asn1Type.Utf8String:
                        shouldBeType = new Asn1Utf8String(content);
                        break;
                    case Asn1Type.RelativeOid:
                        shouldBeType = new Asn1RelativeOid(content);
                        break;
                    case Asn1Type.Sequence:
                        shouldBeType = new Asn1Sequence(asn1Class, content);
                        break;
                    case Asn1Type.Set:
                        shouldBeType = new Asn1Set(asn1Class, content);
                        break;
                    case Asn1Type.NumericString:
                        shouldBeType = new Asn1NumericString(content);
                        break;
                    case Asn1Type.PrintableString:
                        shouldBeType = new Asn1PrintableString(content);
                        break;
                    case Asn1Type.T61String:
                        shouldBeType = new Asn1T61String(content);
                        break;
                    case Asn1Type.Ia5String:
                        shouldBeType = new Asn1Ia5String(content);
                        break;
                    case Asn1Type.UtcTime:
                        shouldBeType = new Asn1UtcTime(content);
                        break;
                    case Asn1Type.GeneralizedTime:
                        shouldBeType = new Asn1GeneralizedTime(content);
                        break;
                    case Asn1Type.GraphicString:
                        shouldBeType = new Asn1GraphicString(content);
                        break;
                    case Asn1Type.GeneralString:
                        shouldBeType = new Asn1GeneralString(content);
                        break;
                    case Asn1Type.UniversalString:
                        shouldBeType = new Asn1UniversalString(content);
                        break;
                    case Asn1Type.BmpString:
                        shouldBeType = new Asn1BmpString(content);
                        break;
                    case Asn1Type.ObjectDescriptor:
                    case Asn1Type.External:
                    case Asn1Type.EmbeddedPdv:
                    case Asn1Type.VideotexString:
                    case Asn1Type.VisibleString:
                    case Asn1Type.CharacterString:
                    case Asn1Type.LongForm:
                    default:
                        throw new NotSupportedException();
                }
            }

            foundObject = shouldBeType as T;

            // sanity check
            if (foundObject == null)
            {
                throw new FormatException(FormattableString.Invariant($"ASN.1 node at offset {nodeStartOffset} is of type {shouldBeType.GetType()} but expected type was {typeof(T)}"));
            }

            return foundObject;
        }

        private static int ReadAsn1Tag(int firstByte, Stream inputStream)
        {
            var wannaBeTag = firstByte & (~(128 + 64 + 32));

            // long form
            if (wannaBeTag == 31)
            {
                int tag = 0;
                int nextByte;
                do
                {
                    nextByte = inputStream.ReadByte();
                    var partialValue = nextByte & 0x7f; // clean bit 8
                    tag = tag << 7; // only 7 bits are used for encoding of tag number
                    tag += partialValue;
                }
                while ((nextByte & 0x80) == 0x80);

                return tag;
            }
            else
            {
                return wannaBeTag;
            }
        }

        private static Asn1Class ReadAsn1Class(int firstByte)
        {
            return (Asn1Class)(firstByte & (128 + 64)); // zero anything but 7th and 6th bit
        }

        private long ReadLength()
        {
            long length = 0;
            int lengthFirstByte = this.inputStream.ReadByte();

            // checks
            // 0xff => the value 11111111 (binary) shall not be used
            if (lengthFirstByte == 0xff)
            {
                throw new FormatException("Invalid length format. The value 11111111 (binary) shall not be used.");
            }

            // indefinite form of length
            if (lengthFirstByte == 0x80)
            {
                return -1; // will be corrected later
            }

            // check if length is encoded in short form or long form
            bool isMultiByteLength = (lengthFirstByte & 0x80) != 0;

            // short form length
            if (isMultiByteLength == false)
            {
                length = lengthFirstByte;
            }
            else
            {
                // All bits of the subsequent octets form the encoding of an unsigned binary integer equal to the number of octets in the contents octets.
                // get number of octets
                int numBytesLength = lengthFirstByte & 0x7f;

                // in c# sizeof(int) is 4 bytes. We can not interpret more. We could use long, but reading data from stream expects int, not long
                if (numBytesLength > 4)
                {
                    throw new FormatException("Invalid length value (too long)");
                }

                // read the value of length in long form
                while (numBytesLength > 0)
                {
                    length = (length << 8) | (uint)this.inputStream.ReadByte();
                    numBytesLength--;
                }
            }

            return length;
        }
    }
}
