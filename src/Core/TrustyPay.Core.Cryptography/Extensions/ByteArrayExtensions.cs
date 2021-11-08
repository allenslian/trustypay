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
            if (source == null || target == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Buffer.BlockCopy(source, 0, target, 0, Math.Min(source.Length, target.Length));
            if (target.Length > source.Length)
            {
                // it is not secure.
                Array.Fill(target, (byte)letter, source.Length, target.Length - source.Length);
            }
        }
    }
}