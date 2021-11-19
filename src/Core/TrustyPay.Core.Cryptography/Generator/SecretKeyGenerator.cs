using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    public class SecretKeyGenerator
    {
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
            var chars = new char[size];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 52;
                chars[i] = m >= 26 ? (char)(m - 26 + 'a') : (char)(m + 'A'); // 52 means a-z + A-Z
            }
            return new string(chars);
        }

        /// <summary>
        /// Generate random alphabet&amp;number string
        /// </summary>
        /// <param name="size">string length</param>
        /// <returns>alphabet&amp;number string</returns>
        public static string GenerateRandomAlphabetAndNumbers(int size = 8)
        {
            var buffer = GenerateRandomBytes(size);
            var numbers = new char[size];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 62; // 62 means 0-9 + a-z + A-Z
                if (m >= 36)
                {
                    numbers[i] = (char)(m - 36 + 'a');
                }
                else if (m >= 10)
                {
                    numbers[i] = (char)(m - 10 + 'A');
                }
                else
                {
                    numbers[i] = (char)(m + '0');
                }
            }
            return new string(numbers);
        }
    }
}