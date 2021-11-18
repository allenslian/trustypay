using System;
using System.Text;

namespace TrustyPay.Core.Cryptography
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert hex string to byte array.
        /// </summary>
        /// <param name="source">hex string</param>
        /// <param name="paddingSize">padding size, which should be greater than one!</param>
        /// <returns>byte array</returns>
        /// <exception cref="ArgumentNullException">Throw the exception when source is null or empty</exception>
        /// <exception cref="ArgumentException">Throw the exception when paddingSize doesn't match source char array.</exception>
        public static byte[] FromHexString(this string source, ushort paddingSize = 2)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (paddingSize < 2)
            {
                throw new ArgumentException("paddingSize should be greater than one!", nameof(paddingSize));
            }

            if (source.Length % paddingSize != 0)
            {
                throw new ArgumentException("paddingSize is invalid!", nameof(paddingSize));
            }

            var buffer = new byte[source.Length / paddingSize];
            var index = 0;
            for (var i = 0; i < source.Length; i += paddingSize)
            {
                var byteValue = GetInt32Value(source[i + paddingSize - 2]) * 16 + GetInt32Value(source[i + paddingSize - 1]);
                buffer[index++] = (byte)byteValue;
            }
            return buffer;
        }

        /// <summary>
        /// Convert char to int
        /// </summary>
        /// <param name="c">char</param>
        /// <returns>int</returns>
        private static int GetInt32Value(char c)
        {
            if (c >= 'a')
            {
                return (c - 'a') + 10;
            }
            else if (c >= 'A')
            {
                return (c - 'A') + 10;
            }
            else
            {
                return c - '0';
            }
        }

        /// <summary>
        /// Convert base64 string to byte array.
        /// </summary>
        /// <param name="source">base64 string</param>
        /// <returns>byte array</returns>
        /// <exception cref="ArgumentNullException">Throw the exception when source is null or empty</exception>
        public static byte[] FromBase64String(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Convert.FromBase64String(source);
        }

        /// <summary>
        /// Convert ascii string to byte array.
        /// </summary>
        /// <param name="source">ascii string</param>
        /// <param name="charset">charset</param>
        /// <returns>byte array</returns>
        public static byte[] FromASCIIString(this string source)
        {
            if (source == null)
            {
                return Array.Empty<byte>();
            }

            return Encoding.ASCII.GetBytes(source);
        }

        /// <summary>
        /// Convert utf-8 string to byte array.
        /// </summary>
        /// <param name="source">ascii string</param>
        /// <param name="charset">charset</param>
        /// <returns>byte array</returns>
        public static byte[] FromUTF8String(this string source)
        {
            if (source == null)
            {
                return Array.Empty<byte>();
            }

            return Encoding.UTF8.GetBytes(source);
        }

        /// <summary>
        /// Convert charset string to byte array.
        /// </summary>
        /// <param name="source">charset string</param>
        /// <param name="charset">charset</param>
        /// <returns>byte array</returns>
        public static byte[] FromCharsetString(this string source, string charset)
        {
            if (string.IsNullOrEmpty(source))
            {
                return Array.Empty<byte>();
            }

            if (string.IsNullOrEmpty(charset))
            {
                return Encoding.Default.GetBytes(source);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(charset).GetBytes(source);
        }
    }
}