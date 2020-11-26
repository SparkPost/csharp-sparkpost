using SparkPost.Utilities;
using Xunit;

namespace SparkPost.Tests.Utilities
{
    public class SnakeCaseTests
    {
        [Fact]
        public void It_should_convert_things_to_snake_case()
        {
            Assert.Equal("t", SnakeCase.Convert("T"));
            Assert.Equal("test", SnakeCase.Convert("Test"));
            Assert.Equal("t_e_s_t", SnakeCase.Convert("TEST"));
            Assert.Equal("john_galt", SnakeCase.Convert("JohnGalt"));
        }

        [Fact]
        public void It_should_handle_harder_strings()
        {
            Assert.Equal("test_testing", SnakeCase.Convert("TestTesting"));
            Assert.Equal("testing_test", SnakeCase.Convert("TestingTest"));
            Assert.Equal("appppp_appppppp", SnakeCase.Convert("ApppppAppppppp"));
            Assert.Equal("appppppp_appppp", SnakeCase.Convert("ApppppppAppppp"));
        }

        [Fact]
        public void It_should_convert_null_to_null()
        {
            Assert.Null(SnakeCase.Convert(null));
        }
    }
}