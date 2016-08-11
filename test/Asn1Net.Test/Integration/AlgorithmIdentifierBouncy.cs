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
using Org.BouncyCastle.Asn1;

namespace Net.Asn1.Writer.Tests.Integration
{
    public class AlgorithmIdentifierBouncy : Asn1Encodable
    {
        public DerObjectIdentifier Algorithm
        {
            get;
            private set;
        }

        public Asn1Encodable Parameters
        {
            get;
            private set;
        }

        public AlgorithmIdentifierBouncy(DerObjectIdentifier algorithm, Asn1Encodable parameters)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            Algorithm = algorithm;
            Parameters = parameters;
        }

        public AlgorithmIdentifierBouncy(Asn1Sequence seq)
        {
            if (seq == null)
                throw new ArgumentNullException("seq");

            if (seq.Count < 1 || seq.Count > 2)
                throw new ArgumentException("Invalid number of sequence members");

            Algorithm = DerObjectIdentifier.GetInstance(seq[0]);
            Parameters = (seq.Count == 2) ? seq[1] : null;
        }

        public static AlgorithmIdentifierBouncy GetInstance(object obj)
        {
            if (obj == null || obj is AlgorithmIdentifierBouncy)
                return (AlgorithmIdentifierBouncy)obj;

            if (obj is Asn1Sequence)
                return new AlgorithmIdentifierBouncy((Asn1Sequence)obj);

            throw new ArgumentException("Unable to convert: " + obj.GetType().Name, "obj");
        }

        public override Asn1Object ToAsn1Object()
        {
            if (Parameters == null)
                return new DerSequence(Algorithm);
            else
                return new DerSequence(Algorithm, Parameters);
        }
    }

}
