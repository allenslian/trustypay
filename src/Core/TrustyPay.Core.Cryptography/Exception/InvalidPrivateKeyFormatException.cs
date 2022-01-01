

namespace TrustyPay.Core.Cryptography
{
    public sealed class InvalidPrivateKeyFormatException : SignatureException
    {
        public InvalidPrivateKeyFormatException() : base("The private key format is not valid!!!") { }
    }
}