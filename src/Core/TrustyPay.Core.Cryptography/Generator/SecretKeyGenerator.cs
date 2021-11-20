using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    public static class SecretKeyGenerator
    {
        /// <summary>
        /// A charsets, which excludes I,l,O,o because they are similiar.
        /// </summary>
        private const string Charsets = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz0123456789";

        /// <summary>
        /// Generate random bytes
        /// </summary>
        /// <param name="size">byte length</param>
        /// <returns>byte array</returns>
        private static byte[] GenerateRandomBytes(int size = 8)
        {
            if (size < 0)
            {
                throw new ArgumentException("The size should be greater than zero!", nameof(size));
            }

            if (size == 0)
            {
                return Array.Empty<byte>();
            }

            var buffer = new byte[size];
            RandomNumberGenerator.Fill(buffer);
            return buffer;
        }

        /// <summary>
        /// Generate random salt
        /// </summary>
        /// <param name="size">salt length</param>
        /// <returns>byte array</returns>
        public static byte[] GenerateRandomSalt(int size = 8)
        {
            return GenerateRandomBytes(size);
        }

        /// <summary>
        /// Generate random hex string.
        /// </summary>
        /// <param name="size">string length</param>
        /// <returns>hex string</returns>
        /// <exception cref="ArgumentException">If the size is odd number, it'll throw the exception!</exception>
        public static string GenerateRandomHexString(int size = 16)
        {
            if (size % 2 != 0)
            {
                throw new ArgumentException("The size should be even number!", nameof(size));
            }

            var buffer = GenerateRandomBytes(size / 2);
            return buffer.ToHexString();
        }

        /// <summary>
        /// Generate random base64 string.
        /// </summary>
        /// <param name="size">string length</param>
        /// <returns>base64 string</returns>
        public static string GenerateRandomBase64String(int size = 12)
        {
            int bufferSize = (int)Math.Ceiling((double)size * 6 / 8); // base64(6) and byte(8)
            var buffer = GenerateRandomBytes(bufferSize);
            return buffer.ToBase64String()[..size];
        }

        /// <summary>
        /// Generate random number string
        /// </summary>
        /// <param name="size">string length</param>
        /// <returns>number string</returns>
        public static string GenerateRandomNumbers(int size = 8)
        {
            var buffer = GenerateRandomBytes(size);
            var numbers = new char[size];
            for (var i = 0; i < buffer.Length; i++)
            {
                numbers[i] = (char)(buffer[i] % 10 + '0'); // 10 means 0-9 chars.
            }
            return new string(numbers);
        }

        /// <summary>
        /// Generate random alphabet string
        /// </summary>
        /// <param name="size">string length</param>
        /// <returns>alphabet string</returns>
        public static string GenerateRandomAlphabets(int size = 8)
        {
            var buffer = GenerateRandomBytes(size);
            var alphabets = new char[size];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 48;
                alphabets[i] = Charsets[m];
            }
            return new string(alphabets);
        }

        /// <summary>
        /// Generate random alphabet&amp;number string
        /// </summary>
        /// <param name="size">string length</param>
        /// <returns>alphabet&amp;number string</returns>
        public static string GenerateRandomAlphabetAndNumbers(int size = 8)
        {
            var buffer = GenerateRandomBytes(size);
            var chars = new char[size];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % Charsets.Length;
                chars[i] = Charsets[m];
            }
            return new string(chars);
        }
    }
}