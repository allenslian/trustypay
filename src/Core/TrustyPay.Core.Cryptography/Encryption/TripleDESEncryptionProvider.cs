
using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// 3DES encryption algorithm
    /// </summary>
    public sealed class TripleDESEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// A secret key 
        /// </summary>
        private readonly byte[] _key;

        /// <summary>
        /// An initial vector
        /// </summary>
        private readonly byte[] _iv;

        /// <summary>
        /// Cipher mode
        /// </summary>
        private readonly CipherMode _mode;

        /// <summary>
        /// 3DES constructor
        /// </summary>
        /// <param name="secretKey">secret key</param>
        /// <param name="iv">initialization vector</param>
        /// <param name="mode">cipher mode, which supports CBC, ECB and CFB</param>
        public TripleDESEncryptionProvider(
            byte[] secretKey,
            byte[] iv,
            CipherMode mode = CipherMode.CBC)
        {
            _key = InitializeSecretKey(secretKey);
            _iv = InitializeIV(iv);
            _mode = mode;
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            using var tripleDES = new TripleDESCryptoServiceProvider
            {
                BlockSize = 64,
                Mode = _mode
            };
            var encryptor = tripleDES.CreateEncryptor(_key, _iv);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(cipherBytes));
            }

            using var tripleDES = new TripleDESCryptoServiceProvider
            {
                BlockSize = 64,
                Mode = _mode
            };
            var decryptor = tripleDES.CreateDecryptor(_key, _iv);
            return decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        }

        /// <summary>
        /// Initialize the secret key
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static byte[] InitializeSecretKey(byte[] secretKey)
        {
            if (secretKey == null || secretKey.Length == 0)
            {
                throw new ArgumentNullException(nameof(secretKey));
            }

            byte[] targetKey = secretKey.Length <= 16 ? new byte[16] : new byte[24];
            secretKey.PadLetters(
                targetKey, 
                secretKey.Length <= 16 ? 'X' : 'Y');
            return targetKey;
        }

        /// <summary>
        /// Initialize the iv
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        private static byte[] InitializeIV(byte[] iv)
        {
            var targetIV = new byte[GetDefaultBlockSize() / 8];
            if (iv == null || iv.Length == 0)
            {
                Array.Fill(targetIV, byte.MinValue);
                return targetIV;
            }

            iv.PadLetters(targetIV, 'O');
            return targetIV;
        }

        /// <summary>
        /// Get block size by its key size.
        /// </summary>
        /// <param name="keySize">key size</param>
        /// <returns></returns>
        private static int GetDefaultBlockSize()
        {
            return 64;
        }
    }
}