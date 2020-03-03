using System;
using System.Net.Http;
using AutoMoq.Helpers;
using NUnit.Framework;
using Should;

namespace SparkPost.Tests
{
    public class ClientTests
    {
        [TestFixture]
        public class HttpClientOverridingTests
        {
            [SetUp]
            public void Setup()
            {
                client = new Client(null);
            }

            private Client client;

            [Test]
            public void By_default_it_should_return_new_http_clients_each_time()
            {
                var first = client.CustomSettings.CreateANewHttpClient();
                var second = client.CustomSettings.CreateANewHttpClient();

                first.ShouldNotBeNull();
                second.ShouldNotBeNull();
                first.ShouldNotBeSameAs(second);
            }

            [Test]
            public void It_should_allow_the_overriding_of_the_http_client_building()
            {
                var httpClient = new HttpClient();

                client.CustomSettings.BuildHttpClientsUsing(() => httpClient);

                client.CustomSettings.CreateANewHttpClient().ShouldBeSameAs(httpClient);
                client.CustomSettings.CreateANewHttpClient().ShouldBeSameAs(httpClient);
                client.CustomSettings.CreateANewHttpClient().ShouldBeSameAs(httpClient);
                client.CustomSettings.CreateANewHttpClient().ShouldBeSameAs(httpClient);
            }

            [Test]
            public void It_should_default_to_async()
            {
                client.CustomSettings.SendingMode.ShouldEqual(SendingModes.Async);
            }

            [Test]
            public void it_should_have_inbound_domains()
            {
                client.InboundDomains.ShouldNotBeNull();
            }

            [Test]
            public void It_should_set_any_subaccount_id_passed_to_it()
            {
                (new Client(Guid.NewGuid().ToString(), 1234))
                    .SubaccountId.ShouldEqual(1234);
            }
        }

        [TestFixture]
        public class UserAgentTests : AutoMoqTestFixture<Client.Settings>
        {
            [SetUp]
            public void Setup()
            {
                ResetSubject();
            }

            [Test]
            public void It_should_default_to_the_library_version()
            {
                Subject.UserAgent.ShouldEqual($"csharp-sparkpost/1.15.0");
            }

            [Test]
            public void It_should_allow_the_user_agent_to_be_changed()
            {
                var userAgent = Guid.NewGuid().ToString();
                Subject.UserAgent = userAgent;
                Subject.UserAgent.ShouldEqual(userAgent);
            }
        }
    }
}