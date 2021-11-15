
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    public sealed class SHA256DigestProvider : IDigestProvider
    {
        public byte[] Hash(byte[] plainBytes)
        {
            using var sha256 = new SHA256CryptoServiceProvider();
            return sha256.ComputeHash(plainBytes);
        }
    }
}