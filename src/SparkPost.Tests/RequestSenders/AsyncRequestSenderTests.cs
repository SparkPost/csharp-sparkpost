using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using SparkPost.RequestSenders;
using Xunit;

namespace SparkPost.Tests.RequestSenders
{
    public class AsyncRequestSenderTests
    {
        public class SendTests
        {
            private readonly Request request;
            private readonly string apiHost;
            private readonly string apiKey;
            private readonly HttpResponseMessage defaultHttpResponseMessage;
            private readonly SendTests.AsyncTesting asyncTesting;
            private readonly Mock<IClient> client;

            public SendTests()
            { 
                var httpClient = new HttpClient();
                apiHost = "http://test.com";
                apiKey = Guid.NewGuid().ToString();

                var settings = new Client.Settings();
                settings.BuildHttpClientsUsing(() => httpClient);
                client = new Mock<IClient>();
                client.Setup(x => x.CustomSettings).Returns(settings);
                client.Setup(x => x.ApiHost).Returns(apiHost);
                client.Setup(x => x.ApiKey).Returns(apiKey);

                asyncTesting = new SendTests.AsyncTesting(client.Object);

                request = new Request();

                defaultHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted)
                {
                    Content = new StringContent(Guid.NewGuid().ToString())
                };
            }


            [Fact]
            public async void It_should_return_the_http_response_message_info()
            {
                var content = Guid.NewGuid().ToString();
                asyncTesting.SetupTheResponseWith((r, h) => new HttpResponseMessage(HttpStatusCode.Accepted)
                {
                    Content = new StringContent(content)
                });

                var result = await asyncTesting.Send(request);
                Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
                Assert.Equal(content, result.Content);
            }

            [Fact]
            public async Task It_should_return_the_http_response_message_info_take_2()
            {
                var content = Guid.NewGuid().ToString();
                asyncTesting.SetupTheResponseWith((r, h) => new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(content)
                });

                var result = await asyncTesting.Send(request);
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
                Assert.Equal("Not Found", result.ReasonPhrase);
                Assert.Equal(content, result.Content);
            }

            [Fact]
            public async Task It_should_pass_the_api_key()
            {
                asyncTesting.SetupTheResponseWith((r, h) =>
                {
                    Assert.Equal(apiKey, h.DefaultRequestHeaders.Authorization.ToString());
                    return defaultHttpResponseMessage;
                });

                await asyncTesting.Send(request);
            }

            [Fact]
            public async Task It_should_send_the_request_to_the_appropriate_host()
            {
                asyncTesting.SetupTheResponseWith((r, h) =>
                {
                    Assert.Equal(apiHost + "/", h.BaseAddress.ToString());
                    return defaultHttpResponseMessage;
                });

                await asyncTesting.Send(request);
            }

            [Fact]
            public async Task It_should_set_the_subaccount_when_the_subaccount_is_not_zero()
            {
                client.Setup(x => x.SubaccountId).Returns(345);
                asyncTesting.SetupTheResponseWith((r, h) =>
                {
                    var match = h.DefaultRequestHeaders.First(x => x.Key == "X-MSYS-SUBACCOUNT");
                    Assert.Single(match.Value);
                    Assert.Equal("345", match.Value.First());
                    return defaultHttpResponseMessage;
                });

                await asyncTesting.Send(request);
            }

            [Fact]
            public async Task It_should_NOT_set_a_subaccount_when_the_subaccount_is_zero()
            {
                client.Setup(x => x.SubaccountId).Returns(0);
                asyncTesting.SetupTheResponseWith((r, h) =>
                {
                    var count = h.DefaultRequestHeaders.Count(x => x.Key == "X-MSYS-SUBACCOUNT");
                    Assert.Equal(0, count);
                    return defaultHttpResponseMessage;
                });

                await asyncTesting.Send(request);
            }

            public class AsyncTesting : AsyncRequestSender
            {
                private Func<Request, HttpClient, HttpResponseMessage> responseBuilder;

                public AsyncTesting(IClient client) : base(client, null)
                {
                }

                public void SetupTheResponseWith(Func<Request, HttpClient, HttpResponseMessage> responseBuilder)
                {
                    this.responseBuilder = responseBuilder;
                }

                protected override Task<HttpResponseMessage> GetTheResponse(Request request, HttpClient httpClient)
                {
                    return Task.FromResult(responseBuilder(request, httpClient));
                }
            }
        }
    }
}