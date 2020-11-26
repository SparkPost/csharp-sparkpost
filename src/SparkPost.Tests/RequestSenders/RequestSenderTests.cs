using System.Threading.Tasks;
using Moq;
using SparkPost.RequestSenders;
using Xunit;

namespace SparkPost.Tests.RequestSenders
{
    public class RequestSenderTests
    {
        public class SendTests
        {
            private readonly RequestSender fixture;
            private readonly Request request;
            private readonly Mock<AsyncRequestSender> async;
            private readonly Mock<SyncRequestSender> sync;
            private readonly Client client;

            public SendTests()
            {
                client = new Client(null);

                request = new Request();

                async = new Mock<AsyncRequestSender>(null, null);
                sync = new Mock<SyncRequestSender>(null);

                fixture = new RequestSender(async.Object, sync.Object, client);
            }

            [Fact]
            public void It_should_return_the_result_from_async()
            {
                client.CustomSettings.SendingMode = SendingModes.Async;

                var response = Task.FromResult(new Response());
                async.Setup(x => x.Send(request)).Returns(response);

                var result = fixture.Send(request);
                Assert.Equal(response.Result, result.Result);
            }

            [Fact]
            public void It_should_return_the_result_from_sync()
            {
                client.CustomSettings.SendingMode = SendingModes.Sync;

                var response = Task.FromResult(new Response());
                sync.Setup(x => x.Send(request)).Returns(response);

                var result = fixture.Send(request);
                Assert.Equal(response.Result, result.Result);
            }
        }
    }
}