using Xunit;

namespace SparkPost.Tests
{
    public class WebhookTests
    {
        public class DefaultTests
        {
            [Fact]
            public void It_should_initialize_events()
            {
                Assert.NotNull(new Webhook().Events);
            }
        }
    }
}