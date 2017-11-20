using System.Threading.Tasks;
using AutoMoq.Helpers;
using Moq;
using NUnit.Framework;
using Shouldly;
using SparkPost.RequestSenders;

namespace SparkPost.Tests.RequestSenders
{
    public class RequestSenderTests
    {
        [TestFixture]
        public class SendTests : AutoMoqTestFixture<RequestSender>
        {
            [SetUp]
            public void Setup()
            {
                ResetSubject();

                Mocked<IClient>().SetupAllProperties();
                client = new Client(null);

                request = new Request();

                async = new Mock<AsyncRequestSender>(null, null);
                sync = new Mock<SyncRequestSender>(null);

                Mocker.SetInstance<IClient>(client);
                Mocker.SetInstance(async.Object);
                Mocker.SetInstance(sync.Object);
            }

            private Request request;
            private Mock<AsyncRequestSender> async;
            private Mock<SyncRequestSender> sync;
            private Client client;

            [Test]
            public void It_should_return_the_result_from_async()
            {
                client.CustomSettings.SendingMode = SendingModes.Async;

                var response = Task.FromResult(new Response());
                async.Setup(x => x.Send(request)).Returns(response);

                var result = Subject.Send(request);
                result.Result.ShouldBeSameAs(response.Result);
            }

            [Test]
            public void It_should_return_the_result_from_sync()
            {
                client.CustomSettings.SendingMode = SendingModes.Sync;

                var response = Task.FromResult(new Response());
                sync.Setup(x => x.Send(request)).Returns(response);

                var result = Subject.Send(request);
                result.Result.ShouldBeSameAs(response.Result);
            }
        }
    }
}