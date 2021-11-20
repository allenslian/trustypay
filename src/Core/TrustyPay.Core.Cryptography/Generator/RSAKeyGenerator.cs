using System;
using System.Linq;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// RSA private/public key generator
    /// </summary>
    public static class RSAKeyGenerator
    {
        /// <summary>
        /// key sizes supported
        /// </summary>
        private static int[] KeySizes = new int[] { 1024, 2048, 3072, 4096 };

        /// <summary>
        /// Ensure valid key size
        /// </summary>
        /// <param name="keySize">key size</param>
        /// <exception cref="ArgumentNullException">keySize isn't 1024/2048/3072/4096</exception>
        private static void EnsureValidKeySize(int keySize)
        {
            if (!KeySizes.Contains(keySize))
            {
                throw new ArgumentException("The keySize is invalid! it should be 1024/2048/3072/4096!", nameof(keySize));
            }
        }

        /// <summary>
        /// Generate new private/public key.
        /// </summary>
        /// <param name="keySize">key size, it denotes encrypted/signed value's bit size! such as 2048 = 256 * 8 bits</param>
        /// <param name="format">privat key format: pkcs1/pkcs8</param>
        /// <returns>PrivateKey &amp; PublicKey</returns>
        /// <exception cref="NotSupportedException">not pkcs1/pkcs8</exception>
        public static Tuple<RSACryptoProvider.PrivateKey, RSACryptoProvider.PublicKey> GenerateKeyPair(
            int keySize = 2048,
            RSACryptoProvider.PrivateKeyFormat format = RSACryptoProvider.PrivateKeyFormat.Pkcs1)
        {
            EnsureValidKeySize(keySize);

            using var rsa = new RSACryptoServiceProvider(keySize);
            switch (format)
            {
                case RSACryptoProvider.PrivateKeyFormat.Pkcs1:
                    return Tuple.Create(
                        new RSACryptoProvider.PrivateKey(
                            rsa.ExportRSAPrivateKey(),
                            RSACryptoProvider.PrivateKeyFormat.Pkcs1
                        ),
                        new RSACryptoProvider.PublicKey(
                            rsa.ExportRSAPublicKey(),
                            RSACryptoProvider.PublicKeyFormat.Pkcs1
                        ));
                case RSACryptoProvider.PrivateKeyFormat.Pkcs8:
                    return Tuple.Create(
                        new RSACryptoProvider.PrivateKey(
                            rsa.ExportPkcs8PrivateKey(),
                            RSACryptoProvider.PrivateKeyFormat.Pkcs8
                        ),
                        new RSACryptoProvider.PublicKey(
                            rsa.ExportSubjectPublicKeyInfo(),
                            RSACryptoProvider.PublicKeyFormat.X509
                        ));
                default:
                    throw new NotSupportedException("Not support the private key format!");
            }
        }
    }
}