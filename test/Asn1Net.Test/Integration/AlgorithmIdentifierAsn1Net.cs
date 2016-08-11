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
using System.IO;
using System.Linq;
using Net.Asn1.Type;

namespace Net.Asn1.Writer.Tests.Integration
{
    class AlgorithmIdentifierAsn1Net
    {
        public Asn1ObjectIdentifier Algorithm
        {
            get;
            private set;
        }

        public Asn1ObjectBase Parameters
        {
            get;
            private set;
        }

        public AlgorithmIdentifierAsn1Net(Asn1ObjectIdentifier algorithm, Asn1ObjectBase parameters)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            Algorithm = algorithm;
            Parameters = parameters;
        }

        public AlgorithmIdentifierAsn1Net(Asn1Sequence seq)
        {
            if (seq == null)
                throw new ArgumentNullException("seq");

            if (seq.Content.Count < 1 || seq.Content.Count > 2)
                throw new ArgumentException("Invalid number of sequence members");

            Algorithm = seq.Content.First() as Asn1ObjectIdentifier;
            Parameters = (seq.Content.Count == 2) ? seq.Content[1] : null;
        }

        public AlgorithmIdentifierAsn1Net(byte[] content)
        {
            BerReader reader = new BerReader(new MemoryStream(content));
            Asn1Sequence seq;
            reader.ReadOne<Asn1Sequence>(out seq);

            Algorithm = seq.Content.First() as Asn1ObjectIdentifier;
            Parameters = (seq.Content.Count == 2) ? seq.Content[1] : null;
        }

        public static AlgorithmIdentifierAsn1Net GetInstance(object obj)
        {
            if (obj == null || obj is AlgorithmIdentifierAsn1Net)
                return (AlgorithmIdentifierAsn1Net)obj;

            if (obj is Asn1Sequence)
                return new AlgorithmIdentifierAsn1Net((Asn1Sequence)obj);

            throw new ArgumentException("Unable to convert: " + obj.GetType().Name, "obj");
        }

        public Asn1ObjectBase Encode()
        {
            var content = new List<Asn1ObjectBase> { Algorithm };
            if (Parameters != null)
            {
                content.Add(Parameters);
            }

            var encoded = new Asn1Sequence(Asn1Class.Universal, content);
            return encoded;
        }
    }
}
