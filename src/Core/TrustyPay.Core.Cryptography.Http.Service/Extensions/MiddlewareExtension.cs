

using Microsoft.AspNetCore.Builder;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseSignatureMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<VerifySignatureMiddleware>();
            return builder;
        }
    }
}