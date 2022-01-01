

namespace TrustyPay.Core.Cryptography
{
    public sealed class InvalidPublicKeyFormatException : SignatureException
    {
        public InvalidPublicKeyFormatException() : base("The public key format is not valid!!!") { }
    }
}