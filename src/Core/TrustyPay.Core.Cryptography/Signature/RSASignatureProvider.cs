using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// RSA algorithm
    /// </summary>
    public partial class RSACryptoProvider : ISignatureProvider
    {
        public enum KeySizes
        {
            RSA1024 = 1024,

            RSA2048 = 2048,

            RSA3072 = 3072,

            RSA4096 = 4096
        }

        public enum PrivateKeyFormat
        {
            Pkcs1 = 0x0,

            Pkcs8 = 0x1
        }

        public enum PublicKeyFormat
        {
            Pkcs1 = 0x0,

            X509 = 0x1
        }

        /// <summary>
        /// A private key
        /// </summary>
        public sealed class PrivateKey
        {
            public PrivateKey(byte[] key, PrivateKeyFormat format)
            {
                Key = key;
                Format = format;
            }

            /// <summary>
            /// Private key
            /// </summary>
            public byte[] Key { get; private set; }

            /// <summary>
            /// Key format
            /// </summary>
            public PrivateKeyFormat Format { get; private set; }
        }

        /// <summary>
        /// A public key
        /// </summary>
        public sealed class PublicKey
        {
            public PublicKey(byte[] key, PublicKeyFormat format)
            {
                Key = key;
                Format = format;
            }

            /// <summary>
            /// Public key
            /// </summary>
            public byte[] Key { get; private set; }

            /// <summary>
            /// Key format
            /// </summary>
            public PublicKeyFormat Format { get; private set; }
        }

        /// <summary>
        /// Private key
        /// </summary>
        private readonly PrivateKey _privateKey;

        /// <summary>
        /// Public key
        /// </summary>
        private readonly PublicKey _publicKey;

        /// <summary>
        /// Private/Public key size
        /// </summary>
        private readonly KeySizes _keySize;

        /// <summary>
        /// Signature padding
        /// </summary>
        private readonly RSASignaturePadding _signaturePadding;

        /// <summary>
        /// RSA constructor
        /// </summary>
        /// <param name="privateKey">your private key</param>
        /// <param name="publicKey">another party public key</param>
        /// <param name="padding">signature padding</param>
        /// <param name="keySize">private/public key size</param>
        public RSACryptoProvider(PrivateKey privateKey, PublicKey publicKey,
            RSASignaturePadding padding = null, KeySizes keySize = KeySizes.RSA2048)
            : this(privateKey, publicKey, padding, null, keySize)
        {
        }

        /// <summary>
        /// RSA constructor
        /// </summary>
        /// <param name="privateKey">your private key</param>
        /// <param name="publicKey">another party public key</param>
        /// <param name="keySize">private/public key size</param>
        /// <param name="signaturePadding">signature padding</param>
        /// <param name="encryptionPadding">signature padding</param>
        public RSACryptoProvider(PrivateKey privateKey, PublicKey publicKey,
            RSASignaturePadding signaturePadding,
            RSAEncryptionPadding encryptionPadding,
            KeySizes keySize = KeySizes.RSA2048)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
            _keySize = keySize;
            _signaturePadding = signaturePadding ?? RSASignaturePadding.Pkcs1;
            _encryptionPadding = encryptionPadding ?? RSAEncryptionPadding.Pkcs1;
        }

        public byte[] Sign(byte[] plainBytes, HashAlgorithmName hashAlgorithm)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
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
            return rsa.SignData(plainBytes, hashAlgorithm, _signaturePadding);
        }

        public bool Verify(byte[] plainBytes, byte[] signatureBytes, HashAlgorithmName hashAlgorithm)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            if (signatureBytes == null || signatureBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(signatureBytes));
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
            return rsa.VerifyData(plainBytes, signatureBytes, hashAlgorithm, _signaturePadding);
        }
    }
}