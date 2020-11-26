using System;
using System.Net.Http;
using SparkPost.RequestMethods;
using Xunit;

namespace SparkPost.Tests.RequestMethods
{
    public class DeleteTests
    {
        public class CanExecuteTests
        {
            private readonly Delete delete;

            public CanExecuteTests()
            {
                var httpClient = new HttpClient();
                delete = new Delete(httpClient);
            }

            [Fact]
            public void It_should_return_true_for_delete()
            {
                var request = new Request {Method = "DELETE"};
                Assert.True(delete.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_delete_lower()
            {
                var request = new Request {Method = "delete"};
                Assert.True(delete.CanExecute(request));
            }

            [Fact]
            public void It_should_return_true_for_delete_spaces()
            {
                var request = new Request {Method = "delete  "};
                Assert.True(delete.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_others()
            {
                var request = new Request {Method = Guid.NewGuid().ToString()};
                Assert.False(delete.CanExecute(request));
            }

            [Fact]
            public void It_should_return_false_for_null()
            {
                var request = new Request {Method = null};
                Assert.False(delete.CanExecute(request));
            }
        }
    }
}