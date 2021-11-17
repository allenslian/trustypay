using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// SM4 encryption algorithm
    /// </summary>
    public class SM4EncryptionProvider : IEncryptionProvider
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
        /// Padding mode
        /// </summary>
        private readonly PaddingMode _padding;

        /// <summary>
        /// Cipher mode
        /// </summary>
        private readonly CipherMode _mode;

        /// <summary>
        /// SM4 constructor
        /// </summary>
        /// <param name="secretKey">secret key</param>
        /// <param name="iv">initialization vector</param>
        /// <param name="mode">cipher mode</param>
        /// <param name="padding">padding mode</param>
        public SM4EncryptionProvider(
            byte[] secretKey,
            byte[] iv,
            CipherMode mode = CipherMode.CBC,
            PaddingMode padding = PaddingMode.PKCS7)
        {
            // if (mode != CipherMode.CBC && mode != CipherMode.ECB)
            // {
            //     throw new ArgumentException(
            //         "SM4 only supports ECB and CBC cipher!",
            //         nameof(mode));
            // }

            // if (padding != PaddingMode.PKCS7 && padding != PaddingMode.Zeros)
            // {
            //     throw new ArgumentException(
            //         "SM4 only supports ZeroPadding and PKCS7Padding/PKCS5Padding!",
            //         nameof(padding));
            // }

            _key = InitializeSecretKey(secretKey);
            _iv = InitializeIV(iv);
            _mode = mode;
            _padding = padding;
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            PaddedBufferedBlockCipher cipher = null;
            switch (_mode)
            {
                case CipherMode.CBC:
                    {
                        cipher = new PaddedBufferedBlockCipher(
                            new CbcBlockCipher(new SM4Engine()),
                            ConvertBlockCipherPadding(_padding)
                        );
                        cipher.Init(true, new ParametersWithIV(
                            new KeyParameter(_key, 0, _key.Length), _iv));
                        return cipher.DoFinal(plainBytes);
                    }
                case CipherMode.ECB:
                    {
                        cipher = new PaddedBufferedBlockCipher(
                            new SM4Engine(),
                            ConvertBlockCipherPadding(_padding)
                        );
                        cipher.Init(true, new KeyParameter(_key, 0, _key.Length));
                        return cipher.DoFinal(plainBytes);
                    }
                default:
                    throw new NotSupportedException("SM4 doesn't support the cipher mode!");
            }
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(cipherBytes));
            }

            PaddedBufferedBlockCipher cipher = null;
            switch (_mode)
            {
                case CipherMode.CBC:
                    {
                        cipher = new PaddedBufferedBlockCipher(
                            new CbcBlockCipher(new SM4Engine()),
                            ConvertBlockCipherPadding(_padding)
                        );
                        cipher.Init(false, new ParametersWithIV(
                            new KeyParameter(_key, 0, _key.Length), _iv));
                        return cipher.DoFinal(cipherBytes);
                    }
                case CipherMode.ECB:
                    {
                        cipher = new PaddedBufferedBlockCipher(
                            new SM4Engine(),
                            ConvertBlockCipherPadding(_padding)
                        );
                        cipher.Init(false, new KeyParameter(_key, 0, _key.Length));
                        return cipher.DoFinal(cipherBytes);
                    }
                default:
                    throw new NotSupportedException("SM4 doesn't support the cipher mode!");
            }
        }

        private IBlockCipherPadding ConvertBlockCipherPadding(PaddingMode padding)
        {
            return padding switch
            {
                PaddingMode.PKCS7 => new Pkcs7Padding(),
                PaddingMode.Zeros => new ZeroBytePadding(),
                PaddingMode.ANSIX923 => new X923Padding(),
                _ => throw new NotSupportedException("Not support padding mode!!")
            };
        }

        /// <summary>
        /// Initialize the secret key
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual byte[] InitializeSecretKey(byte[] secretKey)
        {
            if (secretKey == null || secretKey.Length == 0)
            {
                throw new ArgumentNullException(nameof(secretKey));
            }

            var targetKey = new byte[GetDefaultBlockSize() / 8];
            secretKey.PadLetters(targetKey, 'O');
            return targetKey;
        }

        /// <summary>
        /// Initialize the iv
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        protected virtual byte[] InitializeIV(byte[] iv)
        {
            var targetIV = new byte[GetDefaultBlockSize() / 8];
            if (iv == null || iv.Length == 0)
            {
                Array.Fill(targetIV, byte.MinValue);
                return targetIV;
            }

            iv.PadLetters(targetIV, 'I');
            return targetIV;
        }

        /// <summary>
        /// Get block size
        /// </summary>
        /// <returns>128 bit</returns>
        protected static int GetDefaultBlockSize()
        {
            return 128;
        }
    }
}