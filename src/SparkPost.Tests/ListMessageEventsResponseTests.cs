using Xunit;

namespace SparkPost.Tests
{
    public class ListMessageEventsResponseTests
    {
        public class DefaultTests
        {
            [Fact]
            public void It_should_not_have_nil_links()
            {
                var response = new ListMessageEventsResponse();
                Assert.NotNull(response.Links);
            }

            [Fact]
            public void It_should_not_have_nil_events()
            {
                var response = new ListMessageEventsResponse();
                Assert.NotNull(response.MessageEvents);
            }
        }
    }
}