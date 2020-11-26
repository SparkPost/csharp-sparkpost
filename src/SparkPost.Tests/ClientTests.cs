using System;
using System.Net.Http;
using Xunit;

namespace SparkPost.Tests
{
    public partial class ClientTests
    {
        public class HttpClientOverridingTests
        {
            private readonly Client client;

            public HttpClientOverridingTests()
            {
                client = new Client(null);
            }

            [Fact]
            public void By_default_it_should_return_new_http_clients_each_time()
            {
                var first = client.CustomSettings.CreateANewHttpClient();
                var second = client.CustomSettings.CreateANewHttpClient();

                Assert.NotNull(first);
                Assert.NotNull(second);
                Assert.NotEqual(first, second);
            }

            [Fact]
            public void It_should_allow_the_overriding_of_the_http_client_building()
            {
                var httpClient = new HttpClient();

                client.CustomSettings.BuildHttpClientsUsing(() => httpClient);

                Assert.Equal(httpClient, client.CustomSettings.CreateANewHttpClient());
                Assert.Equal(httpClient, client.CustomSettings.CreateANewHttpClient());
                Assert.Equal(httpClient, client.CustomSettings.CreateANewHttpClient());
                Assert.Equal(httpClient, client.CustomSettings.CreateANewHttpClient());
            }

            [Fact]
            public void It_should_default_to_async()
            {
                Assert.Equal(SendingModes.Async, client.CustomSettings.SendingMode);
            }

            [Fact]
            public void it_should_have_inbound_domains()
            {
                Assert.NotNull(client.InboundDomains);
            }

            [Fact]
            public void It_should_set_any_subaccount_id_passed_to_it()
            {
                Assert.Equal(1234, new Client(Guid.NewGuid().ToString(), 1234).SubaccountId);
            }
        }
    }
}