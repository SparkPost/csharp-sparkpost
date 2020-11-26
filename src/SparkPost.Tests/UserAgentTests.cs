using System;
using Xunit;

namespace SparkPost.Tests
{
    public partial class ClientTests
    {
        public partial class UserAgentTests
        {
            private readonly Client.Settings settings;

            public UserAgentTests()
            {
                settings = new Client.Settings();
            }

            [Fact]
            public void It_should_default_to_the_library_version()
            {
                Assert.Equal($"csharp-sparkpost/1.15.0", settings.UserAgent);
            }

            [Fact]
            public void It_should_allow_the_user_agent_to_be_changed()
            {
                var userAgent = Guid.NewGuid().ToString();
                settings.UserAgent = userAgent;
                Assert.Equal(userAgent, settings.UserAgent);
            }
        }
    }
}