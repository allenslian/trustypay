
using System;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public class InvalidTimestampException : ArgumentException
    {
        public InvalidTimestampException() : base("Invalid timestamp!!!")
        {
        }
    }
}