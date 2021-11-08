using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// AES encryption algorithm
    /// </summary>
    public sealed class AESEncryptionProvider : IEncryptionProvider
    {
        public enum KeySizes
        {
            AES128 = 128,

            AES192 = 192,

            AES256 = 256
        }

        /// <summary>
        /// A secret key 
        /// </summary>
        private readonly byte[] _key;

        /// <summary>
        /// An initial vector
        /// </summary>
        private readonly byte[] _iv;

        /// <summary>
        /// Padding mode
        /// </summary>
        private readonly PaddingMode _padding;

        /// <summary>
        /// Key size
        /// </summary>
        private readonly KeySizes _keySize;

        /// <summary>
        /// Cipher mode
        /// </summary>
        private readonly CipherMode _mode;

        /// <summary>
        /// AES constructor
        /// </summary>
        /// <param name="secretKey">secret key</param>
        /// <param name="iv">initialization vector, whose length is 16 by default.</param>
        /// <param name="keySize">key size</param>
        /// <param name="mode">cipher mode, which supports CBC, ECB and CFB</param>
        /// <param name="padding">padding mode</param>
        /// <exception cref="ArgumentNullException">Throw an exception when secret key is null or empty</exception>
        public AESEncryptionProvider(byte[] secretKey,
            byte[] iv,
            KeySizes keySize = KeySizes.AES128,
            CipherMode mode = CipherMode.CBC,
            PaddingMode padding = PaddingMode.PKCS7)
        {
            _key = InitializeSecretKey(secretKey, keySize);
            _iv = InitializeIV(iv, keySize);
            _keySize = keySize;
            _padding = padding;
            _mode = mode;
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            using var aes = new AesCryptoServiceProvider
            {
                KeySize = (int)_keySize,
                Padding = _padding,
                Mode = _mode,
                BlockSize = GetDefaultBlockSize(_keySize)
            };

            var encryptor = aes.CreateEncryptor(_key, _iv);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(cipherBytes));
            }

            using var aes = new AesCryptoServiceProvider
            {
                KeySize = (int)_keySize,
                Padding = _padding,
                Mode = _mode,
                BlockSize = GetDefaultBlockSize(_keySize)
            };

            var decryptor = aes.CreateDecryptor(_key, _iv);
            return decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        }

        /// <summary>
        /// Initialize the secret key
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static byte[] InitializeSecretKey(byte[] secretKey, KeySizes keySize)
        {
            if (secretKey == null || secretKey.Length == 0)
            {
                throw new ArgumentNullException(nameof(secretKey));
            }

            var targetKey = new byte[(int)keySize / 8];
            secretKey.PadLetters(targetKey, GetLetterPaddedByKeySize(keySize));
            return targetKey;
        }

        /// <summary>
        /// Initialize the iv
        /// </summary>
        /// <param name="iv"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        private static byte[] InitializeIV(byte[] iv, KeySizes keySize)
        {
            var targetIV = new byte[GetDefaultBlockSize(keySize) / 8];
            if (iv == null || iv.Length == 0)
            {
                Array.Fill(targetIV, byte.MinValue);
                return targetIV;
            }

            iv.PadLetters(targetIV, GetLetterPaddedByKeySize(keySize));
            return targetIV;
        }

        /// <summary>
        /// Get letter by key size.
        /// </summary>
        /// <param name="keySize"></param>
        /// <returns>A char</returns>
        private static char GetLetterPaddedByKeySize(KeySizes keySize)
        {
            return keySize switch
            {
                KeySizes.AES128 => 'X',
                KeySizes.AES192 => 'Y',
                KeySizes.AES256 => 'Z',
                _ => 'O'
            };
        }

        /// <summary>
        /// Get block size by its key size.
        /// </summary>
        /// <param name="keySize">key size</param>
        /// <returns></returns>
        private static int GetDefaultBlockSize(KeySizes keySize)
        {
            return keySize switch
            {
                KeySizes.AES128 => 128,
                KeySizes.AES192 => 128,
                KeySizes.AES256 => 128,
                _ => throw new NotSupportedException($"Not support the key size({(int)keySize})!"),
            };
        }
    }
}