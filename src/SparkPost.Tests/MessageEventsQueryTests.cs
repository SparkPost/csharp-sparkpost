using Xunit;

namespace SparkPost.Tests
{
    public class MessageEventsQueryTests
    {
        public class Defaults
        {
            [Fact]
            public void It_should_have_a_default_build_list()
            {
                Assert.NotNull(new MessageEventsQuery().BounceClasses);
            }

            [Fact]
            public void It_should_have_a_default_campaign_ids_list()
            {
                Assert.NotNull(new MessageEventsQuery().CampaignIds);
            }

            [Fact]
            public void It_should_have_a_default_friendly_froms_list()
            {
                Assert.NotNull(new MessageEventsQuery().FriendlyFroms);
            }

            [Fact]
            public void It_should_have_a_default_message_ids_list()
            {
                Assert.NotNull(new MessageEventsQuery().MessageIds);
            }

            [Fact]
            public void It_should_have_a_recipients_list()
            {
                Assert.NotNull(new MessageEventsQuery().Recipients);
            }

            [Fact]
            public void It_should_have_a_Subaccounts_list()
            {
                Assert.NotNull(new MessageEventsQuery().Subaccounts);
            }

            [Fact]
            public void It_should_have_TemplateIds_list()
            {
                Assert.NotNull(new MessageEventsQuery().TemplateIds);
            }

            [Fact]
            public void It_should_have_Transmissions_list()
            {
                Assert.NotNull(new MessageEventsQuery().TransmissionIds);
            }

        }
    }
}