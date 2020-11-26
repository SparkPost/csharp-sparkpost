using Xunit;

namespace SparkPost.Tests
{
    public class RelayWebhookTests
    {
        public class DefaultTests
        {
            [Fact]
            public void It_should_initialize_match()
            {
                Assert.NotNull(new RelayWebhook().Match);
            }

            [Fact]
            public void It_should_initialize_match_protocol()
            {
                Assert.Equal("SMTP", new RelayWebhook().Match.Protocol);
            }
        }
    }
}