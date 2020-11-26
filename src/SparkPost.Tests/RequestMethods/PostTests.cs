using System;
using System.Net.Http;
using SparkPost.RequestMethods;
using Xunit;

namespace SparkPost.Tests.RequestMethods
{
    public class PostTests
    {
        public class CanExecuteTests
        {
            private readonly Post post;

            public CanExecuteTests()
            {
                var httpClient = new HttpClient();
                post = new Post(httpClient);
            }

            [Fact]
            public void It_should_return_true_for_post()
            {
                var request = new Request {Method = "POST"};
                Assert.True(post.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_post_lower()
            {
                var request = new Request {Method = "post"};
                Assert.True(post.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_post_spaces()
            {
                var request = new Request {Method = "post  "};
                Assert.True(post.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_others()
            {
                var request = new Request {Method = Guid.NewGuid().ToString()};
                Assert.False(post.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_nil()
            {
                var request = new Request {Method = null};
                Assert.False(post.CanExecute(request));
            }
        }
    }
}