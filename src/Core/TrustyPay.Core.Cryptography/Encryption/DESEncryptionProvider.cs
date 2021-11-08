
using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// DES encryption algorithm (it should be deprecated!!!)
    /// </summary>
    public sealed class DESEncryptionProvider : IEncryptionProvider
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
        /// DES constructor
        /// </summary>
        /// <param name="secretKey">secret key</param>
        /// <param name="iv">initialization vector</param>
        /// <param name="mode">cipher mode, which supports CBC, ECB and CFB</param>
        public DESEncryptionProvider(
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

            using var des = new DESCryptoServiceProvider
            {
                BlockSize = 64,
                Mode = _mode
            };
            var encryptor = des.CreateEncryptor(_key, _iv);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(cipherBytes));
            }

            using var des = new DESCryptoServiceProvider
            {
                BlockSize = 64,
                Mode = _mode
            };
            var decryptor = des.CreateDecryptor(_key, _iv);
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

            byte[] targetKey = new byte[GetDefaultBlockSize() / 8];
            secretKey.PadLetters(targetKey, 'X');
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