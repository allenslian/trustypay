using System;

namespace TrustyPay.Core.Cryptography
{
    internal static class ByteArrayExtensions
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

            var buffer = new System.Text.StringBuilder(source.Length * paddingSize);
            var format = $"{{0:X{paddingSize}}}";
            foreach (var b in source)
            {
                buffer.AppendFormat(format, b);
            }
            return buffer.ToString();
        }
    }
}