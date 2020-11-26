using System;
using System.Net.Http;
using Moq;
using SparkPost.RequestMethods;
using Xunit;

namespace SparkPost.Tests.RequestMethods
{
    public class GetTests
    {
        public class CanExecuteTests
        {
            private readonly Get get;

            public CanExecuteTests()
            {
                var httpClient = new HttpClient();
                var dataMapper = new Mock<IDataMapper>();
                get = new Get(httpClient, dataMapper.Object);
            }

            [Fact]
            public void It_should_return_true_for_get()
            {
                var request = new Request {Method = "GET"};
                Assert.True(get.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_get_lower()
            {
                var request = new Request {Method = "get"};
                Assert.True(get.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_get_spacing()
            {
                var request = new Request {Method = "get "};
                Assert.True(get.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_others()
            {
                var request = new Request {Method = Guid.NewGuid().ToString()};
                Assert.False(get.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_null()
            {
                var request = new Request {Method = null};
                Assert.False(get.CanExecute(request));
            }
        }
    }
}