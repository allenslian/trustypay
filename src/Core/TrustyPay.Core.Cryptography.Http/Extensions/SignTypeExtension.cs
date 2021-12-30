using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography.Http
{
    public static class SignTypeExtension
    {
        public static HashAlgorithmName ToHashAlgorithmName(this SignType type)
        {
            return type switch
            {
                SignType.RS256 => HashAlgorithmName.SHA256,
                SignType.RS384 => HashAlgorithmName.SHA384,
                SignType.RS512 => HashAlgorithmName.SHA512,
                _ => HashAlgorithmName.SHA256,
            };
        }
    }
}