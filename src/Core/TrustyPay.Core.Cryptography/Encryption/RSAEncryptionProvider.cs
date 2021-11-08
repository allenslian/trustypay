using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    public partial class RSACryptoProvider : IEncryptionProvider
    {
        /// <summary>
        /// Encryption padding
        /// </summary>
        private readonly RSAEncryptionPadding _encryptionPadding;

        /// <summary>
        /// RSA constructor
        /// </summary>
        /// <param name="privateKey">your private key</param>
        /// <param name="publicKey">another party public key</param>
        /// <param name="keySize">private/public key size</param>
        /// <param name="padding">encryption padding</param>
        public RSACryptoProvider(PrivateKey privateKey, PublicKey publicKey,
            KeySizes keySize = KeySizes.RSA2048, RSAEncryptionPadding padding = null)
            : this(privateKey, publicKey, null, padding, keySize)
        {
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            using var rsa = new RSACryptoServiceProvider((int)_keySize);
            switch (_publicKey.Format)
            {
                case PublicKeyFormat.Pkcs1:
                    rsa.ImportRSAPublicKey(_publicKey.Key, out _);
                    break;
                case PublicKeyFormat.X509:
                    rsa.ImportSubjectPublicKeyInfo(_publicKey.Key, out _);
                    break;
                default:
                    throw new NotSupportedException($"The '{_publicKey.Format}' isn't supported yet!");
            }
            return rsa.Encrypt(plainBytes, _encryptionPadding);
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(cipherBytes));
            }
            
            using var rsa = new RSACryptoServiceProvider((int)_keySize);
            switch (_privateKey.Format)
            {
                case PrivateKeyFormat.Pkcs1:
                    rsa.ImportRSAPrivateKey(_privateKey.Key, out _);
                    break;
                case PrivateKeyFormat.Pkcs8:
                    rsa.ImportPkcs8PrivateKey(_privateKey.Key, out _);
                    break;
                default:
                    throw new NotSupportedException($"The '{_privateKey.Format}' isn't supported yet!");
            }
            return rsa.Decrypt(cipherBytes, _encryptionPadding);
        }
    }
}