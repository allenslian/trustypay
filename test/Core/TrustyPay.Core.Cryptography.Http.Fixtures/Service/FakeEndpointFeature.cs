
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using TrustyPay.Core.Cryptography.Http.Service;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Service
{
    internal class FakeEndpointFeature : IEndpointFeature
    {
        public FakeEndpointFeature(RequestDelegate next, bool hasSignatureAttribute)
        {
            if (hasSignatureAttribute)
            {
                Endpoint = new Endpoint(
                    next,
                    new EndpointMetadataCollection(new SignatureVerificationAttribute()),
                    "GetEmployees");
            }
            else
            {
                Endpoint = new Endpoint(next, null, "GetEmployees");
            }
        }

        public Endpoint Endpoint { get; set; }
    }
}