using SparkPost.RequestMethods;
using Xunit;

namespace SparkPost.Tests
{
    public class RequestMethodFinderTests
    {
        public class FindForTests
        {
            private readonly RequestMethodFinder finder;

            public FindForTests()
            {
                finder = new RequestMethodFinder(null, null);
            }

            [Fact]
            public void It_should_return_put_for_put()
            {
                Assert.IsType<Put>(finder.FindFor(new Request {Method = "PUT"}));
            }

            [Fact]
            public void It_should_return_post_for_post()
            {
                Assert.IsType<Post>(finder.FindFor(new Request {Method = "POST"}));
            }

            [Fact]
            public void It_should_return_delete_for_delete()
            {
                Assert.IsType<Delete>(finder.FindFor(new Request {Method = "DELETE"}));
            }

            [Fact]
            public void It_should_return_get_for_get()
            {
                Assert.IsType<Get>(finder.FindFor(new Request {Method = "GET"}));
            }
        }
    }
}