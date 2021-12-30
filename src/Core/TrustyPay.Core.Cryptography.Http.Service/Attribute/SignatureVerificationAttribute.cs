using System;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class SignatureVerificationAttribute : Attribute
    {

    }
}