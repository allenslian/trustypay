using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Flurl.Http.Testing;

using TrustyPay.Core.Cryptography.SDK;


namespace TrustyPay.Core.Cryptography.Http.Fixtures.SDK
{
    public class HttpClientFixture
    {
        [Fact]
        public void ShouldBeInvalidArguments()
        {
            IHttpClient client = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                client = new DefaultHttpClient("http://localhost:5000", null, "An_api_key");
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                client = new DefaultHttpClient("http://localhost:5000", "trustypay app001", null);
            });
        }

        [Fact]
        public async Task ShouldGetObjects()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", @"{
                        ""data"": [{
                            ""id"": ""0001"",
                            ""amount"": 100.00,
                            ""payer"": ""A"",
                            ""remarks"": null,
                            ""status"": 1,
                            ""dateCreated"": ""2021-10-01""
                        }]
                    }"
                },
                { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
            });

            IHttpClient client = new DefaultHttpClient(
                "http://localhost:5000",
                "trustypay app001",
                "An_api_key"
            );
            var result = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                "/api/payment/transfer-orders",
                "X0001223"
            );
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal("0001", result.Data[0].Id);
            Assert.Equal("2021-10-01", result.Data[0].DateCreated.ToString("yyyy-MM-dd"));
        }

        [Fact]
        public void ShouldBeInvalidResult()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response", @"{
                        ""data"": [{
                            ""id"": ""0001"",
                            ""amount"": 100.00,
                            ""payer"": ""A"",
                            ""remarks"": null,
                            ""status"": 1,
                            ""dateCreated"": ""2021-10-01""
                        }]
                    }"
                },
                { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
            });

            IHttpClient client = new DefaultHttpClient(
                "http://localhost:5000",
                "trustypay app001",
                "An_api_key"
            );
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var result = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                    "/api/payment/transfer-orders",
                    "X0001223"
                );
            });

        }

        [Fact]
        public async Task ShouldBeNullResult()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", null
                },
                { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
            });

            IHttpClient client = new DefaultHttpClient(
                "http://localhost:5000",
                "trustypay app001",
                "An_api_key"
            );

            var result = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                "/api/payment/transfer-orders",
                "X0001223"
            );
            Assert.Null(result);
        }

        [Fact]
        public async Task ShouldGetObjectsWithSignature()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", @"{""data"":[{""id"":""0001"",""amount"":100.00,""payer"":""A"",""remarks"":null,""status"":1,""dateCreated"":""2021-10-01""}]}"
                },
                { "charset", "utf-8" },
                { "timestamp", "1639272410256" },
                { "signType", 1 },
                { "", "test" },
                { "sign", "LbkA5jKa9b2+eIhQjPCvJXoaQazh5sYE/QEAUT2QQouuo8xeXnanwwiq7lLcm79I3zn4X8RIeW7c/gsHHGWNKCoyJj0/1Xjqo26VhCVAM2Ua/AZ3XleMkhBGwl78lkUl84Oa6otaYjKt4JPgibwIdN9oG/I/tkDs+HFXXsezDaI="}
            });

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            var result = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                "/api/payment/transfer-orders",
                "X0001223"
            );
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal("0001", result.Data[0].Id);
            Assert.Equal("2021-10-01", result.Data[0].DateCreated.ToString("yyyy-MM-dd"));
        }

        [Fact]
        public async Task ShouldGetStringWithSignature()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", "ok"
                },
                { "charset", "utf-8" },
                { "timestamp", "1639272410256" },
                { "signType", 1 },
                { "sign", "ZafKApTSNQoO5q7XzwG5Fo9BuHkTiD7KtK2i8K8o0855vOXey74gOgIaDEt8kmvK841iDIQqrfwubuY8hhzQ9J79fZvCrey2zlPmtf/f4bTRx5PZAdJWAwFEUW5YJuj+i8QG1Y2dhFsEbEuwVJNmaWLQGCiWH4FvHoFakPOHXyg="}
            });

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            var result = await client.PostJsonAsync<CreateOrder, string>(
                "/api/payment/transfer-orders", new CreateOrder
                {
                    Code = "N20211101003",
                    TotalAmount = 12.30M,
                    Payer = "Microsoft",
                    PayerBankAccount = "1234567890",
                    Items = new CreateOrder.OrderLineItem[]
                    {
                        new CreateOrder.OrderLineItem{
                            LineNo = 1,
                            Amount = 10M,
                            Payee = "Allen",
                            PayeeAccountName = "Allen",
                            PayeeAccountNO = "3123456789"
                        },
                        new CreateOrder.OrderLineItem{
                            LineNo = 2,
                            Amount = 2.30M,
                            Payee = "Bill",
                            PayeeAccountName = "Bill",
                            PayeeAccountNO = "4123456789"
                        }
                    }
                }
            );
            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task ShouldGetObjectsWithDefaultSignType()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", @"{""data"":[{""id"":""0001"",""amount"":100.00,""payer"":""A"",""remarks"":null,""status"":1,""dateCreated"":""2021-10-01""}]}"
                },
                { "charset", "utf-8" },
                { "timestamp", "1639272410256" },
                { "sign", "SR1dV8dqzEwea9nRHZuP62zoEyHxSwUN+4HMOsQnp7ydaZxQw3z6rJO2EuaC/86zBGqCZzqkwXjr/8N0zWiHjzVXe2Yw+svbKnzsvT1cAlVDIiaKH0eh9tavfI8laytfK94oaGZRukR1tUslL9vaFZJmJuPMdYsnZSM+lbHPf1k="}
            });

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            var result = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                "/api/payment/transfer-orders",
                "X0001223"
            );
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal("0001", result.Data[0].Id);
            Assert.Equal("2021-10-01", result.Data[0].DateCreated.ToString("yyyy-MM-dd"));
        }

        [Fact]
        public async Task ShouldGetError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", @"{""message"":""Not found the order!"",""code"":404}"
                },
                { "timestamp", "1639272410256" },
                { "sign", "bYUykvDXODrXkZ3yFH4FV9nJXuJEBY4BAimPzPvhdHViBDLeil/Y2UzBxmqRW9uAyBjokAfsu+reutfHRVVvlzCVOIaWSCdJXCt/H4H3/AZL6vrw6IRSCcKtotaoDx1SsRZPa5LzfcN+h9t0B4OeKsnLStd++RKTjuS8BA7uIMU="}
            }, 404);

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                try
                {
                    _ = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                        "/api/payment/transfer-orders",
                        "X0001223"
                    );
                }
                catch (Exception ex)
                {
                    Assert.Equal("Not found the order!", ex.Message);
                    throw ex;
                }
            });
        }

        [Fact]
        public async Task ShouldGetErrorWithoutResult()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                { "timestamp", "1639272410256" },
                { "sign", "dlGHk3lCsthYdIaE3OgH7KNjgDFi7U71xsF4eQe/rMBKM7uCwHiBK1+pEKKexC6tGBystBJna3P++Noqd8qRsDTZrftEx6K6PS83g4b3qNpXD+DHbDcftEiwTDpbE032DFK4xli2i3abK5/KRLKRUCC0YmmJFhsnP9iY9FgGz64="}
            }, 400);

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                try
                {
                    _ = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                        "/api/payment/transfer-orders",
                        "X0001223"
                    );
                }
                catch (ArgumentException ex)
                {
                    Assert.Equal("No any result in the response body!!!", ex.Message);
                    throw ex;
                }
            });
        }

        [Fact]
        public async Task ShouldGetErrorWhenResultIsNull()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", null
                },
                { "timestamp", "1639272410256" },
                { "sign", "dlGHk3lCsthYdIaE3OgH7KNjgDFi7U71xsF4eQe/rMBKM7uCwHiBK1+pEKKexC6tGBystBJna3P++Noqd8qRsDTZrftEx6K6PS83g4b3qNpXD+DHbDcftEiwTDpbE032DFK4xli2i3abK5/KRLKRUCC0YmmJFhsnP9iY9FgGz64="}
            }, 400);

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                try
                {
                    _ = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                        "/api/payment/transfer-orders",
                        "X0001223"
                    );
                }
                catch (ArgumentException ex)
                {
                    Assert.Equal("The error in the response body is null!", ex.Message);
                    throw ex;
                }
            });
        }

        [Fact]
        public async Task ShouldGetErrorWhenResultNotContainsMessage()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", @"{""error"":""Not found the order!"",""code"":400}"
                },
                { "timestamp", "1639272410256" },
                { "sign", "RLLyHq0+FMQkyI23gaHl21NPbcA4ZnjE7mPGmJK3LZaaBsQsFyhPuHWvb17GxcY0KaDYhOLyjqewcuBxlhlcOX+RQMTkfk2sMyj//lksjfvWhxfS96lod/b2dHxg8f8oItBxUjZzb0VFLjZ6/RAY6iPqrh7gxDOa095VQX5Z6Hk="}
            }, 400);

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                try
                {
                    _ = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                        "/api/payment/transfer-orders",
                        "X0001223"
                    );
                }
                catch (ArgumentException ex)
                {
                    Assert.Equal("The result doesn't contain any property called 'message'!", ex.Message);
                    throw ex;
                }
            });
        }

        [Fact]
        public async Task ShouldMissingSignature()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "result", @"{""data"":[{""id"":""0001"",""amount"":100.00,""payer"":""A"",""remarks"":null,""status"":1,""dateCreated"":""2021-10-01""}]}"
                },
                { "charset", "utf-8" },
                { "timestamp", "1639272410256" },
                { "signType", 1 },
            });

            IHttpClient client = new HttpClientBuilder<DefaultHttpClient>(
                new DefaultHttpClient("http://localhost:5000", "trustypay app001", "An_api_key"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICWwIBAAKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQABAoGAHW8hODhl97wlI4c3wsjwgwdkKSEuWU+yhU/OjgzQRzYhhcbDtA62T6K+XIqNJNJ4LHBJNy7015PxMylTfY9L/TNGPIZLvYOftmEyOFEgUSXcmwzbTKbiLGMCNEJCQ96VYPrw4C6x6VXp0HbwuWvIGAsAIYSnnB32Jl/tgi/ehIECQQDGGI31Jnm69XsHMWfAfMDNvyf71VRu3lOxrijzToU5Z5dmsEHV6B0ql8fh0gl6OvZlCzHE7yyXMiebWmEec9/NAkEAqehPbT/+5/BbiB5h8SNEp8o/JFZ/1ErDmO/lE2XUp69hSRGeVvy1IUuAGLbc3EJEk6AtQPO0vszg2s9+vDtmDwJAQCG465HePQG2J7j97to0jSeCqUwCPrZpgA9zIHneNZxs7ojHd5niv2ROCLS37sNh+4ppPWl1FSnemrPi1zoKTQJAXwObjyOvf7Lo7MjYyomHdjFieAarO2OH2DmnJ97VOeSYic0Bd/GftPvMqYVxIvcn9EoppF3koKJfx90rKUYqPwJAUW9+b4CBjGexFVA5/lH4sK4llgtYz+0axRcfviHScZWoE0xDCwGbw/zBEhp6e85x+upAb5w6vJGXw/7H6C1now==",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCDefltmlmdxMLbFfjqhBsCXwstCi4/DwClT1oiiVK0dNiQwVoU+uBswW5CmSehoiVjz81SOAtH5ngq5ZhEuYlL9p+1rnqjVqK7fv2ZLSteEHADoGvs3hR9R4WdUaLex5E9pb1S6EEWjP1a+bfOjB3SDpZfYrr44i4CbQTIFDTLAwIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            await Assert.ThrowsAsync<MissingSignatureException>(async () =>
            {
                var result = await client.GetAsync<string, ApiResult<OrderResult[]>>(
                    "/api/payment/transfer-orders",
                    "X0001223"
                );
            });
        }
    }
}