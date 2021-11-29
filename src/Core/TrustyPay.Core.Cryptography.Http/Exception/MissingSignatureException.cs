
namespace TrustyPay.Core.Cryptography.Http
{
    public class MissingSignatureException : SignatureException
    {
        public MissingSignatureException() : base("MISSING signature!!!") { }
    } 
}