using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Flurl.Http.Testing;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Http
{
    public class HttpClientFixture
    {
        [Fact]
        public async Task ShouldBeNull()
        {
            IHttpClient client = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await client.GetAsync<OrderCriteria, ResponseBizContent>(null, null);
            });

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await client.PostJsonAsync<CreateOrder, CreateOrderResult>(null, null);
            });

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await client.PutJsonAsync<CreateOrder, CreateOrderResult>(null, null);
            });

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await client.DeleteAsync<CreateOrder, CreateOrderResult>(null, null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                client = new HttpClientBuilder(null).Build();
            });
        }

        [Fact]
        public async Task ShouldBeInvalidUrl()
        {
            IHttpClient client = new FakeHttpClient(null, "abcd1234");
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var bizContent = await client.GetAsync<OrderCriteria, ResponseBizContent>(
                "", new OrderCriteria
                {
                    OrderCode = "2021021501",
                }, null);
            });

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var bizContent = await client.GetAsync<OrderCriteria, ResponseBizContent>(
                "/api/v1/orders", new OrderCriteria
                {
                    OrderCode = "2021021501",
                }, null);
            });
        }

        [Fact]
        public async Task ShouldBeValidUrl()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{
                        ""return_code"": ""0"", 
                        ""return_msg"": ""OK"",
                        ""status"": ""1"",
                        ""sumPayamt"": ""100"",
                        ""payeeList"": [
                        {
                            ""payAmount"": ""100"",
                            ""payeeCompanyName"": ""SHQY024"",
                            ""payeeAccno"": ""1234562019212019112"",
                            ""payeeAddress"":""shoukaufna"",
                            ""payeeOrgcode"": ""0000000008""
                        }]
                    }"
                },
                { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
            });

            IHttpClient client = new FakeHttpClient(null, "abcd1234");
            var bizContent = await client.GetAsync<OrderCriteria, ResponseBizContent>(
            "http://localhost:5000/api/v1/orders", new OrderCriteria
            {
                OrderCode = "2021021501",
            }, null);
            Assert.Equal("0", bizContent.ReturnCode);
        }

        [Fact]
        public async Task ShouldGetOneError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{
                        ""return_code"": 400014, 
                        ""return_msg"": ""交易时间戳解析异常""
                    }"
                },
                { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
            }, 400);

            IHttpClient client = new FakeHttpClient("http://localhost:5000", "abcd1234");
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                try
                {
                    var bizContent = await client.GetAsync<OrderCriteria, ResponseBizContent>("/api/v1/orders", new OrderCriteria
                    {
                        OrderCode = "2021021501",
                    }, null);
                }
                catch (Exception ex)
                {
                    Assert.Equal("交易时间戳解析异常", ex.Message);
                    throw;
                }
            });
        }

        [Fact]
        public async Task ShouldGetOneObject()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{
                        ""return_code"": ""0"", 
                        ""return_msg"": ""OK"",
                        ""status"": ""1"",
                        ""sumPayamt"": ""100"",
                        ""payeeList"": [
                        {
                            ""payAmount"": ""100"",
                            ""payeeCompanyName"": ""SHQY024"",
                            ""payeeAccno"": ""1234562019212019112"",
                            ""payeeAddress"":""shoukaufna"",
                            ""payeeOrgcode"": ""0000000008""
                        }]
                    }"
                },
                { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
            });

            IHttpClient client = new FakeHttpClient("http://localhost:5000", "abcd1234");
            var bizContent = await client.GetAsync<OrderCriteria, OrderResult>("/api/v1/orders", new OrderCriteria
            {
                OrderCode = "2021021501",
            }, null);

            Assert.Equal("0", bizContent.ReturnCode);
            Assert.Equal("OK", bizContent.ReturnMsg);
            Assert.Equal("1", bizContent.Status);
            Assert.NotEmpty(bizContent.Payees);
            Assert.Equal("100", bizContent.Payees[0].Amount);
        }

        [Fact]
        public async Task ShouldPostOneObject()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{
                        ""return_code"": ""0"", 
                        ""return_msg"": ""OK"",
                        ""orderCurr"": ""001"",
                        ""payAmount"": ""12.3"",
                        ""partnerSeq"": ""11111112222222"",
                        ""status"": ""1""
                    }"
                },
                { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
            });

            IHttpClient client = new FakeHttpClient("http://localhost:5000", "abcd1234");
            var bizContent = await client.PostJsonAsync<CreateOrder, CreateOrderResult>("/api/v1/orders", new CreateOrder
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
            }, null);

            Assert.Equal("0", bizContent.ReturnCode);
            Assert.Equal("OK", bizContent.ReturnMsg);
            Assert.Equal("1", bizContent.Status);
            Assert.Equal("12.3", bizContent.PayAmount);
        }

        [Fact]
        public async Task ShouldPostOneError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{""return_code"":""400011"",""return_msg"":""参数非法""}"
                },
                { "sign", "OZDddIl1y7obVILaDDDpLBQYvioJmAtNPbzpUbqpsJcmkc9A0bilmgM3RdmDcDrPsXXAkrNj3G/QGYrglLhQKaZZIliyOli2jK1o0vabX2NXsmNhKEWtyI216WeahiHywJuB1OKysDbuGZU6FNayy1LocOjf6R7nWKC4K6ICiBU="}
            }, 400);

            IHttpClient client = new FakeHttpClient("http://localhost:5000", "abcd1234");
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                try
                {
                    var bizContent = await client.PostJsonAsync<CreateOrder, CreateOrderResult>("/api/v1/orders", new CreateOrder
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
                    }, null);
                }
                catch (Exception ex)
                {
                    Assert.Equal("参数非法", ex.Message);
                    throw;
                }
            });
        }

        [Fact]
        public async Task ShouldPutOneError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{""return_code"":""400011"",""return_msg"":""参数非法""}"
                },
                { "sign", "OZDddIl1y7obVILaDDDpLBQYvioJmAtNPbzpUbqpsJcmkc9A0bilmgM3RdmDcDrPsXXAkrNj3G/QGYrglLhQKaZZIliyOli2jK1o0vabX2NXsmNhKEWtyI216WeahiHywJuB1OKysDbuGZU6FNayy1LocOjf6R7nWKC4K6ICiBU="}
            }, 400);

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "abcd1234"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICXAIBAAKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQABAoGABmzMr2oiJyvk0EZ0bAzXVvWTAZNA2LNMOsxFJZRG3J7q59IotIhyZFssgsC3Fx97wQ3cHF4B3uBKUIRAZ3ZcRb7o3Y0QpXnVGTv0EYVfIQxQPtkbUBWjdd+39Byg4HUMxgmxDJW+6UfkzDzSIkOlEqWFzGUf88kje8cQkEZxfYECQQC8ryMb8KpPmnUkeFy1RkzeHnAEqL94K9dKoPU5z0c+LN22eMs/cwFqpsDeC5E/6K74pcp9/JN15H6GTXmTIEKZAkEAszQHXVDvwMGhbdkIDxB7tiKY3F5fn9pUKdDN8zC5/fTRQI9mmJZtXZSN2JwQHuzzdlgrqzmLXKkHm1wLYjfxNQJBALR9iEU644AATZxUcsKI/BDh5t/eGEI5FdnyvIHPUOQeAPyC8lHAjpNZ7la5k/khePariVcZHGoC6DFKvUhK7MkCQB04OJMwpUcqy2Wb48KBD4rtLTuRb8oi0WJYF5y1rz4Hcy8xsqrirEZ7+Hz/RWmlfTUov0YWBfE/5mugoIKNWJkCQB5Z7wjHoroBO4VekuZQP4AbzDETDBpgNoRx8wWbQFAgIwCrI80Z8UHS4qbfuV7BWLxmUyBp5w8a21+rbFjkCwQ=",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                try
                {
                    var result = await client.PutJsonAsync<TerminateOrder, TernimateOrderResult>(
                        "/api/v1/orders", new TerminateOrder
                        {
                            Code = "N20211101003"
                        });
                }
                catch (Exception ex)
                {
                    Assert.Equal("参数非法", ex.Message);
                    throw;
                }
            });
        }

        [Fact]
        public async Task ShouldDeleteOneObject()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{""return_code"": ""0"",""return_msg"": ""OK"",""status"": ""1""}"
                },
                { "sign", "fjdBeoZGSjNNYTFtfKNQEi7dzkQO1mEwpqSzR70M+Gxc6OU9aQSWHElv2yIQi1gJIWKetrKXKdR+3qMyG63BiXfJDzQDh8exYXbpRgGpfnqcasfqhOh7RbfT0WmngYMkRJyZZrrGnOrcsmjBaxSe38QqoNcL6vtDrm8ZhkR+xk8="}
            }, 200);

            ISignatureProvider provider = new RSACryptoProvider(
                RSAKeyFactory.ImportPrivateKeyFromBase64String(
                    "MIICXAIBAAKBgQCI8IXsewY1dwJQi1iwKqA7g+5pOXqJZGww8ALncfCMcPCWa1mWnus0X8KNXWle7Fub0Xp0AGXPBh4G4UPImnC4hRTIxNMtGZemE+muesR/Ct2OSfBJB7g6Amfe5wb1r2uviLRPax5duQ72GdDA0GziW/HdL8C5zKAjkBa725jOfwIDAQABAoGAMlAk9IkS8+vg5tT75eYTbyp/GxwqQHasJaLZfk3nssIAM1QH24ZSJrEUWzo5781prytdEWfgABtgRujXLcpIpYbblBYe0a5JBZ+bPSVnWvVx2fd53URAeVA5SLpCYLvKc7+EWcrF4cRhjlJnv21kXJkzOdljEWiRBVKet2Kd5GECQQDjPPbQuwL878Sh9rV4yM3kwBOpAtPV3m/79gg293tbhLZSjs9Y7O6QuEDUysVxZAacGfN8X5fLrtYxzB5WV6MvAkEAmkWtguZmVtiYju64/43HJd9ldR3CnQKWZqeq04mHjT9cBcguzR9i8xo9ASvajL4N1wBFK0jHIMemBN45sjv1sQJAMn2/Uc3b8hvMkzhgRkBID4XmWG16aFEOjOu0E5KV5FXutRVqWRX8REfMj/umN3XESjcx9PbAoc9tZI84RnMenQJAdqlLmdjwNLKrPZ2fTGnYGWhomJnIfI5l8xXaTpWFXv6yIHxmiz4uFQFElScPFpyE9q/thCqZh7wpFat8w3xdMQJBAMta5+ydbQTNU8scTf1MC16ZYrX++4WhNm7aVpqPN2LV3yAWLISF4ERciujNjsbIOQrZepxizVpn2R45+rc69H8=",
                    RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                RSAKeyFactory.ImportPublicKeyFromBase64String(
                    "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCI8IXsewY1dwJQi1iwKqA7g+5pOXqJZGww8ALncfCMcPCWa1mWnus0X8KNXWle7Fub0Xp0AGXPBh4G4UPImnC4hRTIxNMtGZemE+muesR/Ct2OSfBJB7g6Amfe5wb1r2uviLRPax5duQ72GdDA0GziW/HdL8C5zKAjkBa725jOfwIDAQAB",
                    RSACryptoProvider.PublicKeyFormat.X509),
                    RSASignaturePadding.Pkcs1,
                    RSACryptoProvider.KeySizes.RSA1024);

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "abcd1234"))
                .WithSigner(provider)
                .Build();
            var bizContent = await client.DeleteAsync<RemoveOrder, RemoveOrderResult>(
                "/api/v1/orders", new RemoveOrder
                {
                    Code = "2021021501",
                }, null);
            Assert.Equal("0", bizContent.ReturnCode);
            Assert.Equal("OK", bizContent.ReturnMsg);
            Assert.Equal("1", bizContent.Status);
        }

        [Fact]
        public async Task ShouldDeleteOneError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{""return_code"":""400011"",""return_msg"":""参数非法""}"
                },
                { "sign", "Zmk+klO+1AF/ODOf7CfbwIaYznFtA5KDu3NTLk74YldTZf07AXqYZtpvK6NxKDah0LtxXXnsJSiEPqDn7fsDCYacJsCUdHPS+jlsnuYUNtqQnSWkr1y0LdyEpLgNCakuAM4mT4uVV6RxodcMU1niPdBw5ypIN6pz1mLzzDEPPWI="}
            }, 500);

            ISignatureProvider provider = new RSACryptoProvider(
                RSAKeyFactory.ImportPrivateKeyFromBase64String(
                    "MIICXAIBAAKBgQCI8IXsewY1dwJQi1iwKqA7g+5pOXqJZGww8ALncfCMcPCWa1mWnus0X8KNXWle7Fub0Xp0AGXPBh4G4UPImnC4hRTIxNMtGZemE+muesR/Ct2OSfBJB7g6Amfe5wb1r2uviLRPax5duQ72GdDA0GziW/HdL8C5zKAjkBa725jOfwIDAQABAoGAMlAk9IkS8+vg5tT75eYTbyp/GxwqQHasJaLZfk3nssIAM1QH24ZSJrEUWzo5781prytdEWfgABtgRujXLcpIpYbblBYe0a5JBZ+bPSVnWvVx2fd53URAeVA5SLpCYLvKc7+EWcrF4cRhjlJnv21kXJkzOdljEWiRBVKet2Kd5GECQQDjPPbQuwL878Sh9rV4yM3kwBOpAtPV3m/79gg293tbhLZSjs9Y7O6QuEDUysVxZAacGfN8X5fLrtYxzB5WV6MvAkEAmkWtguZmVtiYju64/43HJd9ldR3CnQKWZqeq04mHjT9cBcguzR9i8xo9ASvajL4N1wBFK0jHIMemBN45sjv1sQJAMn2/Uc3b8hvMkzhgRkBID4XmWG16aFEOjOu0E5KV5FXutRVqWRX8REfMj/umN3XESjcx9PbAoc9tZI84RnMenQJAdqlLmdjwNLKrPZ2fTGnYGWhomJnIfI5l8xXaTpWFXv6yIHxmiz4uFQFElScPFpyE9q/thCqZh7wpFat8w3xdMQJBAMta5+ydbQTNU8scTf1MC16ZYrX++4WhNm7aVpqPN2LV3yAWLISF4ERciujNjsbIOQrZepxizVpn2R45+rc69H8=",
                    RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                RSAKeyFactory.ImportPublicKeyFromBase64String(
                    "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCI8IXsewY1dwJQi1iwKqA7g+5pOXqJZGww8ALncfCMcPCWa1mWnus0X8KNXWle7Fub0Xp0AGXPBh4G4UPImnC4hRTIxNMtGZemE+muesR/Ct2OSfBJB7g6Amfe5wb1r2uviLRPax5duQ72GdDA0GziW/HdL8C5zKAjkBa725jOfwIDAQAB",
                    RSACryptoProvider.PublicKeyFormat.X509),
                    RSASignaturePadding.Pkcs1,
                    RSACryptoProvider.KeySizes.RSA1024);

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "abcd1234"))
                .WithSigner(provider)
                .Build();

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                try
                {
                    var bizContent = await client.DeleteAsync<RemoveOrder, RemoveOrderResult>(
                        "/api/v1/orders", new RemoveOrder
                        {
                            Code = "2021021501",
                        }, null);
                }
                catch (Exception ex)
                {
                    Assert.Equal("参数非法", ex.Message);
                    throw;
                }
            });
        }

        [Fact]
        public async Task ShouldVerifySuccessfully()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{""trxTimestamp"":null,""trxTime"":null,""return_msg"":"""",""msg_id"":""9925"",""return_code"":0,""trxDate"":null,""corp_serno"":""UpImage16329716536653510""}"
                },
                { "sign", "Wt4cGwArwmzt2LCODPnoiEGQQtXBOrZVASG7H0Dyqa7qjoC9KE+1ejSMLKvQdx1abxyXJHcHm0/oOEL2g4+wmVeUKVg8xQveGCVDkDbsHKvlEbYH928oXgS2ILCeYafMq53wGT3c2JlTABEKU164fL98skWjdmIi7lweOLw9gQY="}
            });

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "abcd1234"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICXAIBAAKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQABAoGABmzMr2oiJyvk0EZ0bAzXVvWTAZNA2LNMOsxFJZRG3J7q59IotIhyZFssgsC3Fx97wQ3cHF4B3uBKUIRAZ3ZcRb7o3Y0QpXnVGTv0EYVfIQxQPtkbUBWjdd+39Byg4HUMxgmxDJW+6UfkzDzSIkOlEqWFzGUf88kje8cQkEZxfYECQQC8ryMb8KpPmnUkeFy1RkzeHnAEqL94K9dKoPU5z0c+LN22eMs/cwFqpsDeC5E/6K74pcp9/JN15H6GTXmTIEKZAkEAszQHXVDvwMGhbdkIDxB7tiKY3F5fn9pUKdDN8zC5/fTRQI9mmJZtXZSN2JwQHuzzdlgrqzmLXKkHm1wLYjfxNQJBALR9iEU644AATZxUcsKI/BDh5t/eGEI5FdnyvIHPUOQeAPyC8lHAjpNZ7la5k/khePariVcZHGoC6DFKvUhK7MkCQB04OJMwpUcqy2Wb48KBD4rtLTuRb8oi0WJYF5y1rz4Hcy8xsqrirEZ7+Hz/RWmlfTUov0YWBfE/5mugoIKNWJkCQB5Z7wjHoroBO4VekuZQP4AbzDETDBpgNoRx8wWbQFAgIwCrI80Z8UHS4qbfuV7BWLxmUyBp5w8a21+rbFjkCwQ=",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();

            var result = await client.PutJsonAsync<TerminateOrder, TernimateOrderResult>(
                "/api/v1/orders", new TerminateOrder
                {
                    Code = "N20211101003"
                });
            Assert.NotNull(result);
            Assert.Null(result.Timestamp);
            Assert.Equal("9925", result.MsgId);
            Assert.Equal("UpImage16329716536653510", result.SeqNo);
        }

        [Fact]
        public async Task ShouldFailToVerify()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{""trxTimestamp"":null,""trxTime"":null,""return_msg"":"""",""return_code"":0,""trxDate"":null,""corp_serno"":""UpImage16329716536653510""}"
                },
                { "sign", "Wt4cGwArwmzt2LCODPnoiEGQQtXBOrZVASG7H0Dyqa7qjoC9KE+1ejSMLKvQdx1abxyXJHcHm0/oOEL2g4+wmVeUKVg8xQveGCVDkDbsHKvlEbYH928oXgS2ILCeYafMq53wGT3c2JlTABEKU164fL98skWjdmIi7lweOLw9gQY="}
            });

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "abcd1234"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICXAIBAAKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQABAoGABmzMr2oiJyvk0EZ0bAzXVvWTAZNA2LNMOsxFJZRG3J7q59IotIhyZFssgsC3Fx97wQ3cHF4B3uBKUIRAZ3ZcRb7o3Y0QpXnVGTv0EYVfIQxQPtkbUBWjdd+39Byg4HUMxgmxDJW+6UfkzDzSIkOlEqWFzGUf88kje8cQkEZxfYECQQC8ryMb8KpPmnUkeFy1RkzeHnAEqL94K9dKoPU5z0c+LN22eMs/cwFqpsDeC5E/6K74pcp9/JN15H6GTXmTIEKZAkEAszQHXVDvwMGhbdkIDxB7tiKY3F5fn9pUKdDN8zC5/fTRQI9mmJZtXZSN2JwQHuzzdlgrqzmLXKkHm1wLYjfxNQJBALR9iEU644AATZxUcsKI/BDh5t/eGEI5FdnyvIHPUOQeAPyC8lHAjpNZ7la5k/khePariVcZHGoC6DFKvUhK7MkCQB04OJMwpUcqy2Wb48KBD4rtLTuRb8oi0WJYF5y1rz4Hcy8xsqrirEZ7+Hz/RWmlfTUov0YWBfE/5mugoIKNWJkCQB5Z7wjHoroBO4VekuZQP4AbzDETDBpgNoRx8wWbQFAgIwCrI80Z8UHS4qbfuV7BWLxmUyBp5w8a21+rbFjkCwQ=",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();

            await Assert.ThrowsAsync<InvalidSignatureException>(async () =>
            {
                var result = await client.PutJsonAsync<TerminateOrder, TernimateOrderResult>(
                    "/api/v1/orders", new TerminateOrder
                    {
                        Code = "N20211101003"
                    });
            });
        }

        [Fact]
        public async Task ShouldFailToVerifyWithError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new Dictionary<string, object>
            {
                {
                    "response_biz_content", @"{""return_code"":""400011"",""return_msg"":""参数非法 ""}"
                },
                { "sign", "OZDddIl1y7obVILaDDDpLBQYvioJmAtNPbzpUbqpsJcmkc9A0bilmgM3RdmDcDrPsXXAkrNj3G/QGYrglLhQKaZZIliyOli2jK1o0vabX2NXsmNhKEWtyI216WeahiHywJuB1OKysDbuGZU6FNayy1LocOjf6R7nWKC4K6ICiBU="}
            }, 400);

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "abcd1234"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICXAIBAAKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQABAoGABmzMr2oiJyvk0EZ0bAzXVvWTAZNA2LNMOsxFJZRG3J7q59IotIhyZFssgsC3Fx97wQ3cHF4B3uBKUIRAZ3ZcRb7o3Y0QpXnVGTv0EYVfIQxQPtkbUBWjdd+39Byg4HUMxgmxDJW+6UfkzDzSIkOlEqWFzGUf88kje8cQkEZxfYECQQC8ryMb8KpPmnUkeFy1RkzeHnAEqL94K9dKoPU5z0c+LN22eMs/cwFqpsDeC5E/6K74pcp9/JN15H6GTXmTIEKZAkEAszQHXVDvwMGhbdkIDxB7tiKY3F5fn9pUKdDN8zC5/fTRQI9mmJZtXZSN2JwQHuzzdlgrqzmLXKkHm1wLYjfxNQJBALR9iEU644AATZxUcsKI/BDh5t/eGEI5FdnyvIHPUOQeAPyC8lHAjpNZ7la5k/khePariVcZHGoC6DFKvUhK7MkCQB04OJMwpUcqy2Wb48KBD4rtLTuRb8oi0WJYF5y1rz4Hcy8xsqrirEZ7+Hz/RWmlfTUov0YWBfE/5mugoIKNWJkCQB5Z7wjHoroBO4VekuZQP4AbzDETDBpgNoRx8wWbQFAgIwCrI80Z8UHS4qbfuV7BWLxmUyBp5w8a21+rbFjkCwQ=",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            await Assert.ThrowsAsync<InvalidSignatureException>(async () =>
            {
                await client.PutJsonAsync<TerminateOrder, TernimateOrderResult>(
                    "/api/v1/orders", new TerminateOrder
                    {
                        Code = "N20211101003"
                    });
            });
        }

        // [Fact]
        // public async Task ShouldBeEmptyRequestBody()
        // {
        //     using var httpTest = new HttpTest();
        //     httpTest.RespondWith("hello");

        //     var client = new HttpClientBuilder<FakeHttpClient>(
        //         new FakeHttpClient("http://localhost:5000", string.Empty))
        //         .WithRSASigner(
        //             new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
        //                 "MIICXAIBAAKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQABAoGABmzMr2oiJyvk0EZ0bAzXVvWTAZNA2LNMOsxFJZRG3J7q59IotIhyZFssgsC3Fx97wQ3cHF4B3uBKUIRAZ3ZcRb7o3Y0QpXnVGTv0EYVfIQxQPtkbUBWjdd+39Byg4HUMxgmxDJW+6UfkzDzSIkOlEqWFzGUf88kje8cQkEZxfYECQQC8ryMb8KpPmnUkeFy1RkzeHnAEqL94K9dKoPU5z0c+LN22eMs/cwFqpsDeC5E/6K74pcp9/JN15H6GTXmTIEKZAkEAszQHXVDvwMGhbdkIDxB7tiKY3F5fn9pUKdDN8zC5/fTRQI9mmJZtXZSN2JwQHuzzdlgrqzmLXKkHm1wLYjfxNQJBALR9iEU644AATZxUcsKI/BDh5t/eGEI5FdnyvIHPUOQeAPyC8lHAjpNZ7la5k/khePariVcZHGoC6DFKvUhK7MkCQB04OJMwpUcqy2Wb48KBD4rtLTuRb8oi0WJYF5y1rz4Hcy8xsqrirEZ7+Hz/RWmlfTUov0YWBfE/5mugoIKNWJkCQB5Z7wjHoroBO4VekuZQP4AbzDETDBpgNoRx8wWbQFAgIwCrI80Z8UHS4qbfuV7BWLxmUyBp5w8a21+rbFjkCwQ=",
        //                 RSACryptoProvider.PrivateKeyFormat.Pkcs1),
        //             new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
        //                 "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQAB",
        //                 RSACryptoProvider.PublicKeyFormat.X509)
        //         )
        //         .Build();
        //     await Assert.ThrowsAsync<MissingSignatureException>(async () =>
        //     {
        //         var result = await client.PutJsonAsync<int, string>(
        //             "/api/v1/orders", 123);
        //     });
        // }

        [Fact]
        public async Task ShouldBeEmptyResponseBody()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWith(string.Empty);

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "helloworld"))
                .WithRSASigner(
                    new Tuple<string, RSACryptoProvider.PrivateKeyFormat>(
                        "MIICXAIBAAKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQABAoGABmzMr2oiJyvk0EZ0bAzXVvWTAZNA2LNMOsxFJZRG3J7q59IotIhyZFssgsC3Fx97wQ3cHF4B3uBKUIRAZ3ZcRb7o3Y0QpXnVGTv0EYVfIQxQPtkbUBWjdd+39Byg4HUMxgmxDJW+6UfkzDzSIkOlEqWFzGUf88kje8cQkEZxfYECQQC8ryMb8KpPmnUkeFy1RkzeHnAEqL94K9dKoPU5z0c+LN22eMs/cwFqpsDeC5E/6K74pcp9/JN15H6GTXmTIEKZAkEAszQHXVDvwMGhbdkIDxB7tiKY3F5fn9pUKdDN8zC5/fTRQI9mmJZtXZSN2JwQHuzzdlgrqzmLXKkHm1wLYjfxNQJBALR9iEU644AATZxUcsKI/BDh5t/eGEI5FdnyvIHPUOQeAPyC8lHAjpNZ7la5k/khePariVcZHGoC6DFKvUhK7MkCQB04OJMwpUcqy2Wb48KBD4rtLTuRb8oi0WJYF5y1rz4Hcy8xsqrirEZ7+Hz/RWmlfTUov0YWBfE/5mugoIKNWJkCQB5Z7wjHoroBO4VekuZQP4AbzDETDBpgNoRx8wWbQFAgIwCrI80Z8UHS4qbfuV7BWLxmUyBp5w8a21+rbFjkCwQ=",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    new Tuple<string, RSACryptoProvider.PublicKeyFormat>(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            await Assert.ThrowsAsync<MissingSignatureException>(async () =>
            {
                var result = await client.PutJsonAsync<TerminateOrder, TernimateOrderResult>(
                    "/api/v1/orders", new TerminateOrder
                    {
                        Code = "N20211101003"
                    });
            });
        }

        [Fact]
        public async Task ShouldBeEmptyResponseError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWith(string.Empty, 500);

            var client = new HttpClientBuilder(
                new FakeHttpClient("http://localhost:5000", "helloworld"))
                .WithRSASigner(
                    RSAKeyFactory.ImportPrivateKeyFromBase64String(
                        "MIICXAIBAAKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQABAoGABmzMr2oiJyvk0EZ0bAzXVvWTAZNA2LNMOsxFJZRG3J7q59IotIhyZFssgsC3Fx97wQ3cHF4B3uBKUIRAZ3ZcRb7o3Y0QpXnVGTv0EYVfIQxQPtkbUBWjdd+39Byg4HUMxgmxDJW+6UfkzDzSIkOlEqWFzGUf88kje8cQkEZxfYECQQC8ryMb8KpPmnUkeFy1RkzeHnAEqL94K9dKoPU5z0c+LN22eMs/cwFqpsDeC5E/6K74pcp9/JN15H6GTXmTIEKZAkEAszQHXVDvwMGhbdkIDxB7tiKY3F5fn9pUKdDN8zC5/fTRQI9mmJZtXZSN2JwQHuzzdlgrqzmLXKkHm1wLYjfxNQJBALR9iEU644AATZxUcsKI/BDh5t/eGEI5FdnyvIHPUOQeAPyC8lHAjpNZ7la5k/khePariVcZHGoC6DFKvUhK7MkCQB04OJMwpUcqy2Wb48KBD4rtLTuRb8oi0WJYF5y1rz4Hcy8xsqrirEZ7+Hz/RWmlfTUov0YWBfE/5mugoIKNWJkCQB5Z7wjHoroBO4VekuZQP4AbzDETDBpgNoRx8wWbQFAgIwCrI80Z8UHS4qbfuV7BWLxmUyBp5w8a21+rbFjkCwQ=",
                        RSACryptoProvider.PrivateKeyFormat.Pkcs1),
                    RSAKeyFactory.ImportPublicKeyFromBase64String(
                        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCEFM6NPDDtiNT43+9/nqBD2PiLQ1OCa62eZNqRTHtqI+/Wpm0awKLqSKlt80zBQCaWdAxidWtabpcH6jtmXXMBXxK9vYcvn4ChapfbFfomU3l/1VEYGo6ke6hagoUSF5zz5pPHv+fzBb+nfvSlXiOMzNHKEcqXCmI8I0ustT7SrQIDAQAB",
                        RSACryptoProvider.PublicKeyFormat.X509)
                )
                .Build();
            await Assert.ThrowsAsync<MissingSignatureException>(async () =>
            {
                var result = await client.PutJsonAsync<TerminateOrder, TernimateOrderResult>(
                    "/api/v1/orders", new TerminateOrder
                    {
                        Code = "N20211101003"
                    });
            });
        }
    }
}