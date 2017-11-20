using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml.XPath;
using AutoMoq.Helpers;
using Moq;
using NUnit.Framework;
using SparkPost.RequestSenders;

namespace SparkPost.Tests
{
    public class SuppressionTests
    {
        [TestFixture]
        public class DeleteTests : AutoMoqTestFixture<Suppressions>
        {
            private Response response;
            private string email;

            [SetUp]
            public void Setup()
            {
                ResetSubject();

                response = new Response {StatusCode = HttpStatusCode.NoContent};

                Mocked<IRequestSender>()
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Returns(Task.FromResult(response));

                email = Guid.NewGuid().ToString();
            }

            [Test]
            public async void It_should_return_true_if_the_web_request_returns_no_content()
            {
                var result = await Subject.Delete(email);
                result.ShouldBeTrue();
            }

            [Test]
            public async void It_should_return_false_if_the_web_request_returns_anything_but_no_content()
            {
                response.StatusCode = HttpStatusCode.Accepted;
                (await Subject.Delete(email)).ShouldBeFalse();

                response.StatusCode = HttpStatusCode.Ambiguous;
                (await Subject.Delete(email)).ShouldBeFalse();

                response.StatusCode = HttpStatusCode.UpgradeRequired;
                (await Subject.Delete(email)).ShouldBeFalse();
            }

            [Test]
            public async void It_should_build_the_web_request_parameters_correctly()
            {
                var version = Guid.NewGuid().ToString();

                Mocked<IClient>()
                    .Setup(x => x.Version)
                    .Returns(version);

                Mocked<IRequestSender>()
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Callback((Request r) =>
                    {
                        r.Url.ShouldEqual($"api/{version}/suppression-list/{email}");
                        r.Method.ShouldEqual("DELETE");
                    })
                    .Returns(Task.FromResult(response));

                await Subject.Delete(email);
            }

            [Test]
            public async void It_should_encode_the_email_address()
            {
                var version = Guid.NewGuid().ToString();

                email = "testing@test.com";

                Mocked<IClient>()
                    .Setup(x => x.Version)
                    .Returns(version);

                Mocked<IRequestSender>()
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Callback((Request r) =>
                    {
                        r.Url.ShouldEqual($"api/{version}/suppression-list/testing%40test.com");
                        r.Method.ShouldEqual("DELETE");
                    })
                    .Returns(Task.FromResult(response));

                await Subject.Delete(email);
            }
        }

        [TestFixture]
        public class CreateOrUpdateTests : AutoMoqTestFixture<Suppressions>
        {
            private Response response;
            private List<Suppression> suppressions;

            [SetUp]
            public void Setup()
            {
                ResetSubject();

                response = new Response
                {
                    StatusCode = HttpStatusCode.OK
                };

                Mocked<IRequestSender>()
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Returns(Task.FromResult(response));

                suppressions = new List<Suppression>
                {
                    new Suppression(),
                    new Suppression()
                };
            }

            [Test]
            public async void It_should_return_a_response_when_the_web_request_is_ok()
            {
                var result = await Subject.CreateOrUpdate(suppressions);

                result.ShouldNotBeNull();
            }

            [Test]
            public async void It_should_return_the_reason_phrase()
            {
                response.ReasonPhrase = Guid.NewGuid().ToString();
                var result = await Subject.CreateOrUpdate(suppressions);
                result.ReasonPhrase.ShouldEqual(response.ReasonPhrase);
            }

            [Test]
            public async void It_should_return_the_content()
            {
                response.Content = Guid.NewGuid().ToString();
                var result = await Subject.CreateOrUpdate(suppressions);
                result.Content.ShouldEqual(response.Content);
            }

            [Test]
            public async void It_should_make_a_properly_formed_request()
            {
                var client = Mocked<IClient>().Object;
                Mocked<IClient>().Setup(x => x.Version).Returns(Guid.NewGuid().ToString());
                Mocked<IRequestSender>()
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Callback((Request r) =>
                    {
                        r.Url.ShouldEqual($"api/{client.Version}/suppression-list");
                        r.Method.ShouldEqual("PUT JSON");
                    })
                    .Returns(Task.FromResult(response));

                await Subject.CreateOrUpdate(suppressions);
            }

            [Test]
            public async void It_should_throw_if_the_http_status_code_is_not_ok()
            {
                response.StatusCode = HttpStatusCode.Accepted;

                Exception exception = null;
                try
                {
                    await Subject.CreateOrUpdate(suppressions);
                }
                catch(ResponseException ex)
                {
                    exception = ex;
                }

                exception.ShouldNotBeNull();
            }
        }
    }
}