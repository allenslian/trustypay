using System;

namespace TrustyPay.Core.Cryptography
{
    internal static class HexStringExtensions
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

            var buffer = new byte[source.Length/paddingSize];
            for (var i = 0; i < buffer.Length; i += 1)
            {
                buffer[i] = Convert.ToByte(
                    source[new Range(i * paddingSize, (i + 1) * paddingSize)], 
                    16);
            }
            return buffer;
        }
    }
}