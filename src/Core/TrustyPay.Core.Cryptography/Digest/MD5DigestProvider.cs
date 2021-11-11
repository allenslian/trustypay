using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    public sealed class MD5DigestProvider : IDigestProvider
    {
        public byte[] Hash(byte[] plainBytes)
        {
            using var md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(plainBytes);
        }
    }
}