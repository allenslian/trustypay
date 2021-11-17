using System;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// SM2 algorithm
    /// </summary>
    public partial class SM2CryptoProvider : IEncryptionProvider
    {
        public enum CipherFormat
        {
            C1C2C3 = 0x0,

            C1C3C2 = 0x1
        }

        /// <summary>
        /// Private Key
        /// </summary>
        private byte[] _privateKey;

        /// <summary>
        /// Public Key
        /// </summary>
        private byte[] _publicKey;

        /// <summary>
        /// Cipher format
        /// </summary>
        private CipherFormat _format;

        /// <summary>
        /// EC Parameter
        /// </summary>
        private X9ECParameters _ec;

        /// <summary>
        /// SM2 engine
        /// </summary>
        private SM2Engine _sm2;

        /// <summary>
        /// sm3 digest length
        /// </summary>
        private const int SM3DigestLength = 32;

        /// <summary>
        /// SM2 constructor
        /// </summary>
        /// <param name="privateKey">your private key</param>
        /// <param name="publicKey">other public key</param>
        /// <param name="format">cipher format</param>
        /// <exception cref="ArgumentNullException">private/public key is null or emtpy.</exception>
        public SM2CryptoProvider(
            byte[] privateKey, byte[] publicKey,
            CipherFormat format = CipherFormat.C1C2C3)
        {
            if (privateKey == null || privateKey.Length == 0)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            if (publicKey == null || publicKey.Length == 0)
            {
                throw new ArgumentNullException(nameof(publicKey));
            }

            _ec = GMNamedCurves.GetByName("sm2p256v1");
            _sm2 = new SM2Engine();
            _privateKey = privateKey;
            _publicKey = publicKey;
            _format = format;
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            var pubKey = new ECPublicKeyParameters(
                _ec.Curve.DecodePoint(_publicKey),
                new ECDomainParameters(_ec)
            );

            _sm2.Init(true, new ParametersWithRandom(pubKey, new SecureRandom()));

            var cipherBytes = _sm2.ProcessBlock(plainBytes, 0, plainBytes.Length);
            return _format == CipherFormat.C1C3C2 ? ToC1C3C2(cipherBytes) : cipherBytes;
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(cipherBytes));
            }

            var priKey = new ECPrivateKeyParameters(
                new BigInteger(_privateKey),
                new ECDomainParameters(_ec)
            );

            _sm2.Init(false, priKey);

            cipherBytes = _format == CipherFormat.C1C3C2 ? ToC1C2C3(cipherBytes) : cipherBytes;
            var plainBytes = _sm2.ProcessBlock(cipherBytes, 0, cipherBytes.Length);
            return plainBytes;
        }

        private byte[] ToC1C2C3(byte[] cipherBytes)
        {
            int c1 = (_ec.Curve.FieldSize + 7) / 8 * 2 + 1;
            var buffer = new byte[cipherBytes.Length];
            Buffer.BlockCopy(cipherBytes, 0, buffer, 0, c1);
            Buffer.BlockCopy(cipherBytes, c1 + SM3DigestLength, buffer, c1, cipherBytes.Length - c1 - SM3DigestLength);
            Buffer.BlockCopy(cipherBytes, c1, buffer, cipherBytes.Length - SM3DigestLength, SM3DigestLength);
            return buffer;
        }

        private byte[] ToC1C3C2(byte[] cipherBytes)
        {
            int c1 = (_ec.Curve.FieldSize + 7) / 8 * 2 + 1;
            var buffer = new byte[cipherBytes.Length];
            Buffer.BlockCopy(cipherBytes, 0, buffer, 0, c1);
            Buffer.BlockCopy(cipherBytes, cipherBytes.Length - SM3DigestLength, buffer, c1, SM3DigestLength);
            Buffer.BlockCopy(cipherBytes, c1, buffer, c1 + SM3DigestLength, cipherBytes.Length - c1 - SM3DigestLength);
            return buffer;
        }
    }
}