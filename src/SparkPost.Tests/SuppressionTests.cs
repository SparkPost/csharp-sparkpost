using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Moq;
using SparkPost.RequestSenders;
using Xunit;

namespace SparkPost.Tests
{
    public class SuppressionTests
    {
        public class DeleteTests
        {
            private readonly Response response;
            private readonly string email;
            private readonly Suppressions suppressions;
            private readonly Mock<IClient> client;
            private readonly Mock<IRequestSender> requestSender;

            public DeleteTests()
            {
                response = new Response { StatusCode = HttpStatusCode.NoContent };

                requestSender = new Mock<IRequestSender>();

                requestSender.Setup(x => x.Send(It.IsAny<Request>()))
                             .Returns(Task.FromResult(response));

                var dataMapper = new Mock<IDataMapper>();

                client = new Mock<IClient>();

                suppressions = new Suppressions(client.Object, requestSender.Object, dataMapper.Object);

                email = Guid.NewGuid().ToString();
            }

            [Fact]
            public async Task It_should_return_true_if_the_web_request_returns_no_content()
            {
                var result = await suppressions.Delete(email);
                Assert.True(result);
            }

            [Fact]
            public async Task It_should_return_false_if_the_web_request_returns_anything_but_no_content()
            {
                response.StatusCode = HttpStatusCode.Accepted;
                bool deleted = await suppressions.Delete(email);
                Assert.False(deleted);

                response.StatusCode = HttpStatusCode.Ambiguous;
                deleted = await suppressions.Delete(email);
                Assert.False(deleted);

                response.StatusCode = HttpStatusCode.UpgradeRequired;
                deleted = await suppressions.Delete(email);
                Assert.False(deleted);
            }

            [Fact]
            public async Task It_should_build_the_web_request_parameters_correctly()
            {
                var version = Guid.NewGuid().ToString();

                client
                    .Setup(x => x.Version)
                    .Returns(version);

                requestSender
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Callback((Request r) =>
                    {
                        Assert.Equal($"api/{version}/suppression-list/{email}", r.Url);
                        Assert.Equal("DELETE", r.Method);
                    })
                    .Returns(Task.FromResult(response));

                await suppressions.Delete(email);
            }

            [Fact]
            public async Task It_should_encode_the_email_address()
            {
                var version = Guid.NewGuid().ToString();

                var email = "testing@test.com";

                client
                    .Setup(x => x.Version)
                    .Returns(version);

                requestSender
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Callback((Request r) =>
                    {
                        Assert.Equal($"api/{version}/suppression-list/testing%40test.com", r.Url);
                        Assert.Equal("DELETE", r.Method);
                    })
                    .Returns(Task.FromResult(response));

                await suppressions.Delete(email);
            }
        }

        public class CreateOrUpdateTests
        {
            private readonly Response response;
            private readonly List<Suppression> suppressionsList;
            private readonly Suppressions suppressions;
            private readonly Mock<IClient> client;
            private readonly Mock<IRequestSender> requestSender;

            public CreateOrUpdateTests()
            {
                response = new Response
                {
                    StatusCode = HttpStatusCode.OK
                };

                suppressionsList = new List<Suppression>
                {
                    new Suppression(),
                    new Suppression()
                };

                requestSender = new Mock<IRequestSender>();
                requestSender.Setup(x => x.Send(It.IsAny<Request>()))
                             .Returns(Task.FromResult(response));

                var dataMapper = new Mock<IDataMapper>();

                client = new Mock<IClient>();

                suppressions = new Suppressions(client.Object, requestSender.Object, dataMapper.Object);
            }

            [Fact]
            public async Task It_should_return_a_response_when_the_web_request_is_ok()
            {
                var result = await suppressions.CreateOrUpdate(suppressionsList);
                Assert.NotNull(result);
            }

            [Fact]
            public async Task It_should_return_the_reason_phrase()
            {
                response.ReasonPhrase = Guid.NewGuid().ToString();
                var result = await suppressions.CreateOrUpdate(suppressionsList);
                Assert.Equal(response.ReasonPhrase, result.ReasonPhrase);
            }

            [Fact]
            public async Task It_should_return_the_content()
            {
                response.Content = Guid.NewGuid().ToString();
                var result = await suppressions.CreateOrUpdate(suppressionsList);
                Assert.Equal(response.Content, result.Content);
            }

            [Fact]
            public async Task It_should_make_a_properly_formed_request()
            {
                client.Setup(x => x.Version)
                      .Returns(Guid.NewGuid().ToString());
                requestSender
                    .Setup(x => x.Send(It.IsAny<Request>()))
                    .Callback((Request r) =>
                    {
                        Assert.Equal($"api/{client.Object.Version}/suppression-list", r.Url);
                        Assert.Equal("PUT JSON", r.Method);
                    })
                    .Returns(Task.FromResult(response));

                await suppressions.CreateOrUpdate(suppressionsList);
            }

            [Fact]
            public async Task It_should_throw_if_the_http_status_code_is_not_ok()
            {
                response.StatusCode = HttpStatusCode.Accepted;

                await Assert.ThrowsAsync<ResponseException>(async () => await suppressions.CreateOrUpdate(suppressionsList));
            }
        }
    }
}