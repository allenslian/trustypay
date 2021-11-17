using System;
using System.Text;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// An extension for IEncryptionProvider
    /// </summary>
    public static class EncryptionProviderExtensions
    {
        /// <summary>
        /// Encrypt plain byte array to base64 cipher text.
        /// </summary>
        /// <param name="source">IEncryptionProvider instance</param>
        /// <param name="plainBytes">plain byte array</param>
        /// <returns>base64 cipher text</returns>
        /// <exception cref="ArgumentNullException">source is null, or plainBytes is null, or plainBytes length is zero.</exception>
        public static string EncryptToBase64String(
            this IEncryptionProvider source,
            byte[] plainBytes)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            var cipherBytes = source.Encrypt(plainBytes);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// Encrypt plain byte array to hex cipher text.
        /// </summary>
        /// <param name="source">IEncryptionProvider instance</param>
        /// <param name="plainBytes">plain byte array</param>
        /// <param name="paddingSize">padding size, which is 2 by default</param>
        /// <returns>hex cipher text</returns>
        /// <exception cref="ArgumentNullException">source is null, or plainBytes is null, or plainBytes length is zero.</exception>
        public static string EncryptToHexString(
            this IEncryptionProvider source,
            byte[] plainBytes, ushort paddingSize = 2)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }

            var cipherBytes = source.Encrypt(plainBytes);
            return cipherBytes.ToHexString(paddingSize);
        }


        /// <summary>
        /// Decrypt from base64 cipher text to plain byte array.
        /// </summary>
        /// <param name="source">IEncryptionProvider instance</param>
        /// <param name="cipherText">base64 cipher text</param>
        /// <returns>plain byte array</returns>
        /// <exception cref="ArgumentNullException">source is null, or cipherText is null, or cipherText is empty.</exception>
        public static byte[] DecryptFromBase64String(
            this IEncryptionProvider source,
            string cipherText)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            var cipherBytes = cipherText.FromBase64String();
            return source.Decrypt(cipherBytes);
        }

        /// <summary>
        /// Decrypt from hex cipher text to plain byte array.
        /// </summary>
        /// <param name="source">IEncryptionProvider instance</param>
        /// <param name="cipherText">hex cipher text</param>
        /// <param name="paddingSize">padding size, which is 2 by default</param>
        /// <returns>plain byte array</returns>
        /// <exception cref="ArgumentNullException">source is null, or cipherText is null, or cipherText is empty.</exception>
        public static byte[] DecryptFromHexString(
            this IEncryptionProvider source,
            string cipherText, ushort paddingSize = 2)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // if (string.IsNullOrEmpty(cipherText))
            // {
            //     throw new ArgumentNullException(nameof(cipherText));
            // }

            var cipherBytes = cipherText.FromHexString(paddingSize);
            return source.Decrypt(cipherBytes);
        }
    }
}