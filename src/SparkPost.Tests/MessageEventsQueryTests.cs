using NUnit.Framework;
using Should;

namespace SparkPost.Tests
{
    public class MessageEventsQueryTests
    {
        [TestFixture]
        public class Defaults
        {
            [Test]
            public void It_should_have_a_default_build_list()
            {
                new MessageEventsQuery().BounceClasses.ShouldNotBeNull();
            }

            [Test]
            public void It_should_have_a_defualt_campaign_ids_list()
            {
                new MessageEventsQuery().CampaignIds.ShouldNotBeNull();
            }

            [Test]
            public void It_should_have_a_default_friendly_froms_list()
            {
                new MessageEventsQuery().FromAddresses.ShouldNotBeNull();
            }

            [Test]
            public void It_should_have_a_default_message_ids_list()
            {
                new MessageEventsQuery().MessageIds.ShouldNotBeNull();
            }

            [Test]
            public void It_should_have_a_recipients_list()
            {
                new MessageEventsQuery().Recipients.ShouldNotBeNull();
            }

            [Test]
            public void It_should_have_a_Subaccounts_list()
            {
                new MessageEventsQuery().Subaccounts.ShouldNotBeNull();
            }

            [Test]
            public void It_should_have_TemplateIds_list()
            {
                new MessageEventsQuery().TemplateIds.ShouldNotBeNull();
            }

            [Test]
            public void It_should_have_Transmissions_list()
            {
                new MessageEventsQuery().TransmissionIds.ShouldNotBeNull();
            }

        }
    }
}