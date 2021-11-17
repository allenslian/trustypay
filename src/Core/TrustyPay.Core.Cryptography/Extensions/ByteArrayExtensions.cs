using System;

namespace TrustyPay.Core.Cryptography
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// If source's length is greater than target's length, we'll only copy target's length.
        /// Otherwise we'll fill the letters into target.
        /// </summary>
        /// <param name="source">source byte array</param>
        /// <param name="target">target byte array</param>
        /// <param name="letter">padding letter</param>
        /// <exception cref="ArgumentNullException">Throw the exception when source is null or target is null</exception>
        public static void PadLetters(this byte[] source, byte[] target, char letter)
        {
            if (source == null || source.Length == 0)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null || target.Length == 0)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Buffer.BlockCopy(source, 0, target, 0, Math.Min(source.Length, target.Length));
            if (target.Length > source.Length)
            {
                // it is not secure.
                Array.Fill(target, (byte)letter, source.Length, target.Length - source.Length);
            }
        }

        /// <summary>
        /// Convert byte array to base64 string.
        /// </summary>
        /// <param name="source">byte array</param>
        /// <returns>base64 string</returns>
        /// <exception cref="ArgumentNullException">Throw the exception when source is null or source's length is zero</exception>
        public static string ToBase64String(this byte[] source)
        {
            if (source == null || source.Length == 0)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Convert.ToBase64String(source);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="source">source byte array</param>
        /// <param name="paddingSize">padding size</param>
        /// <returns>hex string</returns>
        /// <exception cref="ArgumentNullException">Throw the exception when source is null or source's length is zero</exception>
        public static string ToHexString(this byte[] source, ushort paddingSize = 2)
        {
            if (source == null || source.Length == 0)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (paddingSize < 2)
            {
                throw new ArgumentException("paddingSize should be greater than one!", nameof(paddingSize));
            }

            if (source.Length > int.MaxValue / paddingSize)
            {
                throw new ArgumentOutOfRangeException(nameof(source), "source length is more than int max value!");
            }

            var index = 0;
            var buffer = new char[source.Length * paddingSize];
            Array.Fill(buffer, '0');
            for (var i = 0; i < buffer.Length; i += paddingSize)
            {
                byte b = source[index++];
                buffer[i + paddingSize - 2] = GetHexChar(b / 16);
                buffer[i + paddingSize - 1] = GetHexChar(b % 16);
            }
            return new string(buffer);
        }

        private static char GetHexChar(int i)
        {
            if (i < 10)
            {
                return (char)('0' + i);
            }
            return (char)('A' + i - 10);
        }
    }
}