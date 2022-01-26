using System;
using Microsoft.AspNetCore.Builder;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseSignatureMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SignatureMiddleware>(new SignatureOption());
            return builder;
        }

        public static IApplicationBuilder UseSignatureMiddleware(this IApplicationBuilder builder, Action<SignatureOption> options)
        {
            var option = new SignatureOption();
            options?.Invoke(option);

            builder.UseMiddleware<SignatureMiddleware>(option);
            return builder;
        }
    }
}