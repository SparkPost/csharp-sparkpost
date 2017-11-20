using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using SparkPost.ValueMappers;

namespace SparkPost.Tests
{
    public class DataMapperTests
    {

        [TestFixture]
        public class RecipientMappingTests
        {
            [SetUp]
            public void Setup()
            {
                recipient = new Recipient();
                mapper = new DataMapper("v1");
            }

            private DataMapper mapper;
            private Recipient recipient;

            [Test]
            public void address()
            {
                var value = Guid.NewGuid().ToString();
                recipient.Address.Email = value;
                mapper.ToDictionary(recipient)
                    ["address"]
                    .CastAs<IDictionary<string, object>>()
                    ["email"]
                    .ShouldBe(value);
            }

            [Test]
            public void return_path()
            {
                var value = Guid.NewGuid().ToString();
                recipient.ReturnPath = value;
                mapper.ToDictionary(recipient)["return_path"].ShouldBe(value);
            }

            [Test]
            public void tags()
            {
                var tag1 = Guid.NewGuid().ToString();
                var tag2 = Guid.NewGuid().ToString();
                recipient.Tags.Add(tag1);
                recipient.Tags.Add(tag2);
                var theTags = mapper.ToDictionary(recipient)
                    ["tags"];
                theTags
                    .CastAs<IEnumerable<object>>()
                    .Count().ShouldBe(2);
                mapper.ToDictionary(recipient)
                    ["tags"]
                    .CastAs<IEnumerable<object>>()
                    .ShouldContain(tag1);
                mapper.ToDictionary(recipient)
                    ["tags"]
                    .CastAs<IEnumerable<object>>()
                    .ShouldContain(tag2);
            }

            [Test]
            public void empty_tags_are_ignored()
            {
                mapper.ToDictionary(recipient)
                    .Keys.ShouldNotContain("tags");
            }

            [Test]
            public void metadata()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                recipient.Metadata[key] = value;
                mapper.ToDictionary(recipient)["metadata"]
                    .CastAs<IDictionary<string, object>>()[key].ShouldBe(value);
            }

            [Test]
            public void do_not_include_empty_metadata()
            {
                mapper.ToDictionary(recipient).Keys.ShouldNotContain("metadata");
            }

            [Test]
            public void substitution_data()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                recipient.SubstitutionData[key] = value;
                mapper.ToDictionary(recipient)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key].ShouldBe(value);
            }

            [Test]
            public void do_not_include_empty_substitution_data()
            {
                mapper.ToDictionary(recipient).Keys.ShouldNotContain("substitution_data");
            }

            [Test]
            public void do_not_alter_the_keys_passed_to_substitution_data()
            {
                var key = "TEST";
                var value = Guid.NewGuid().ToString();
                recipient.SubstitutionData[key] = value;
                mapper.ToDictionary(recipient)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key].ShouldBe(value);
            }

            [Test]
            public void The_type_should_be_ignored()
            {
                recipient.Type = RecipientType.CC;
                mapper.ToDictionary(recipient).Keys.ShouldNotContain("type");
            }
        }

        [TestFixture]
        public class AddressMappingTests
        {
            [SetUp]
            public void Setup()
            {
                address = new Address();
                mapper = new DataMapper("v1");
            }

            private Address address;
            private DataMapper mapper;

            [Test]
            public void email()
            {
                var value = Guid.NewGuid().ToString();
                address.Email = value;
                mapper.ToDictionary(address)["email"].ShouldBe(value);
            }

            [Test]
            public void name()
            {
                var value = Guid.NewGuid().ToString();
                address.Name = value;
                mapper.ToDictionary(address)["name"].ShouldBe(value);
            }

            [Test]
            public void header_to()
            {
                var value = Guid.NewGuid().ToString();
                address.HeaderTo = value;
                mapper.ToDictionary(address)["header_to"].ShouldBe(value);
            }

            [Test]
            public void header_to_is_not_returned_if_empty()
            {
                mapper.ToDictionary(address)
                    .Keys.ShouldNotContain("header_to");
            }
        }

        [TestFixture]
        public class TransmissionMappingTests
        {
            [SetUp]
            public void Setup()
            {
                transmission = new Transmission();
                mapper = new DataMapper("v1");
            }

            private Transmission transmission;
            private DataMapper mapper;

            [Test]
            public void It_should_set_the_content_dictionary()
            {
                var email = Guid.NewGuid().ToString();
                transmission.Content.From = new Address { Email = email };
                mapper.ToDictionary(transmission)["content"]
                    .CastAs<IDictionary<string, object>>()["from"]
                    .CastAs<IDictionary<string, object>>()["email"]
                    .ShouldBe(email);
            }

            [Test]
            public void It_should_set_the_recipients()
            {
                var recipient1 = new Recipient { Address = new Address { Email = Guid.NewGuid().ToString() } };
                var recipient2 = new Recipient { Address = new Address { Email = Guid.NewGuid().ToString() } };

                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                var result = mapper.ToDictionary(transmission)["recipients"] as IEnumerable<IDictionary<string, object>>;
                result.Count().ShouldBe(2);
                result.ToList()[0]["address"]
                    .CastAs<IDictionary<string, object>>()
                    ["email"].ShouldBe(recipient1.Address.Email);
                result.ToList()[1]["address"]
                    .CastAs<IDictionary<string, object>>()
                    ["email"].ShouldBe(recipient2.Address.Email);
            }

            [Test]
            public void It_should_set_the_recipients_to_a_list_id_if_a_list_id_is_provided()
            {
                var listId = Guid.NewGuid().ToString();
                transmission.ListId = listId;

                var result = mapper.ToDictionary(transmission)["recipients"] as IDictionary<string, object>;
                result["list_id"].ShouldBe(listId);
            }

            [Test]
            public void campaign_id()
            {
                var value = Guid.NewGuid().ToString();
                transmission.CampaignId = value;
                mapper.ToDictionary(transmission)["campaign_id"].ShouldBe(value);
            }

            [Test]
            public void description()
            {
                var value = Guid.NewGuid().ToString();
                transmission.Description = value;
                mapper.ToDictionary(transmission)["description"].ShouldBe(value);
            }

            [Test]
            public void return_path()
            {
                var value = Guid.NewGuid().ToString();
                transmission.ReturnPath = value;
                mapper.ToDictionary(transmission)["return_path"].ShouldBe(value);
            }

            [Test]
            public void do_not_send_the_return_path_if_it_is_not_provided()
            {
                mapper.ToDictionary(transmission).Keys.ShouldNotContain("return_path");
            }

            [Test]
            public void metadata()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                transmission.Metadata[key] = value;
                mapper.ToDictionary(transmission)["metadata"]
                    .CastAs<IDictionary<string, object>>()[key].ShouldBe(value);
            }

            [Test]
            public void do_not_alter_the_keys_passed_to_metadata()
            {
                var key = "TEST";
                var value = Guid.NewGuid().ToString();
                transmission.Metadata[key] = value;
                mapper.ToDictionary(transmission)["metadata"]
                    .CastAs<IDictionary<string, object>>()[key].ShouldEqual(value);
            }

            [Test]
            public void do_not_include_empty_metadata()
            {
                mapper.ToDictionary(transmission).Keys.ShouldNotContain("metadata");
            }

            [Test]
            public void substitution_data()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                transmission.SubstitutionData[key] = value;
                mapper.ToDictionary(transmission)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key].ShouldBe(value);
            }

            [Test]
            public void do_not_include_empty_substitution_data()
            {
                mapper.ToDictionary(transmission).Keys.ShouldNotContain("substitution_data");
            }

            [Test]
            public void do_not_alter_the_keys_passed_to_substitution_data()
            {
                var key = "TEST";
                var value = Guid.NewGuid().ToString();
                transmission.SubstitutionData[key] = value;
                mapper.ToDictionary(transmission)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key].ShouldBe(value);
            }

            [Test]
            public void options()
            {
                transmission.Options.ClickTracking = true;
                mapper.ToDictionary(transmission)["options"]
                    .CastAs<IDictionary<string, object>>()
                    ["click_tracking"].ShouldBe(true);

                transmission.Options.ClickTracking = false;
                mapper.ToDictionary(transmission)["options"]
                    .CastAs<IDictionary<string, object>>()
                    ["click_tracking"].ShouldBe(false);

                transmission.Options.InlineCss = true;
                mapper.ToDictionary(transmission)["options"]
                    .CastAs<IDictionary<string, object>>()
                    ["inline_css"].ShouldBe(true);
            }
        }

        [TestFixture]
        public class MappingCcFields
        {
            [SetUp]
            public void Setup()
            {
                transmission = new Transmission();
                mapper = new DataMapper("v1");
            }

            private Transmission transmission;
            private DataMapper mapper;

            [TestCase(true)]
            [TestCase(false)]
            public void It_should_set_the_CC_Header_for_only_the_cc_emails(bool useTo)
            {
                var recipient1 = new Recipient {Type = RecipientType.CC, Address = new Address {Email = Guid.NewGuid().ToString()}};
                var recipient2 = new Recipient {Type = RecipientType.BCC, Address = new Address { Email = Guid.NewGuid().ToString()}};
                var recipient3 = new Recipient {Type = RecipientType.CC, Address = new Address {Email = Guid.NewGuid().ToString()}};
                var recipient4 = useTo 
                        ? new Recipient { Type = RecipientType.To, Address = new Address { Email = Guid.NewGuid().ToString() } }
                        : new Recipient();

                transmission.Recipients = new List<Recipient> {recipient1, recipient2, recipient3, recipient4};

                var cc = mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    ["headers"]
                    .CastAs<IDictionary<string, string>>()
                    ["CC"];

                cc.ShouldBe(recipient1.Address.Email + ", " + recipient3.Address.Email);
            }
            
            [TestCase(true)]
            [TestCase(false)]
            public void It_should_not_overwrite_any_existing_headers(bool useTo)
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();

                var recipient1 = new Recipient {Type = RecipientType.CC, Address = new Address {Email = Guid.NewGuid().ToString()}};
                var recipient2 = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address() }
                        : new Recipient();
                transmission.Recipients = new List<Recipient> {recipient1, recipient2};

                transmission.Content.Headers[key] = value;

                 mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    ["headers"]
                    .CastAs<IDictionary<string, string>>()
                    [key].ShouldBe(value);
            }

            [TestCase(true)]
            [TestCase(false)]
            public void It_should_not_set_the_cc_if_there_are_no_cc_emails(bool useTo)
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();

                var recipient1 = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address { Email = Guid.NewGuid().ToString() } }
                        : new Recipient();
                var recipient2 = new Recipient {Type = RecipientType.BCC, Address = new Address {Email = Guid.NewGuid().ToString()}};
                var recipient3 = new Recipient { Type = RecipientType.BCC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                transmission.Recipients = new List<Recipient> {recipient1, recipient2, recipient3};

                transmission.Content.Headers[key] = value;

                 mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    ["headers"]
                    .CastAs<IDictionary<string, string>>()
                    .ContainsKey("CC")
                    .ShouldBeFalse();
            }

            [TestCase(true)]
            [TestCase(false)]
            public void It_should_not_set_a_header_value_if_there_are_no_ccs(bool useTo)
            {
                var recipient1 = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address { Email = Guid.NewGuid().ToString() } }
                        : new Recipient();
                var recipient2 = new Recipient {Type = RecipientType.BCC, Address = new Address {Email = Guid.NewGuid().ToString()}};
                transmission.Recipients = new List<Recipient> {recipient1, recipient2};

                 mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    .ContainsKey("headers")
                    .ShouldBeFalse();
            }

            [TestCase(true)]
            [TestCase(false)]
            public void It_should_ignore_empty_ccs(bool useTo)
            {
                var recipient1 = new Recipient {Type = RecipientType.CC, Address = new Address {Email = ""}};
                var recipient2 = new Recipient {Type = RecipientType.CC, Address = new Address {Email = null}};
                var recipient3 = new Recipient {Type = RecipientType.CC, Address = new Address {Email = " "}};
                var toRecipient = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address() }
                        : new Recipient();
                transmission.Recipients = new List<Recipient> {recipient1, recipient2, recipient3, toRecipient};

                 mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    .ContainsKey("headers")
                    .ShouldBeFalse();
            }

            [TestCase(true)]
            [TestCase(false)]
            public void It_should_ignore_any_cc_recipients_with_no_address(bool useTo)
            {
                var recipient1 = new Recipient {Type = RecipientType.CC, Address = null};
                var toRecipient = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address() }
                        : new Recipient();
                transmission.Recipients = new List<Recipient> {recipient1, toRecipient};

                 mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    .ContainsKey("headers")
                    .ShouldBeFalse();
            }

            [Test]
            public void It_should_set_the_name_and_header_to_fields()
            {
                var toName = Guid.NewGuid().ToString();
                var toEmail = Guid.NewGuid().ToString();

                var toRecipient = new Recipient { Type = RecipientType.To, Address = new Address(toEmail, toName) };
                var ccRecipient = new Recipient { Type = RecipientType.CC, Address = new Address() };
                var bccRecipient = new Recipient { Type = RecipientType.BCC, Address = new Address() };
                transmission.Recipients = new List<Recipient>() { toRecipient, ccRecipient, bccRecipient };

                var addresses = mapper.ToDictionary(transmission)
                        ["recipients"]
                        .CastAs<IEnumerable<IDictionary<string, object>>>()
                        .Select(r => r["address"])
                        .Cast<IDictionary<string, object>>();

                foreach (var address in addresses)
                {
                    address["name"].ShouldBe(toName);
                    address["header_to"].ShouldBe(toEmail);
                }
            }

            [TestCase("Bob Jones", "bob@jones.com", "Bob Jones <bob@jones.com>")]
            [TestCase(null, "bob@jones.com", "bob@jones.com")]
            [TestCase("", "bob@jones.com", "bob@jones.com")]
            [TestCase("Jones, Bob", "bob@jones.com", "\"Jones, Bob\" <bob@jones.com>")]
            public void It_should_format_addresses_correctly(string name, string address, string result)
            {
                var recipient1 = new Recipient { Type = RecipientType.To, Address = new Address() };
                var recipient2 = new Recipient { Type = RecipientType.CC, Address = new Address(address, name) };
                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    ["headers"]
                    .CastAs<IDictionary<string, string>>()
                    ["CC"]
                    .ShouldBe(result);
            }

            [TestCase("Jones, Bob", "bob@jones.com ", "\"Jones, Bob\" <bob@jones.com>")]
            [TestCase("Jones, Bob", " bob@jones.com", "\"Jones, Bob\" <bob@jones.com>")]
            public void It_should_handle_white_space_in_the_email(string name, string address, string result)
            {
                var recipient1 = new Recipient { Type = RecipientType.To, Address = new Address() };
                var recipient2 = new Recipient { Type = RecipientType.CC, Address = new Address(address, name) };
                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    ["headers"]
                    .CastAs<IDictionary<string, string>>()
                    ["CC"]
                    .ShouldBe(result);
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            public void It_should_use_new_or_legacy_handling(int numOfTos)
            {
                var ccAddress = Guid.NewGuid().ToString();
                transmission.Recipients.Add(new Recipient { Type = RecipientType.CC, Address = new Address(ccAddress) });

                for (int i = 0; i < numOfTos; ++i)
                {
                    transmission.Recipients.Add(new Recipient { Type = RecipientType.To, Address = new Address("bob@example.com") });
                }

                var ccHeader = mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    ["headers"]
                    .CastAs<IDictionary<string, string>>()
                    ["CC"];

                if (numOfTos == 1)
                    ccHeader.ShouldBe(ccAddress);
                else
                    ccHeader.ShouldBe($"<{ccAddress}>");
            }

            [Test]
            public void It_should_use_legacy_handling_if_to_address_is_null()
            {
                var ccAddress = Guid.NewGuid().ToString();
                transmission.Recipients.Add(new Recipient { Type = RecipientType.CC, Address = new Address(ccAddress) });
                transmission.Recipients.Add(new Recipient { Type = RecipientType.To, Address = null });
                
                var ccHeader = mapper.ToDictionary(transmission)
                    ["content"]
                    .CastAs<IDictionary<string, object>>()
                    ["headers"]
                    .CastAs<IDictionary<string, string>>()
                    ["CC"];

                ccHeader.ShouldBe($"<{ccAddress}>");
            }
        }

        [TestFixture]
        public class ContentMappingTests
        {
            [SetUp]
            public void Setup()
            {
                content = new Content();
                mapper = new DataMapper("v1");
            }

            private Content content;
            private DataMapper mapper;

            [Test]
            public void from()
            {
                content.From.Email = Guid.NewGuid().ToString();
                content.From.HeaderTo = Guid.NewGuid().ToString();
                content.From.Name = Guid.NewGuid().ToString();

                var result = mapper.ToDictionary(content)["from"].CastAs<IDictionary<string, object>>();

                result["email"].ShouldBe(content.From.Email);
                result["header_to"].ShouldBe(content.From.HeaderTo);
                result["name"].ShouldBe(content.From.Name);
            }

            [Test]
            public void subject()
            {
                var value = Guid.NewGuid().ToString();
                content.Subject = value;
                mapper.ToDictionary(content)["subject"].ShouldBe(value);
            }

            [Test]
            public void text()
            {
                var value = Guid.NewGuid().ToString();
                content.Text = value;
                mapper.ToDictionary(content)["text"].ShouldBe(value);
            }

            [Test]
            public void template_id()
            {
                var value = Guid.NewGuid().ToString();
                content.TemplateId = value;
                mapper.ToDictionary(content)["template_id"].ShouldBe(value);
            }

            [Test]
            public void html()
            {
                var value = Guid.NewGuid().ToString();
                content.Html = value;
                mapper.ToDictionary(content)["html"].ShouldBe(value);
            }

            [Test]
            public void reply_to()
            {
                var value = Guid.NewGuid().ToString();
                content.ReplyTo = value;
                mapper.ToDictionary(content)["reply_to"].ShouldBe(value);
            }

            [Test]
            public void headers()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                content.Headers[key] = value;
                mapper.ToDictionary(content)["headers"]
                    .CastAs<IDictionary<string, string>>()
                    [key].ShouldBe(value);
            }

            [Test]
            public void do_not_include_empty_headers()
            {
                mapper.ToDictionary(content)
                    .Keys.ShouldNotContain("headers");
            }

            [Test]
            public void attachments()
            {
                var firstName = Guid.NewGuid().ToString();
                var secondName = Guid.NewGuid().ToString();
                content.Attachments.Add(new Attachment { Name = firstName });
                content.Attachments.Add(new Attachment { Name = secondName });

                var names = mapper.ToDictionary(content)["attachments"]
                    .CastAs<IEnumerable<object>>()
                    .Select(x => x.CastAs<Dictionary<string, object>>())
                    .Select(x => x["name"]);

                names.Count().ShouldBe(2);
                names.ShouldContain(firstName);
                names.ShouldContain(secondName);
            }

            [Test]
            public void no_attachments_should_not_include_the_attachments_block()
            {
                mapper.ToDictionary(content).Keys.ShouldNotContain("attachments");
            }

            [Test]
            public void inline_images()
            {
                var firstName = Guid.NewGuid().ToString();
                var secondName = Guid.NewGuid().ToString();
                content.InlineImages.Add(new InlineImage { Name = firstName });
                content.InlineImages.Add(new InlineImage { Name = secondName });

                var mappedAttachments = mapper.ToDictionary(content)["inline_images"];
                var names = mappedAttachments
                    .CastAs<IEnumerable<object>>()
                    .Select(x => x.CastAs<Dictionary<string, object>>())
                    .Select(x => x["name"]);

                names.Count().ShouldBe(2);
                names.ShouldContain(firstName);
                names.ShouldContain(secondName);
            }

            [Test]
            public void no_inline_images_should_not_include_the_inline_images_block()
            {
                mapper.ToDictionary(content).Keys.ShouldNotContain("inline_images");
            }
        }

        [TestFixture]
        public class OptionsMappingTests
        {
            [SetUp]
            public void Setup()
            {
                options = new Options();
                mapper = new DataMapper("v1");
            }

            private DataMapper mapper;
            private Options options;

            [Test]
            public void It_should_default_to_returning_null()
            {
                mapper.ToDictionary(options).ShouldBeNull();
            }

            [Test]
            public void open_tracking()
            {
                options.OpenTracking = true;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["open_tracking"].ShouldBe(true);

                options.OpenTracking = false;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["open_tracking"].ShouldBe(false);
            }

            [Test]
            public void click_tracking()
            {
                options.ClickTracking = true;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["click_tracking"].ShouldBe(true);

                options.ClickTracking = false;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["click_tracking"].ShouldBe(false);
            }

            [Test]
            public void transactional()
            {
                options.Transactional = true;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["transactional"].ShouldBe(true);

                options.Transactional = false;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["transactional"].ShouldBe(false);
            }

            [Test]
            public void sandbox()
            {
                options.Sandbox = true;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["sandbox"].ShouldBe(true);

                options.Sandbox = false;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["sandbox"].ShouldBe(false);
            }

            [Test]
            public void skip_suppression()
            {
                options.SkipSuppression = true;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["skip_suppression"].ShouldBe(true);

                options.SkipSuppression = false;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["skip_suppression"].ShouldBe(false);
            }

            [Test]
            public void start_time()
            {
                var startTime = "2015-02-11T08:00:00-04:00";
                options.StartTime = DateTimeOffset.Parse(startTime);
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["start_time"].ShouldBe(startTime);

                startTime = "2015-02-11T08:00:00-14:00";
                options.StartTime = DateTimeOffset.Parse(startTime);
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["start_time"].ShouldBe(startTime);
            }

            [Test]
            public void hide_start_time_if_it_is_missing()
            {
                options.OpenTracking = true;
                mapper.ToDictionary(options)
                    .CastAs<IDictionary<string, object>>()
                    .Keys.ShouldNotContain("start_time");
            }

            [Test]
            public void inline_css()
            {
                options.InlineCss = true;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["inline_css"].ShouldBe(true);

                options.InlineCss = false;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["inline_css"].ShouldBe(false);
            }

            [Test]
            public void ip_pool()
            {
                var ipPool = Guid.NewGuid().ToString();
                options.IpPool = ipPool;
                mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["ip_pool"].ShouldBe(ipPool);
            }
        }

        [TestFixture]
        public class FileMappingTests
        {
            private File file;
            private DataMapper mapper;

            [SetUp]
            public void Setup()
            {
                file = new Attachment();
                mapper = new DataMapper("v1");
            }

            [Test]
            public void name()
            {
                var value = Guid.NewGuid().ToString();
                file.Name = value;
                mapper.ToDictionary(file)["name"].ShouldBe(value);
            }

            [Test]
            public void type()
            {
                var value = Guid.NewGuid().ToString();
                file.Type = value;
                mapper.ToDictionary(file)["type"].ShouldBe(value);
            }

            [Test]
            public void data()
            {
                var value = Guid.NewGuid().ToString();
                file.Data = value;
                mapper.ToDictionary(file)["data"].ShouldBe(value);
            }
        }

        [TestFixture]
        public class WebhookTests
        {
            private DataMapper dataMapper;

            [SetUp]
            public void Setup()
            {
                dataMapper = new DataMapper();
            }

            [Test]
            public void Name()
            {
                var webhook = new Webhook {Name = Guid.NewGuid().ToString()};
                dataMapper.ToDictionary(webhook)["name"].ShouldBe(webhook.Name);
            }

            [Test]
            public void Target()
            {
                var webhook = new Webhook {Target = Guid.NewGuid().ToString()};
                dataMapper.ToDictionary(webhook)["target"].ShouldBe(webhook.Target);
            }

            [Test]
            public void AuthType()
            {
                var webhook = new Webhook {AuthType = Guid.NewGuid().ToString()};
                dataMapper.ToDictionary(webhook)["auth_type"].ShouldBe(webhook.AuthType);
            }

            [Test]
            public void AuthToken()
            {
                var webhook = new Webhook {AuthToken = Guid.NewGuid().ToString()};
                dataMapper.ToDictionary(webhook)["auth_token"].ShouldBe(webhook.AuthToken);
            }

            [Test]
            public void Events()
            {
                var first = Guid.NewGuid().ToString();
                var second = Guid.NewGuid().ToString();

                var webhook = new Webhook();
                webhook.Events.Add(first);
                webhook.Events.Add(second);

                var dictionary = dataMapper.ToDictionary(webhook);
                var events = dictionary["events"] as IEnumerable<object>;
                events.Count().ShouldBe(2);
                events.ShouldContain(first);
                events.ShouldContain(second);
            }

            [Test]
            public void AuthRequestDetails()
            {
                var webhook = new Webhook
                {
                    AuthRequestDetails = new
                    {
                        Url = "https://oauth.myurl.com/tokens",
                        Body = new {ClientId = "<oauth client id>", ClientSecret = "<oauth client secret>"}
                    }
                };

                var dictionary = dataMapper.ToDictionary(webhook);
                var authRequestDetails = dictionary["auth_request_details"].CastAs<IDictionary<string, object>>();
                authRequestDetails["url"].ShouldBe("https://oauth.myurl.com/tokens");

                authRequestDetails["body"]
                    .CastAs<IDictionary<string, object>>()
                    ["client_id"]
                    .ShouldBe("<oauth client id>");

                authRequestDetails["body"]
                    .CastAs<IDictionary<string, object>>()
                    ["client_secret"]
                    .ShouldBe("<oauth client secret>");
            }

            [Test]
            public void AuthCredentials()
            {
                var webhook = new Webhook
                {
                    AuthCredentials = new
                    {
                        access_token = "<oauth token>",
                        ExpiresIn = 3600
                    }
                };

                var dictionary = dataMapper.ToDictionary(webhook);
                var authRequestDetails = dictionary["auth_credentials"] as Dictionary<string, object>;
                authRequestDetails["access_token"].ShouldBe("<oauth token>");
                authRequestDetails["expires_in"].ShouldBe(3600);
            }
        }


        [TestFixture]
        public class SubaccountTests
        {
            private DataMapper dataMapper;

            [SetUp]
            public void Setup()
            {
                dataMapper = new DataMapper();
            }

            [Test]
            public void Id()
            {
                var subaccount = new Subaccount { Id = 432 };
                dataMapper.ToDictionary(subaccount)["id"].ShouldBe(subaccount.Id);
            }

            [Test]
            public void Name()
            {
                var subaccount = new Subaccount { Name = Guid.NewGuid().ToString() };
                dataMapper.ToDictionary(subaccount)["name"].ShouldBe(subaccount.Name);
            }

            [Test]
            public void Status()
            {
                var subaccount = new Subaccount { Status = SubaccountStatus.Terminated };
                dataMapper.ToDictionary(subaccount)["status"].ShouldBe(SubaccountStatus.Terminated.ToString().ToLowerInvariant());
            }

            [Test]
            public void IpPool()
            {
                var subaccount = new Subaccount { IpPool = Guid.NewGuid().ToString() };
                dataMapper.ToDictionary(subaccount)["ip_pool"].ShouldBe(subaccount.IpPool);
            }

            [Test]
            public void ComplianceStatus()
            {
                var subaccount = new Subaccount { ComplianceStatus = Guid.NewGuid().ToString() };
                dataMapper.ToDictionary(subaccount)["compliance_status"].ShouldBe(subaccount.ComplianceStatus);
            }
        }

        [TestFixture]
        public class AnythingTests
        {
            [Test]
            public void It_should_map_anything_using_our_conventions()
            {
                var dataMapper = new DataMapper();

                var dateTime = new DateTime(2016, 1, 2, 3, 4, 5);

                var result = dataMapper.CatchAll(new {FirstName = "Test1", LastName = "Test2", TheDate = dateTime});

                result["first_name"].ShouldBe("Test1");
                result["last_name"].ShouldBe("Test2");
                ((string)result["the_date"]).Substring(0, 16).ShouldBe("2016-01-02T03:04");
            }
        }

        [TestFixture]
        public class RelayWebhookTests
        {
            [SetUp]
            public void Setup()
            {
                relayWebhook = new RelayWebhook();
                mapper = new DataMapper("v1");
            }

            private RelayWebhook relayWebhook;
            private DataMapper mapper;

            [Test]
            public void name()
            {
                var value = Guid.NewGuid().ToString();
                relayWebhook.Name = value;
                mapper.ToDictionary(relayWebhook)["name"].ShouldBe(value);
            }

            [Test]
            public void match_domain()
            {
                var value = Guid.NewGuid().ToString();
                relayWebhook.Match = new RelayWebhookMatch {Domain = value};
                mapper.ToDictionary(relayWebhook)["match"]
                    .CastAs<IDictionary<string, object>>()
                    ["domain"].ShouldBe(value);
            }

            [Test]
            public void match_protocol()
            {
                var value = Guid.NewGuid().ToString();
                relayWebhook.Match = new RelayWebhookMatch {Protocol = value};
                mapper.ToDictionary(relayWebhook)["match"]
                    .CastAs<IDictionary<string, object>>()
                    ["protocol"].ShouldBe(value);
            }
        }

        [TestFixture]
        public class SendingDomainTests
        {
            private DataMapper mapper;
            private SendingDomain sendingDomain;

            [SetUp]
            public void Setup()
            {
                sendingDomain = new SendingDomain();
                mapper = new DataMapper("v1");
            }

            [Test]
            public void Matches_on_status()
            {
                sendingDomain.Status = new SendingDomainStatus();
                sendingDomain.Status.DkimStatus = DkimStatus.Pending;
                mapper.ToDictionary(sendingDomain)["status"]
                    .CastAs<IDictionary<string, object>>()
                    ["dkim_status"]
                    .CastAs<string>()
                    .ShouldBe("pending");
            }

            [Test]
            public void Null_status_return_null()
            {
                sendingDomain.Status = null;
                var dictionary = mapper.ToDictionary(sendingDomain);
                dictionary
                    .ContainsKey("status")
                    .ShouldBeFalse();
            }

            [Test]
            public void Matches_on_dkim()
            {
                var publicKey = Guid.NewGuid().ToString();
                sendingDomain.Dkim = new Dkim { PublicKey = publicKey };
                mapper.ToDictionary(sendingDomain)["dkim"]
                    .CastAs<IDictionary<string, object>>()
                    ["public_key"]
                    .CastAs<string>()
                    .ShouldBe(publicKey);
            }

            [Test]
            public void Null_dkim_is_is_not_returned()
            {
                sendingDomain.Dkim = null;
                mapper.ToDictionary(sendingDomain)
                    .ContainsKey("dkim")
                    .ShouldBeFalse();
            }
        }

        [TestFixture]
        public class MetricsQueryTests
        {
            private DataMapper _mapper;
            private MetricsQuery _query;
            private string _timeFormat = "yyyy-MM-ddTHH:mm";

            [SetUp]
            public void Setup()
            {
                _query = new MetricsQuery();
                _mapper = new DataMapper("v1");
            }

            [Test]
            public void from()
            {
                var from = DateTime.Parse("2013-11-29 14:26");
                _query.From = from;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["from"].CastTo<string>(), Is.EqualTo(from.ToString(_timeFormat)));
            }

            [Test]
            public void to()
            {
                var to = DateTime.Parse("2003-08-13 12:15");
                _query.To = to;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["to"].CastTo<string>(), Is.EqualTo(to.ToString(_timeFormat)));
            }

            [Test]
            public void timezone()
            {
                var tz = Guid.NewGuid().ToString();
                _query.Timezone = tz;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["timezone"].CastTo<string>(), Is.EqualTo(tz));
            }

            [Test]
            public void precision()
            {
                var p = Guid.NewGuid().ToString();
                _query.Precision = p;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["precision"].CastTo<string>(), Is.EqualTo(p));
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void campaigns(params string[] list)
            {
                _query.Campaigns = list;
                CheckList(list, "campaigns");
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void domains(params string[] list)
            {
                _query.Domains = list;
                CheckList(list, "domains");
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void templates(params string[] list)
            {
                _query.Templates = list;
                CheckList(list, "templates");
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void sendingips(params string[] list)
            {
                _query.SendingIps = list;
                CheckList(list, "sending_ips");
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void ippools(params string[] list)
            {
                _query.IpPools = list;
                CheckList(list, "ip_pools");
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void sendingdomains(params string[] list)
            {
                _query.SendingDomains = list;
                CheckList(list, "sending_domains");
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void subaccounts(params string[] list)
            {
                _query.Subaccounts = list;
                CheckList(list, "subaccounts");
            }

            [TestCase("apple")]
            [TestCase("apple", "banana", "carrot")]
            public void metrics(params string[] list)
            {
                _query.Metrics = list;
                CheckList(list, "metrics");
            }

            private void CheckList(IList<string> list, string k)
            {
                var cat = String.Join(",", list);
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict[k].CastTo<string>(), Is.EqualTo(cat));
            }
        }

        [TestFixture]
        public class MetricsResourceQueryTests
        {
            private DataMapper _mapper;
            private MetricsResourceQuery _query;
            private string _timeFormat = "yyyy-MM-ddTHH:mm";

            [SetUp]
            public void Setup()
            {
                _query = new MetricsResourceQuery();
                _mapper = new DataMapper("v1");
            }

            [Test]
            public void from()
            {
                var from = DateTime.Parse("2013-11-29 14:26");
                _query.From = from;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["from"].CastTo<string>(), Is.EqualTo(from.ToString(_timeFormat)));
            }

            [Test]
            public void to()
            {
                var to = DateTime.Parse("2003-08-13 12:15");
                _query.To = to;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["to"].CastTo<string>(), Is.EqualTo(to.ToString(_timeFormat)));
            }

            [Test]
            public void timezone()
            {
                var tz = Guid.NewGuid().ToString();
                _query.Timezone = tz;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["timezone"].CastTo<string>(), Is.EqualTo(tz));
            }

            [Test]
            public void match()
            {
                var p = Guid.NewGuid().ToString();
                _query.Match = p;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["match"].CastTo<string>(), Is.EqualTo(p));
            }

            [Test]
            public void limit()
            {
                var r = new Random().Next();
                _query.Limit = r;
                var dict = _mapper.CatchAll(_query);
                Assert.That(dict["limit"].CastTo<int>(), Is.EqualTo(r));
            }
        }
    }
}