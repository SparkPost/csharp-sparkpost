using AutoMoq.Helpers;
using NUnit.Framework;
using Shouldly;
using SparkPost.RequestMethods;

namespace SparkPost.Tests
{
    public class RequestMethodFinderTests
    {
        [TestFixture]
        public class FindForTests : AutoMoqTestFixture<RequestMethodFinder>
        {
            [Test]
            public void It_should_return_put_for_put()
            {
                Subject.FindFor(new Request {Method = "PUT"})
                    .ShouldBeType(typeof (Put));
            }

            [Test]
            public void It_should_return_post_for_post()
            {
                Subject.FindFor(new Request {Method = "POST"})
                    .ShouldBeType(typeof (Post));
            }

            [Test]
            public void It_should_return_delete_for_delete()
            {
                Subject.FindFor(new Request {Method = "DELETE"})
                    .ShouldBeType(typeof (Delete));
            }

            [Test]
            public void It_should_return_get_for_get()
            {
                Subject.FindFor(new Request {Method = "GET"})
                    .ShouldBeType(typeof (Get));
            }
        }
    }
}