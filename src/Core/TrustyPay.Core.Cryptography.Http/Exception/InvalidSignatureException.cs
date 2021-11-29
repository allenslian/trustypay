
namespace TrustyPay.Core.Cryptography.Http
{
    public class InvalidSignatureException : SignatureException
    {
        public InvalidSignatureException() : base("INVALID signature!!!") { }
    } 
}