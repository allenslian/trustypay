using System;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// SM2 private/public key generator
    /// </summary>
    public static class SM2KeyGenerator
    {
        /// <summary>
        /// Generate new private/public key.
        /// </summary>
        /// <returns>private key/public key</returns>
        public static Tuple<byte[], byte[]> GenerateKeyPair()
        {
            X9ECParameters ec = GMNamedCurves.GetByName("sm2p256v1");
            var keyGenParams = new ECKeyGenerationParameters(
                new ECDomainParameters(ec),
                new SecureRandom()
            );

            var generator = new ECKeyPairGenerator();
            generator.Init(keyGenParams);
            var keyPair = generator.GenerateKeyPair();

            var priKey = (ECPrivateKeyParameters)keyPair.Private;
            var pubKey = (ECPublicKeyParameters)keyPair.Public;
            return Tuple.Create(
                priKey.D.ToByteArray(),
                pubKey.Q.GetEncoded()
            );
        }
    }
}