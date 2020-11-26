using System.Threading.Tasks;
using Moq;
using SparkPost.RequestSenders;
using Xunit;

namespace SparkPost.Tests.RequestSenders
{
    public class SyncRequestSenderTests
    {
        public class SendTests
        {
            private SyncRequestSender subject;

            [Fact]
            public async Task It_should_return_the_result_from_the_parent_request_sender()
            {
                var request = new Request();
                var response = new Response();

                var requestSender = new Mock<IRequestSender>();
                requestSender.Setup(x => x.Send(request)).Returns(Task.FromResult(response));
                subject = new SyncRequestSender(requestSender.Object);

                var result = await subject.Send(request);

                Assert.Equal(response, result);
            }
        }
    }
}