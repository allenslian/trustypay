using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Flurl.Http.Testing;
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures
{
    public class HttpClientFixture
    {
        [Fact]
        public async Task ShouldGetOneObject()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Dictionary<string, object>{
                    {
                        "response_biz_content", @"{
                            ""return_code"": ""0"", 
                            ""return_msg"": ""OK"",
                            ""msg_id"":""urcnl24ciutr9"",
                            ""sumPayamt"": ""1"",
                            ""orderCurr"": ""001"",
                            ""orderAmount"": ""1"",
                            ""agreeName"": ""1"",
                            ""serialNo"": ""1"",
                            ""agreeCode"": ""1"",
                            ""partnerSeq"": ""1"",
                            ""redirectParam"": ""1"",
                            ""payMode"": ""1"",
                            ""status"": ""1""
                            }"
                    },
                    { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
                });

                IHttpClient client = new FakeHttpClient("http://localhost:5000", "abcd1234");
                var bizContent = await client.GetAsync<RequestContent, ResponseContent>("/api/v1/greeting", new RequestContent
                {
                    OrderCode = "2021021501",
                    Remark = "备注"
                }, null);

                Assert.Equal("0", bizContent.ReturnCode);
                Assert.Equal("OK", bizContent.ReturnMsg);
                Assert.Equal("1", bizContent.Status);
            }
        }

        [Fact]
        public async Task ShouldPostOneObject()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Dictionary<string, object>{
                    {
                        "response_biz_content", @"{
                            ""return_code"": ""0"", 
                            ""return_msg"": ""OK"",
                            ""msg_id"":""urcnl24ciutr9"",
                            ""sumPayamt"": ""1"",
                            ""orderCurr"": ""001"",
                            ""orderAmount"": ""1"",
                            ""agreeName"": ""1"",
                            ""serialNo"": ""1"",
                            ""agreeCode"": ""1"",
                            ""partnerSeq"": ""1"",
                            ""redirectParam"": ""1"",
                            ""payMode"": ""1"",
                            ""status"": ""1""
                            }"
                    },
                    { "sign", "ERITJKEIJKJHKKKKKKKHJEREEEEEEEEEEE"}
                });

                IHttpClient client = new FakeHttpClient("http://localhost:5000", "abcd1234");
                var bizContent = await client.GetAsync<RequestContent, ResponseContent>("/api/v1/greeting", new RequestContent
                {
                    OrderCode = "2021021501",
                    Remark = "备注"
                }, null);

                Assert.Equal("0", bizContent.ReturnCode);
                Assert.Equal("OK", bizContent.ReturnMsg);
                Assert.Equal("1", bizContent.Status);
            }
        }
    }

    internal class ResponseContent
    {
        [JsonProperty("return_code")]
        public string ReturnCode { get; set; }

        [JsonProperty("return_msg")]
        public string ReturnMsg { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    internal class RequestContent
    {
        [JsonProperty("order_code")]
        public string OrderCode { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }
    }
}