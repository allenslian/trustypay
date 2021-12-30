using System;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public class InvalidAppPublicKeyException : ArgumentException
    {
        public InvalidAppPublicKeyException() : base("An invalid app public key!!!")
        {
        }
    }
}