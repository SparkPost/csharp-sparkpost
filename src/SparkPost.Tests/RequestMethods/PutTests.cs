using System;
using System.Net.Http;
using SparkPost.RequestMethods;
using Xunit;

namespace SparkPost.Tests.RequestMethods
{
    public class PutTests
    {
        public class CanExecuteTests
        {
            private readonly Put put;

            public CanExecuteTests()
            {
                var httpClient = new HttpClient();
                put = new Put(httpClient);
            }

            [Fact]
            public void It_should_return_true_for_put()
            {
                var request = new Request {Method = "PUT"};
                Assert.True(put.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_put_lower()
            {
                var request = new Request {Method = "put"};
                Assert.True(put.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_put_json()
            {
                var request = new Request {Method = "PUT JSON"};
                Assert.True(put.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_others()
            {
                var request = new Request {Method = Guid.NewGuid().ToString()};
                Assert.False(put.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_nil()
            {
                var request = new Request {Method = null};
                Assert.False(put.CanExecute(request));
            }
        }
    }
}