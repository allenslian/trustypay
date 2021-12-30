using System;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public class MissingAppIdOrApiKeyException : ArgumentException
    {
        public MissingAppIdOrApiKeyException() : base("Missing app id or api key!!!")
        {
        }
    }
}