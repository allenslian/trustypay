using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using TrustyPay.Core.Cryptography.Http.Service;
using Microsoft.AspNetCore.Http.Features;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.IO;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Service
{
    public class MiddlewareFixture
    {
        [Fact]
        public async Task TestNormalRequestWith200()
        {
            var httpContext = GetFakeHttpContext();
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);
            Assert.Equal(200, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task TestMissingAppIdAndApiKeyError()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/json"));
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query =
                new QueryCollection(new Dictionary<string, StringValues>(){
                    {"filter", new StringValues("hello world")}
                });
            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(400, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":400,\\\"message\\\":\\\"缺少app id和api key信息!\\\"}\"", response);
        }

        [Fact]
        public async Task TestInvalidContentTypeError()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "\"ok\"".FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/javascript"));
            httpContext.Request.Method = "POST";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"test", new StringValues("123")}
            });
            httpContext.Request.Body = new MemoryStream();
            await httpContext.Request.Body.WriteAsync(
                "{\r\n  \"appId\":\"trustypay.client\",\r\n  \"apiKey\": \"icx3wd0js06q5mwbdf1o991s964jve886zjs\",\r\n  \"bizContent\": \"{\\\"name\\\":\\\"allen\\\"}\",\r\n  \"timestamp\": \"1641008396\",\r\n \"sign\": \"OmtcOVQn06Zz6dHh6vmplSbdeIC3i1Ekh6NmkJPta1DEDwqnOCAHR1Ebndd2e+lK25jd8Erlmcp0iZCmhy/3tFuZ0jmVdQyaPOiJtqYklWZxGsqdJk3D2ZixuK+b+kzaqms6+88dEQoplBMlNmcmhnRP2hF+2LLeTILNg78zN5ezWJjSbephzdWJsnh3abGeZnPOTGCPp29qkG92Mys0lzz8Zu+lg+D8alJkK3yFEjaaEw1FZNO7WZSrIHwJ659RdMtq0O2tYzyFOdYF6PHT/jY+giwVc40hfKKg6r0PmFa0YZVhpeA09ikgvkqAe0eSueUeHoptc6ayPXnvnoMeyA==\"\r\n}".FromUTF8String());

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(400, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":400,\\\"message\\\":\\\"The media type[application/javascript] is invalid!!!,目前只支持application/json和application/x-www-form-urlencoded!\\\"}", response);
        }

        [Fact]
        public async Task TestGetRequestIsOk()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "[\r\n  {\r\n \"name\": \"allen\",\r\n  \"no\": \"2021111\"}\r\n]"
                    .FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/javascript"));
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query =
                new QueryCollection(new Dictionary<string, StringValues>(){
                    {"appId", new StringValues("trustypay.client")},
                    {"apiKey", new StringValues("icx3wd0js06q5mwbdf1o991s964jve886zjs")},
                    {"bizContent", new StringValues("{\"fliter\":\"hello world\"}")},
                    {"timestamp", new StringValues("1641008396")},
                    {"charset", new StringValues("utf-8")},
                    {"signType", new StringValues("RS256")},
                    {"sign", new StringValues("u6fvVnlkMT5eiS/qiW9eGwFf4nTIYOqxjxffm/9znQDXVlRcOG272L5qeF/kachZhsHiCe3iHPThuY/k6rryZEvdLTZ4CyBpijYeGuDEm8odVw4JdEt3Gi1jzem9t546h3mpesudC2k3I2R4UvySuUiisSPuGRQH4/8Y1pk1iwEozh/iMg4CGlz+SOmytkxMPuKNqF5l925OFxGFypUCPIFwh0uiEvWjqkiAWw6e6v0vqNPvbsd8ZnrT4k4NUyZ1Ui0NJ3PgwwAWy+TnGryO4RcBon59rZISv76m2NK2wAOfBKmU4Vk/HEZYzqjf4yBbp5MFBW8tmGYMI4AWuLMCQQ==")}
                });
            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(200, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"[{\\\"name\\\":\\\"allen\\\",\\\"no\\\":\\\"2021111\\\"}]\"", response);
        }

        [Fact]
        public async Task TestIncorrectSignature()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "[\r\n  {\r\n \"name\": \"allen\",\r\n  \"no\": \"2021111\"}\r\n]"
                    .FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/json"));
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query =
                new QueryCollection(new Dictionary<string, StringValues>(){
                    {"appId", new StringValues("trustypay.client")},
                    {"apiKey", new StringValues("icx3wd0js06q5mwbdf1o991s964jve886zjs")},
                    {"bizContent", new StringValues("{\"fliter\":\"hello world\"}")},
                    {"timestamp", new StringValues("1641008396")},
                    {"charset", new StringValues("utf-8")},
                    {"signType", new StringValues("RS256")},
                    {"sign", new StringValues("G5uSbdBBAMP0QI7XPLP3dICYKCwdzW7fqoEhxFX1bylvaLWQ6yLKVt/y2gVE3JCZQCN2WqqqlrLhkKFKJ2uhABkQvQfb4vSokfhxBpFkk0PfMzY5+hyk4wL/UFb4evJ2pj9A/7pajCjwDovvtOvIIGjoUOcG6ji1aBC6K/UGmsTP4SvW6AvZ7/+D4oAlmFKn2KDAoQr4SxhOrMT+RXmzvVumWRT6WZcdSYJOLkE4j3HglZRPGdUA8Una80rt54j6PGVjahMVYy+5RjDATVPHgF39mk6+7x/UhPgodvTtqbTvK6qljkoHTlfAy/6VUmmX3EJPFwcTFpK+5bkEHNs8RA==")}
                });
            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(401, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":401,\\\"message\\\":\\\"请求签名验证无效!\\\"}\"", response);
        }

        [Fact]
        public async Task TestIncorrectSignature2()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "[\r\n  {\r\n \"name\": \"allen\",\r\n  \"no\": \"2021111\"}\r\n]"
                    .FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/json"));
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query =
                new QueryCollection(new Dictionary<string, StringValues>(){
                    {"appId", new StringValues("trustypay.client")},
                    {"apiKey", new StringValues("icx3wd0js06q5mwbdf1o991s964jve886zjs")},
                    {"bizContent", new StringValues("{\"fliter\":\"hello world\"}")},
                    {"timestamp", new StringValues("1641008396")},
                    {"charset", new StringValues("utf-8")},
                    {"signType", new StringValues("RS256")},
                    {"sign", new StringValues("5uSbdBBAMP0QI7XPLP3dICYKCwdzW7fqoEhxFX1bylvaLWQ6yLKVt/y2gVE3JCZQCN2WqqqlrLhkKFKJ2uhABkQvQfb4vSokfhxBpFkk0PfMzY5+hyk4wL/UFb4evJ2pj9A/7pajCjwDovvtOvIIGjoUOcG6ji1aBC6K/UGmsTP4SvW6AvZ7/+D4oAlmFKn2KDAoQr4SxhOrMT+RXmzvVumWRT6WZcdSYJOLkE4j3HglZRPGdUA8Una80rt54j6PGVjahMVYy+5RjDATVPHgF39mk6+7x/UhPgodvTtqbTvK6qljkoHTlfAy/6VUmmX3EJPFwcTFpK+5bkEHNs8RA==")}
                });
            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(401, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":401,\\\"message\\\":\\\"请求签名验证无效, 请确认参数是否有效!\\\"}\"", response);
        }

        [Fact]
        public async Task TestPostRequestWithJsonIsOk()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "\"ok\"".FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/json; charset=utf-8"));
            httpContext.Request.Method = "POST";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"test", new StringValues("123")}
            });
            httpContext.Request.Body = new MemoryStream();
            await httpContext.Request.Body.WriteAsync(
                "{\r\n  \"appId\":\"trustypay.client\",\r\n  \"apiKey\": \"icx3wd0js06q5mwbdf1o991s964jve886zjs\",\r\n  \"bizContent\": \"{\\\"name\\\":\\\"allen\\\"}\",\r\n  \"timestamp\": \"1641008396\",\r\n \"sign\": \"OmtcOVQn06Zz6dHh6vmplSbdeIC3i1Ekh6NmkJPta1DEDwqnOCAHR1Ebndd2e+lK25jd8Erlmcp0iZCmhy/3tFuZ0jmVdQyaPOiJtqYklWZxGsqdJk3D2ZixuK+b+kzaqms6+88dEQoplBMlNmcmhnRP2hF+2LLeTILNg78zN5ezWJjSbephzdWJsnh3abGeZnPOTGCPp29qkG92Mys0lzz8Zu+lg+D8alJkK3yFEjaaEw1FZNO7WZSrIHwJ659RdMtq0O2tYzyFOdYF6PHT/jY+giwVc40hfKKg6r0PmFa0YZVhpeA09ikgvkqAe0eSueUeHoptc6ayPXnvnoMeyA==\"\r\n}".FromUTF8String());

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(200, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"ok\"", response);
        }

        [Fact]
        public async Task TestPostRequestWithFormIsOk()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "\"ok\"".FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/x-www-form-urlencoded; charset=utf-8"));
            httpContext.Request.Method = "POST";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"test", new StringValues("123")}
            });
            httpContext.Request.Body = new MemoryStream();
            await httpContext.Request.Body.WriteAsync(
                "appId=trustypay.client&apiKey=icx3wd0js06q5mwbdf1o991s964jve886zjs&bizContent={\"name\":\"allen\"}&timestamp=1641008396&sign=OmtcOVQn06Zz6dHh6vmplSbdeIC3i1Ekh6NmkJPta1DEDwqnOCAHR1Ebndd2e%2BlK25jd8Erlmcp0iZCmhy%2F3tFuZ0jmVdQyaPOiJtqYklWZxGsqdJk3D2ZixuK%2Bb%2Bkzaqms6%2B88dEQoplBMlNmcmhnRP2hF%2B2LLeTILNg78zN5ezWJjSbephzdWJsnh3abGeZnPOTGCPp29qkG92Mys0lzz8Zu%2Blg%2BD8alJkK3yFEjaaEw1FZNO7WZSrIHwJ659RdMtq0O2tYzyFOdYF6PHT%2FjY%2BgiwVc40hfKKg6r0PmFa0YZVhpeA09ikgvkqAe0eSueUeHoptc6ayPXnvnoMeyA%3D%3D&h&hello=null".FromUTF8String());

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(200, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"ok\"", response);
        }

        [Fact]
        public async Task TestMissingSignature()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "[\r\n  {\r\n \"name\": \"allen\",\r\n  \"no\": \"2021111\"}\r\n]"
                    .FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider();

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/javascript"));
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query =
                new QueryCollection(new Dictionary<string, StringValues>(){
                    {"appId", new StringValues("trustypay.client")},
                    {"apiKey", new StringValues("icx3wd0js06q5mwbdf1o991s964jve886zjs")},
                    {"bizContent", new StringValues("{\"fliter\":\"hello world\"}")},
                    {"timestamp", new StringValues("1641008396")},
                    {"signType", new StringValues("RS256")},
                });
            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(400, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":400,\\\"message\\\":\\\"缺少请求签名信息!\\\"}\"", response);
        }

        [Fact]
        public async Task TestIncorrectPublicKey()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "\"ok\"".FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider(
                true
            );

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/x-www-form-urlencoded; charset=utf-8"));
            httpContext.Request.Method = "POST";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"test", new StringValues("123")}
            });
            httpContext.Request.Body = new MemoryStream();
            await httpContext.Request.Body.WriteAsync(
                "appId=trustypay.client&apiKey=icx3wd0js06q5mwbdf1o991s964jve886zjs&bizContent={\"name\":\"allen\"}&timestamp=1641008396&sign=OmtcOVQn06Zz6dHh6vmplSbdeIC3i1Ekh6NmkJPta1DEDwqnOCAHR1Ebndd2e%2BlK25jd8Erlmcp0iZCmhy%2F3tFuZ0jmVdQyaPOiJtqYklWZxGsqdJk3D2ZixuK%2Bb%2Bkzaqms6%2B88dEQoplBMlNmcmhnRP2hF%2B2LLeTILNg78zN5ezWJjSbephzdWJsnh3abGeZnPOTGCPp29qkG92Mys0lzz8Zu%2Blg%2BD8alJkK3yFEjaaEw1FZNO7WZSrIHwJ659RdMtq0O2tYzyFOdYF6PHT%2FjY%2BgiwVc40hfKKg6r0PmFa0YZVhpeA09ikgvkqAe0eSueUeHoptc6ayPXnvnoMeyA%3D%3D&h&hello=null".FromUTF8String());

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(401, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":401,\\\"message\\\":\\\"应用方公钥无效!\\\"}\"", response);
        }

        [Fact]
        public async Task TestIncorrectPrivateKey()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "\"ok\"".FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider(
                false, true
            );

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/x-www-form-urlencoded; charset=utf-8"));
            httpContext.Request.Method = "POST";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"test", new StringValues("123")}
            });
            httpContext.Request.Body = new MemoryStream();
            await httpContext.Request.Body.WriteAsync(
                "appId=trustypay.client&apiKey=icx3wd0js06q5mwbdf1o991s964jve886zjs&bizContent={\"name\":\"allen\"}&timestamp=1641008396&sign=OmtcOVQn06Zz6dHh6vmplSbdeIC3i1Ekh6NmkJPta1DEDwqnOCAHR1Ebndd2e%2BlK25jd8Erlmcp0iZCmhy%2F3tFuZ0jmVdQyaPOiJtqYklWZxGsqdJk3D2ZixuK%2Bb%2Bkzaqms6%2B88dEQoplBMlNmcmhnRP2hF%2B2LLeTILNg78zN5ezWJjSbephzdWJsnh3abGeZnPOTGCPp29qkG92Mys0lzz8Zu%2Blg%2BD8alJkK3yFEjaaEw1FZNO7WZSrIHwJ659RdMtq0O2tYzyFOdYF6PHT%2FjY%2BgiwVc40hfKKg6r0PmFa0YZVhpeA09ikgvkqAe0eSueUeHoptc6ayPXnvnoMeyA%3D%3D&h&hello=null".FromUTF8String());

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(500, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task TestNoPermission()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "\"ok\"".FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider(
                false, false, false
            );

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/x-www-form-urlencoded; charset=utf-8"));
            httpContext.Request.Method = "POST";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"test", new StringValues("123")}
            });
            httpContext.Request.Body = new MemoryStream();
            await httpContext.Request.Body.WriteAsync(
                "appId=trustypay.client&apiKey=icx3wd0js06q5mwbdf1o991s964jve886zjs&bizContent={\"name\":\"allen\"}&timestamp=1641008396&sign=OmtcOVQn06Zz6dHh6vmplSbdeIC3i1Ekh6NmkJPta1DEDwqnOCAHR1Ebndd2e%2BlK25jd8Erlmcp0iZCmhy%2F3tFuZ0jmVdQyaPOiJtqYklWZxGsqdJk3D2ZixuK%2Bb%2Bkzaqms6%2B88dEQoplBMlNmcmhnRP2hF%2B2LLeTILNg78zN5ezWJjSbephzdWJsnh3abGeZnPOTGCPp29qkG92Mys0lzz8Zu%2Blg%2BD8alJkK3yFEjaaEw1FZNO7WZSrIHwJ659RdMtq0O2tYzyFOdYF6PHT%2FjY%2BgiwVc40hfKKg6r0PmFa0YZVhpeA09ikgvkqAe0eSueUeHoptc6ayPXnvnoMeyA%3D%3D&h&hello=null".FromUTF8String());

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(403, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":403,\\\"message\\\":\\\"应用权限不够!\\\"}\"", response);
        }

        [Fact]
        public async Task TestPermissionException()
        {
            RequestDelegate mockNext = (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Body.WriteAsync(
                    "\"ok\"".FromUTF8String());
                return Task.CompletedTask;
            };
            IResourceProvider mockProvider = new FakeResourceProvider(
                false, false, true, true
            );

            var httpContext = GetFakeHttpContextWithSignatureVerificationAttribute(mockNext);
            httpContext.Request.Headers.Add("Content-Type", new StringValues("application/x-www-form-urlencoded; charset=utf-8"));
            httpContext.Request.Method = "POST";
            httpContext.Request.Path = "/api/v1/employees";
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"test", new StringValues("123")}
            });
            httpContext.Request.Body = new MemoryStream();
            await httpContext.Request.Body.WriteAsync(
                "appId=trustypay.client&apiKey=icx3wd0js06q5mwbdf1o991s964jve886zjs&bizContent={\"name\":\"allen\"}&timestamp=1641008396&sign=OmtcOVQn06Zz6dHh6vmplSbdeIC3i1Ekh6NmkJPta1DEDwqnOCAHR1Ebndd2e%2BlK25jd8Erlmcp0iZCmhy%2F3tFuZ0jmVdQyaPOiJtqYklWZxGsqdJk3D2ZixuK%2Bb%2Bkzaqms6%2B88dEQoplBMlNmcmhnRP2hF%2B2LLeTILNg78zN5ezWJjSbephzdWJsnh3abGeZnPOTGCPp29qkG92Mys0lzz8Zu%2Blg%2BD8alJkK3yFEjaaEw1FZNO7WZSrIHwJ659RdMtq0O2tYzyFOdYF6PHT%2FjY%2BgiwVc40hfKKg6r0PmFa0YZVhpeA09ikgvkqAe0eSueUeHoptc6ayPXnvnoMeyA%3D%3D&h&hello=null".FromUTF8String());

            var mw = new SignatureMiddleware(
                mockNext,
                new SignatureOption(),
                mockProvider,
                GetLogger()
            );
            await mw.Invoke(httpContext);

            Assert.Equal(403, httpContext.Response.StatusCode);
            if (httpContext.Response.Body.CanSeek)
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            }
            var response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            Assert.Contains("\"result\":\"{\\\"code\\\":403,\\\"message\\\":\\\"权限验证失败!\\\"}\"", response);
        }


        private HttpContext GetFakeHttpContext()
        {
            var features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(new MemoryStream()));
            var httpContext = new DefaultHttpContext(features);
            return httpContext;
        }

        private HttpContext GetFakeHttpContextWithSignatureVerificationAttribute(RequestDelegate next)
        {
            var features = new FeatureCollection();
            features.Set<IEndpointFeature>(new FakeEndpointFeature(next, true));
            features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(new MemoryStream()));
            var httpContext = new DefaultHttpContext(features);
            return httpContext;
        }

        private ILogger<SignatureMiddleware> GetLogger()
        {
            return LoggerFactory.Create(b =>
            {
                b.AddConsole();
            })
            .CreateLogger<SignatureMiddleware>();
        }
    }
}