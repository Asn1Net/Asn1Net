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

using System.Linq;
using Org.BouncyCastle.Asn1;
using Xunit;

namespace Net.Asn1.Writer.Tests.Integration
{
    public class Test
    {
        [Trait("Integration", "Integration")]
        [Fact]
        public void Integration()
        {
            var bc = new AlgorithmIdentifierBouncy(new Org.BouncyCastle.Asn1.DerObjectIdentifier("1.3.6.1.4.1.311.21.20"), DerNull.Instance);
            var bcEncoded = bc.ToAsn1Object().GetDerEncoded();

            var asn1net = new AlgorithmIdentifierAsn1Net(new Type.Asn1ObjectIdentifier("1.3.6.1.4.1.311.21.20"), new Net.Asn1.Type.Asn1Null());
            var asn1NetEncoded = asn1net.Encode().Write();

            Assert.True(Enumerable.SequenceEqual(bcEncoded, asn1NetEncoded));

            var bc2 = new AlgorithmIdentifierBouncy(Asn1Object.FromByteArray(asn1NetEncoded) as Asn1Sequence);

            var asn1Net2 = new AlgorithmIdentifierAsn1Net(bcEncoded);

            Assert.True(bc2.Algorithm.Id == asn1Net2.Algorithm.Content);
            Assert.True(Enumerable.SequenceEqual(bc2.Parameters.GetDerEncoded(), asn1Net2.Parameters.Write()));
        }
    }
}
