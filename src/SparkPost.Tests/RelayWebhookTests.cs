using NUnit.Framework;
using Shouldly;

namespace SparkPost.Tests
{
    public class RelayWebhookTests
    {
        [TestFixture]
        public class DefaultTests
        {
            [Test]
            public void It_should_initialize_match()
            {
                (new RelayWebhook()).Match.ShouldNotBeNull();
            }

            [Test]
            public void It_should_initialize_match_protocol()
            {
                (new RelayWebhook()).Match.Protocol.ShouldBe("SMTP");
            }
        }
    }
}