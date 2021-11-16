using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    public sealed class MD5DigestProvider : IDigestProvider
    {
        public byte[] Hash(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }
            using var md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(plainBytes);
        }
    }
}