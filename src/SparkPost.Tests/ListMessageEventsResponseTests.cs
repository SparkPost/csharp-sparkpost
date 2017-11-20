using NUnit.Framework;
using Shouldly;

namespace SparkPost.Tests
{
    public class ListMessageEventsResponseTests
    {
        [TestFixture]
        public class DefaultTests
        {
            [Test]
            public void It_should_not_have_nil_links()
            {
                var response = new ListMessageEventsResponse();
                response.Links.ShouldNotBeNull();
            }

            [Test]
            public void It_should_not_have_nil_events()
            {
                var response = new ListMessageEventsResponse();
                response.MessageEvents.ShouldNotBeNull();
            }
        }
    }
}