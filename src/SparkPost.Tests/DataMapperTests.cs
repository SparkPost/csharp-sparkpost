using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SparkPost.Tests
{
    public class DataMapperTests
    {
        public class RecipientMappingTests
        {
            private readonly DataMapper mapper;
            private readonly Recipient recipient;


            public RecipientMappingTests()
            {
                recipient = new Recipient();
                mapper = new DataMapper("v1");
            }
            
            [Fact]
            public void address()
            {
                var value = Guid.NewGuid().ToString();
                recipient.Address.Email = value;
                var x = mapper.ToDictionary(recipient)["address"];
                Assert.Equal(value, mapper.ToDictionary(recipient)
                    ["address"]
                    .CastAs<IDictionary<string, object>>()
                    ["email"]);
            }

            [Fact]
            public void return_path()
            {
                var value = Guid.NewGuid().ToString();
                recipient.ReturnPath = value;
                Assert.Equal(value, mapper.ToDictionary(recipient)["return_path"]);
            }

            [Fact]
            public void tags()
            {
                var tag1 = Guid.NewGuid().ToString();
                var tag2 = Guid.NewGuid().ToString();
                recipient.Tags.Add(tag1);
                recipient.Tags.Add(tag2);
                var theTags = mapper.ToDictionary(recipient)["tags"];
                Assert.Equal(2, theTags.CastAs<IEnumerable<object>>().Count());
                Assert.Contains(tag1, mapper.ToDictionary(recipient)["tags"]
                    .CastAs<IEnumerable<object>>());
                Assert.Contains(tag2, mapper.ToDictionary(recipient)["tags"]
                    .CastAs<IEnumerable<object>>());
            }

            [Fact]
            public void empty_tags_are_ignored()
            {
                Assert.DoesNotContain("tags", mapper.ToDictionary(recipient).Keys);
            }

            [Fact]
            public void metadata()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                recipient.Metadata[key] = value;
                Assert.Equal(value, mapper.ToDictionary(recipient)["metadata"]
                    .CastAs<IDictionary<string, object>>()[key]);
            }

            [Fact]
            public void do_not_include_empty_metadata()
            {
                Assert.DoesNotContain("metadata", mapper.ToDictionary(recipient).Keys);
            }

            [Fact]
            public void substitution_data()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                recipient.SubstitutionData[key] = value;
                Assert.Equal(value, mapper.ToDictionary(recipient)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key]);
            }

            [Fact]
            public void do_not_include_empty_substitution_data()
            {
                Assert.DoesNotContain("substitution_data", mapper.ToDictionary(recipient).Keys);
            }

            [Fact]
            public void do_not_alter_the_keys_passed_to_substitution_data()
            {
                var key = "TEST";
                var value = Guid.NewGuid().ToString();
                recipient.SubstitutionData[key] = value;
                Assert.Equal(value, mapper.ToDictionary(recipient)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key]);
            }

            [Fact]
            public void The_type_should_be_ignored()
            {
                recipient.Type = RecipientType.CC;
                Assert.DoesNotContain("type", mapper.ToDictionary(recipient).Keys);
            }
        }


        public class AddressMappingTests
        {
            private readonly Address address;
            private readonly DataMapper mapper;

            public AddressMappingTests()
            {
                address = new Address();
                mapper = new DataMapper("v1");
            }

            [Fact]
            public void email()
            {
                var value = Guid.NewGuid().ToString();
                address.Email = value;
                Assert.Equal(value, mapper.ToDictionary(address)["email"]);
            }

            [Fact]
            public void name()
            {
                var value = Guid.NewGuid().ToString();
                address.Name = value;
                Assert.Equal(value, mapper.ToDictionary(address)["name"]);
            }

            [Fact]
            public void header_to()
            {
                var value = Guid.NewGuid().ToString();
                address.HeaderTo = value;
                Assert.Equal(value, mapper.ToDictionary(address)["header_to"]);
            }

            [Fact]
            public void header_to_is_not_returned_if_empty()
            {
                Assert.DoesNotContain("header_to", mapper.ToDictionary(address).Keys);
            }
        }

        public class TransmissionMappingTests
        {
            private readonly Transmission transmission;
            private readonly DataMapper mapper;

            public TransmissionMappingTests()
            {
                transmission = new Transmission();
                mapper = new DataMapper("v1");
            }

            [Fact]
            public void It_should_set_the_content_dictionary()
            {
                var email = Guid.NewGuid().ToString();
                transmission.Content.From = new Address { Email = email };
                Assert.Equal(email, mapper.ToDictionary(transmission)["content"]
                    .CastAs<IDictionary<string, object>>()["from"]
                    .CastAs<IDictionary<string, object>>()["email"]);
            }

            [Fact]
            public void It_should_set_the_recipients()
            {
                var recipient1 = new Recipient { Address = new Address { Email = Guid.NewGuid().ToString() } };
                var recipient2 = new Recipient { Address = new Address { Email = Guid.NewGuid().ToString() } };

                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                var result = mapper.ToDictionary(transmission)["recipients"] as IEnumerable<IDictionary<string, object>>;
                Assert.Equal(2, result.Count());
                Assert.Equal(recipient1.Address.Email, result.ToList()[0]["address"]
                    .CastAs<IDictionary<string, object>>()
                    ["email"]);
                Assert.Equal(recipient2.Address.Email, result.ToList()[1]["address"]
                    .CastAs<IDictionary<string, object>>()
                    ["email"]);
            }

            [Fact]
            public void It_should_set_the_recipients_to_a_list_id_if_a_list_id_is_provided()
            {
                var listId = Guid.NewGuid().ToString();
                transmission.ListId = listId;

                var result = mapper.ToDictionary(transmission)["recipients"] as IDictionary<string, object>;
                Assert.Equal(listId, result["list_id"]);
            }

            [Fact]
            public void campaign_id()
            {
                var value = Guid.NewGuid().ToString();
                transmission.CampaignId = value;
                Assert.Equal(value, mapper.ToDictionary(transmission)["campaign_id"]);
            }

            [Fact]
            public void description()
            {
                var value = Guid.NewGuid().ToString();
                transmission.Description = value;
                Assert.Equal(value, mapper.ToDictionary(transmission)["description"]);
            }

            [Fact]
            public void return_path()
            {
                var value = Guid.NewGuid().ToString();
                transmission.ReturnPath = value;
                Assert.Equal(value, mapper.ToDictionary(transmission)["return_path"]);
            }

            [Fact]
            public void do_not_send_the_return_path_if_it_is_not_provided()
            {
                Assert.DoesNotContain("return_path", mapper.ToDictionary(transmission).Keys);
            }

            [Fact]
            public void metadata()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                transmission.Metadata[key] = value;
                Assert.Equal(value, mapper.ToDictionary(transmission)["metadata"]
                    .CastAs<IDictionary<string, object>>()[key]);
            }

            [Fact]
            public void do_not_alter_the_keys_passed_to_metadata()
            {
                var key = "TEST";
                var value = Guid.NewGuid().ToString();
                transmission.Metadata[key] = value;
                Assert.Equal(value, mapper.ToDictionary(transmission)["metadata"]
                    .CastAs<IDictionary<string, object>>()[key]);
            }

            [Fact]
            public void do_not_include_empty_metadata()
            {
                Assert.DoesNotContain("metadata", mapper.ToDictionary(transmission).Keys);
            }

            [Fact]
            public void substitution_data()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                transmission.SubstitutionData[key] = value;
                Assert.Equal(value, mapper.ToDictionary(transmission)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key]);
            }

            [Fact]
            public void do_not_include_empty_substitution_data()
            {
                Assert.DoesNotContain("substitution_data", mapper.ToDictionary(transmission).Keys);
            }

            [Fact]
            public void do_not_alter_the_keys_passed_to_substitution_data()
            {
                var key = "TEST";
                var value = Guid.NewGuid().ToString();
                transmission.SubstitutionData[key] = value;
                Assert.Equal(value, mapper.ToDictionary(transmission)["substitution_data"]
                    .CastAs<IDictionary<string, object>>()[key]);
            }

            [Fact]
            public void options()
            {
                transmission.Options.ClickTracking = true;
                Assert.True((bool)mapper.ToDictionary(transmission)["options"]
                    .CastAs<IDictionary<string, object>>()["click_tracking"]);

                transmission.Options.ClickTracking = false;
                Assert.False((bool)mapper.ToDictionary(transmission)["options"]
                    .CastAs<IDictionary<string, object>>()["click_tracking"]);

                transmission.Options.InlineCss = true;
                Assert.True((bool)mapper.ToDictionary(transmission)["options"]
                    .CastAs<IDictionary<string, object>>()["inline_css"]);
            }
        }


        public class MappingCcFields
        {
            private readonly Transmission transmission;
            private readonly DataMapper mapper;

            public MappingCcFields()
            {
                transmission = new Transmission();
                mapper = new DataMapper("v1");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void It_should_set_the_CC_Header_for_only_the_cc_emails(bool useTo)
            {
                var recipient1 = new Recipient { Type = RecipientType.CC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                var recipient2 = new Recipient { Type = RecipientType.BCC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                var recipient3 = new Recipient { Type = RecipientType.CC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                var recipient4 = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address { Email = Guid.NewGuid().ToString() } }
                        : new Recipient();

                transmission.Recipients = new List<Recipient> { recipient1, recipient2, recipient3, recipient4 };

                var cc = mapper.ToDictionary(transmission)["content"]
                    .CastAs<IDictionary<string, object>>()["headers"]
                    .CastAs<IDictionary<string, string>>()["CC"];

                Assert.Equal(recipient1.Address.Email + ", " + recipient3.Address.Email, cc);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void It_should_not_overwrite_any_existing_headers(bool useTo)
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();

                var recipient1 = new Recipient { Type = RecipientType.CC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                var recipient2 = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address() }
                        : new Recipient();
                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                transmission.Content.Headers[key] = value;

                Assert.Equal(value, mapper.ToDictionary(transmission)["content"]
                   .CastAs<IDictionary<string, object>>()["headers"]
                   .CastAs<IDictionary<string, string>>()[key]);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void It_should_not_set_the_cc_if_there_are_no_cc_emails(bool useTo)
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();

                var recipient1 = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address { Email = Guid.NewGuid().ToString() } }
                        : new Recipient();
                var recipient2 = new Recipient { Type = RecipientType.BCC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                var recipient3 = new Recipient { Type = RecipientType.BCC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                transmission.Recipients = new List<Recipient> { recipient1, recipient2, recipient3 };

                transmission.Content.Headers[key] = value;

                Assert.False(mapper.ToDictionary(transmission)["content"]
                   .CastAs<IDictionary<string, object>>()["headers"]
                   .CastAs<IDictionary<string, string>>().ContainsKey("CC"));
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void It_should_not_set_a_header_value_if_there_are_no_ccs(bool useTo)
            {
                var recipient1 = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address { Email = Guid.NewGuid().ToString() } }
                        : new Recipient();
                var recipient2 = new Recipient { Type = RecipientType.BCC, Address = new Address { Email = Guid.NewGuid().ToString() } };
                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                Assert.False(mapper.ToDictionary(transmission)["content"]
                   .CastAs<IDictionary<string, object>>()
                   .ContainsKey("headers"));
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void It_should_ignore_empty_ccs(bool useTo)
            {
                var recipient1 = new Recipient { Type = RecipientType.CC, Address = new Address { Email = "" } };
                var recipient2 = new Recipient { Type = RecipientType.CC, Address = new Address { Email = null } };
                var recipient3 = new Recipient { Type = RecipientType.CC, Address = new Address { Email = " " } };
                var toRecipient = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address() }
                        : new Recipient();
                transmission.Recipients = new List<Recipient> { recipient1, recipient2, recipient3, toRecipient };

                Assert.False(mapper.ToDictionary(transmission)["content"]
                   .CastAs<IDictionary<string, object>>().ContainsKey("headers"));
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void It_should_ignore_any_cc_recipients_with_no_address(bool useTo)
            {
                var recipient1 = new Recipient { Type = RecipientType.CC, Address = null };
                var toRecipient = useTo
                        ? new Recipient { Type = RecipientType.To, Address = new Address() }
                        : new Recipient();
                transmission.Recipients = new List<Recipient> { recipient1, toRecipient };

                Assert.False(mapper.ToDictionary(transmission)
                   ["content"]
                   .CastAs<IDictionary<string, object>>()
                   .ContainsKey("headers"));
            }

            [Fact]
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
                    Assert.Equal(toName, address["name"]);
                    Assert.Equal(toEmail, address["header_to"]);
                }
            }

            [Theory]
            [InlineData("Bob Jones", "bob@jones.com", "Bob Jones <bob@jones.com>")]
            [InlineData(null, "bob@jones.com", "bob@jones.com")]
            [InlineData("", "bob@jones.com", "bob@jones.com")]
            [InlineData("Jones, Bob", "bob@jones.com", "\"Jones, Bob\" <bob@jones.com>")]
            public void It_should_format_addresses_correctly(string name, string address, string result)
            {
                var recipient1 = new Recipient { Type = RecipientType.To, Address = new Address() };
                var recipient2 = new Recipient { Type = RecipientType.CC, Address = new Address(address, name) };
                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                Assert.Equal(result, mapper.ToDictionary(transmission)["content"]
                    .CastAs<IDictionary<string, object>>()["headers"]
                    .CastAs<IDictionary<string, string>>()["CC"]);
            }

            [Theory]
            [InlineData("Jones, Bob", "bob@jones.com ", "\"Jones, Bob\" <bob@jones.com>")]
            [InlineData("Jones, Bob", " bob@jones.com", "\"Jones, Bob\" <bob@jones.com>")]
            public void It_should_handle_white_space_in_the_email(string name, string address, string result)
            {
                var recipient1 = new Recipient { Type = RecipientType.To, Address = new Address() };
                var recipient2 = new Recipient { Type = RecipientType.CC, Address = new Address(address, name) };
                transmission.Recipients = new List<Recipient> { recipient1, recipient2 };

                Assert.Equal(result, mapper.ToDictionary(transmission)["content"]
                    .CastAs<IDictionary<string, object>>()["headers"]
                    .CastAs<IDictionary<string, string>>()["CC"]);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            public void It_should_use_new_or_legacy_handling(int numOfTos)
            {
                var ccAddress = Guid.NewGuid().ToString();
                transmission.Recipients.Add(new Recipient { Type = RecipientType.CC, Address = new Address(ccAddress) });

                for (int i = 0; i < numOfTos; ++i)
                {
                    transmission.Recipients.Add(new Recipient { Type = RecipientType.To, Address = new Address("bob@example.com") });
                }

                var ccHeader = mapper.ToDictionary(transmission)["content"]
                    .CastAs<IDictionary<string, object>>()["headers"]
                    .CastAs<IDictionary<string, string>>()["CC"];

                if (numOfTos == 1)
                {
                    Assert.Equal(ccAddress, ccHeader);
                }
                else
                {
                    Assert.Equal($"<{ccAddress}>", ccHeader);
                }
            }

            [Fact]
            public void It_should_use_legacy_handling_if_to_address_is_null()
            {
                var ccAddress = Guid.NewGuid().ToString();
                transmission.Recipients.Add(new Recipient { Type = RecipientType.CC, Address = new Address(ccAddress) });
                transmission.Recipients.Add(new Recipient { Type = RecipientType.To, Address = null });

                var ccHeader = mapper.ToDictionary(transmission)["content"]
                    .CastAs<IDictionary<string, object>>()["headers"]
                    .CastAs<IDictionary<string, string>>()["CC"];

                Assert.Equal($"<{ccAddress}>", ccHeader);
            }
        }

        public class ContentMappingTests
        {
            private readonly Content content;
            private readonly DataMapper mapper;

            public ContentMappingTests()
            {
                content = new Content();
                mapper = new DataMapper("v1");
            }
            
            [Fact]
            public void from()
            {
                content.From.Email = Guid.NewGuid().ToString();
                content.From.HeaderTo = Guid.NewGuid().ToString();
                content.From.Name = Guid.NewGuid().ToString();

                var result = mapper.ToDictionary(content)["from"].CastAs<IDictionary<string, object>>();

                Assert.Equal(content.From.Email, result["email"]);
                Assert.Equal(content.From.HeaderTo, result["header_to"]);
                Assert.Equal(content.From.Name, result["name"]);
            }

            [Fact]
            public void subject()
            {
                var value = Guid.NewGuid().ToString();
                content.Subject = value;
                Assert.Equal(value, mapper.ToDictionary(content)["subject"]);
            }

            [Fact]
            public void text()
            {
                var value = Guid.NewGuid().ToString();
                content.Text = value;
                Assert.Equal(value, mapper.ToDictionary(content)["text"]);
            }

            [Fact]
            public void template_id()
            {
                var value = Guid.NewGuid().ToString();
                content.TemplateId = value;
                Assert.Equal(value, mapper.ToDictionary(content)["template_id"]);
            }

            [Fact]
            public void html()
            {
                var value = Guid.NewGuid().ToString();
                content.Html = value;
                Assert.Equal(value, mapper.ToDictionary(content)["html"]);
            }

            [Fact]
            public void reply_to()
            {
                var value = Guid.NewGuid().ToString();
                content.ReplyTo = value;
                Assert.Equal(value, mapper.ToDictionary(content)["reply_to"]);
            }

            [Fact]
            public void headers()
            {
                var key = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                content.Headers[key] = value;
                Assert.Equal(value, mapper.ToDictionary(content)["headers"]
                    .CastAs<IDictionary<string, string>>()[key]);
            }

            [Fact]
            public void do_not_include_empty_headers()
            {
                Assert.DoesNotContain("headers", mapper.ToDictionary(content).Keys);
            }

            [Fact]
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

                Assert.Equal(2, names.Count());
                Assert.Contains(firstName, names);
                Assert.Contains(secondName, names);
            }

            [Fact]
            public void no_attachments_should_not_include_the_attachments_block()
            {
                Assert.DoesNotContain("attachments", mapper.ToDictionary(content).Keys);
            }

            [Fact]
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

                Assert.Equal(2, names.Count());
                Assert.Contains(firstName, names);
                Assert.Contains(secondName, names);
            }

            [Fact]
            public void no_inline_images_should_not_include_the_inline_images_block()
            {
                Assert.DoesNotContain("inline_images", mapper.ToDictionary(content).Keys);
            }
        }

        public class OptionsMappingTests
        {
            private readonly DataMapper mapper;
            private readonly Options options;

            public OptionsMappingTests()
            {
                options = new Options();
                mapper = new DataMapper("v1");
            }
            
            [Fact]
            public void It_should_default_to_returning_null()
            {
                Assert.Null(mapper.ToDictionary(options));
            }

            [Fact]
            public void open_tracking()
            {
                options.OpenTracking = true;
                Assert.True((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["open_tracking"]);

                options.OpenTracking = false;
                Assert.False((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["open_tracking"]);
            }

            [Fact]
            public void click_tracking()
            {
                options.ClickTracking = true;
                Assert.True((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["click_tracking"]);

                options.ClickTracking = false;
                Assert.False((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["click_tracking"]);
            }

            [Fact]
            public void transactional()
            {
                options.Transactional = true;
                Assert.True((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["transactional"]);

                options.Transactional = false;
                Assert.False((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["transactional"]);
            }

            [Fact]
            public void sandbox()
            {
                options.Sandbox = true;
                Assert.True((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["sandbox"]);

                options.Sandbox = false;
                Assert.False((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["sandbox"]);
            }

            [Fact]
            public void skip_suppression()
            {
                options.SkipSuppression = true;
                Assert.True((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["skip_suppression"]);

                options.SkipSuppression = false;
                Assert.False((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["skip_suppression"]);
            }

            [Fact]
            public void start_time()
            {
                var startTime = "2015-02-11T08:00:00-04:00";
                options.StartTime = DateTimeOffset.Parse(startTime);
                Assert.Equal(startTime, mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["start_time"]);

                startTime = "2015-02-11T08:00:00-14:00";
                options.StartTime = DateTimeOffset.Parse(startTime);
                Assert.Equal(startTime, mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["start_time"]);
            }

            [Fact]
            public void hide_start_time_if_it_is_missing()
            {
                options.OpenTracking = true;
                Assert.DoesNotContain("start_time", mapper.ToDictionary(options)
                    .CastAs<IDictionary<string, object>>().Keys);
            }

            [Fact]
            public void inline_css()
            {
                options.InlineCss = true;
                Assert.True((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["inline_css"]);

                options.InlineCss = false;
                Assert.False((bool)mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["inline_css"]);
            }

            [Fact]
            public void ip_pool()
            {
                var ipPool = Guid.NewGuid().ToString();
                options.IpPool = ipPool;
                Assert.Equal(ipPool, mapper.ToDictionary(options).CastAs<IDictionary<string, object>>()
                    ["ip_pool"]);
            }
        }

        public class FileMappingTests
        {
            private File file;
            private DataMapper mapper;

            public FileMappingTests()
            {
                file = new Attachment();
                mapper = new DataMapper("v1");
            }

            [Fact]
            public void name()
            {
                var value = Guid.NewGuid().ToString();
                file.Name = value;
                Assert.Equal(value, mapper.ToDictionary(file)["name"]);
            }

            [Fact]
            public void type()
            {
                var value = Guid.NewGuid().ToString();
                file.Type = value;
                Assert.Equal(value, mapper.ToDictionary(file)["type"]);
            }

            [Fact]
            public void data()
            {
                var value = Guid.NewGuid().ToString();
                file.Data = value;
                Assert.Equal(value, mapper.ToDictionary(file)["data"]);
            }
        }

        public class WebhookTests
        {
            private DataMapper dataMapper;

            public WebhookTests()
            {
                dataMapper = new DataMapper();
            }

            [Fact]
            public void Name()
            {
                var webhook = new Webhook { Name = Guid.NewGuid().ToString() };
                Assert.Equal(webhook.Name, dataMapper.ToDictionary(webhook)["name"]);
            }

            [Fact]
            public void Target()
            {
                var webhook = new Webhook { Target = Guid.NewGuid().ToString() };
                Assert.Equal(webhook.Target, dataMapper.ToDictionary(webhook)["target"]);
            }

            [Fact]
            public void AuthType()
            {
                var webhook = new Webhook { AuthType = Guid.NewGuid().ToString() };
                Assert.Equal(webhook.AuthType, dataMapper.ToDictionary(webhook)["auth_type"]);
            }

            [Fact]
            public void AuthToken()
            {
                var webhook = new Webhook { AuthToken = Guid.NewGuid().ToString() };
                Assert.Equal(webhook.AuthToken, dataMapper.ToDictionary(webhook)["auth_token"]);
            }

            [Fact]
            public void Events()
            {
                var first = Guid.NewGuid().ToString();
                var second = Guid.NewGuid().ToString();

                var webhook = new Webhook();
                webhook.Events.Add(first);
                webhook.Events.Add(second);

                var dictionary = dataMapper.ToDictionary(webhook);
                var events = dictionary["events"] as IEnumerable<object>;
                Assert.Equal(2, events.Count());
                Assert.Contains(first, events);
                Assert.Contains(second, events);
            }

            [Fact]
            public void AuthRequestDetails()
            {
                var webhook = new Webhook
                {
                    AuthRequestDetails = new
                    {
                        Url = "https://oauth.myurl.com/tokens",
                        Body = new { ClientId = "<oauth client id>", ClientSecret = "<oauth client secret>" }
                    }
                };

                var dictionary = dataMapper.ToDictionary(webhook);
                var authRequestDetails = dictionary["auth_request_details"].CastAs<IDictionary<string, object>>();
                Assert.Equal("https://oauth.myurl.com/tokens", authRequestDetails["url"]);

                Assert.Equal("<oauth client id>", authRequestDetails["body"]
                    .CastAs<IDictionary<string, object>>()
                    ["client_id"]);

                Assert.Equal("<oauth client secret>", authRequestDetails["body"]
                    .CastAs<IDictionary<string, object>>()
                    ["client_secret"]);
            }

            [Fact]
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
                Assert.Equal("<oauth token>", authRequestDetails["access_token"]);
                Assert.Equal(3600, authRequestDetails["expires_in"]);
            }
        }


        public class SubaccountTests
        {
            private readonly DataMapper dataMapper;

            public SubaccountTests()
            {
                dataMapper = new DataMapper();
            }

            [Fact]
            public void Id()
            {
                var subaccount = new Subaccount { Id = 432 };
                Assert.Equal(subaccount.Id, dataMapper.ToDictionary(subaccount)["id"]);
            }

            [Fact]
            public void Name()
            {
                var subaccount = new Subaccount { Name = Guid.NewGuid().ToString() };
                Assert.Equal(subaccount.Name, dataMapper.ToDictionary(subaccount)["name"]);
            }

            [Fact]
            public void Status()
            {
                var subaccount = new Subaccount { Status = SubaccountStatus.Terminated };
                Assert.Equal(SubaccountStatus.Terminated.ToString().ToLowerInvariant(), dataMapper.ToDictionary(subaccount)["status"]);
            }

            [Fact]
            public void IpPool()
            {
                var subaccount = new Subaccount { IpPool = Guid.NewGuid().ToString() };
                Assert.Equal(subaccount.IpPool, dataMapper.ToDictionary(subaccount)["ip_pool"]);
            }

            [Fact]
            public void ComplianceStatus()
            {
                var subaccount = new Subaccount { ComplianceStatus = Guid.NewGuid().ToString() };
                Assert.Equal(subaccount.ComplianceStatus, dataMapper.ToDictionary(subaccount)["compliance_status"]);
            }
        }

        public class AnythingTests
        {
            [Fact]
            public void It_should_map_anything_using_our_conventions()
            {
                var dataMapper = new DataMapper();

                var dateTime = new DateTime(2016, 1, 2, 3, 4, 5);

                var result = dataMapper.CatchAll(new { FirstName = "Test1", LastName = "Test2", TheDate = dateTime });

                Assert.Equal("Test1", result["first_name"]);
                Assert.Equal("Test2", result["last_name"]);
                Assert.Equal("2016-01-02T03:04", ((string)result["the_date"]).Substring(0, 16));
            }
        }

        public class RelayWebhookTests
        {
            private readonly RelayWebhook relayWebhook;
            private readonly DataMapper mapper;

            public RelayWebhookTests()
            {
                relayWebhook = new RelayWebhook();
                mapper = new DataMapper("v1");
            }

            [Fact]
            public void name()
            {
                var value = Guid.NewGuid().ToString();
                relayWebhook.Name = value;
                Assert.Equal(value, mapper.ToDictionary(relayWebhook)["name"]);
            }

            [Fact]
            public void match_domain()
            {
                var value = Guid.NewGuid().ToString();
                relayWebhook.Match = new RelayWebhookMatch { Domain = value };
                Assert.Equal(value, mapper.ToDictionary(relayWebhook)["match"]
                    .CastAs<IDictionary<string, object>>()["domain"]);
            }

            [Fact]
            public void match_protocol()
            {
                var value = Guid.NewGuid().ToString();
                relayWebhook.Match = new RelayWebhookMatch { Protocol = value };
                Assert.Equal(value, mapper.ToDictionary(relayWebhook)["match"]
                    .CastAs<IDictionary<string, object>>()["protocol"]);
            }
        }

        public class SendingDomainTests
        {
            private readonly DataMapper mapper;
            private readonly SendingDomain sendingDomain;

            public SendingDomainTests()
            {
                sendingDomain = new SendingDomain();
                mapper = new DataMapper("v1");
            }

            [Fact]
            public void Matches_on_status()
            {
                sendingDomain.Status = new SendingDomainStatus();
                sendingDomain.Status.DkimStatus = DkimStatus.Pending;

                Assert.Equal("pending", mapper.ToDictionary(sendingDomain)["status"]
                    .CastAs<IDictionary<string, object>>()
                    ["dkim_status"]);
            }

            [Fact]
            public void Null_status_return_null()
            {
                sendingDomain.Status = null;
                var dictionary = mapper.ToDictionary(sendingDomain);
                Assert.False(dictionary.ContainsKey("status"));
            }

            [Fact]
            public void Matches_on_dkim()
            {
                var publicKey = Guid.NewGuid().ToString();
                sendingDomain.Dkim = new Dkim { PublicKey = publicKey };
                Assert.Equal(publicKey, mapper.ToDictionary(sendingDomain)["dkim"]
                                              .CastAs<IDictionary<string, object>>()
                                              ["public_key"]);
            }

            [Fact]
            public void Null_dkim_is_is_not_returned()
            {
                sendingDomain.Dkim = null;
                Assert.False(mapper.ToDictionary(sendingDomain)
                    .ContainsKey("dkim"));
            }
        }

        public class MetricsQueryTests
        {
            private readonly DataMapper _mapper;
            private readonly MetricsQuery _query;
            private string _timeFormat = "yyyy-MM-ddTHH:mm";

            public MetricsQueryTests()
            {
                _query = new MetricsQuery();
                _mapper = new DataMapper("v1");
            }

            [Fact]
            public void from()
            {
                var from = DateTime.Parse("2013-11-29 14:26");
                _query.From = from;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(from.ToString(_timeFormat), dict["from"]);
            }

            [Fact]
            public void to()
            {
                var to = DateTime.Parse("2003-08-13 12:15");
                _query.To = to;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(to.ToString(_timeFormat), dict["to"]);
            }

            [Fact]
            public void timezone()
            {
                var tz = Guid.NewGuid().ToString();
                _query.Timezone = tz;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(tz, dict["timezone"]);
            }

            [Fact]
            public void precision()
            {
                var p = Guid.NewGuid().ToString();
                _query.Precision = p;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(p, dict["precision"]);
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void campaigns(params string[] list)
            {
                _query.Campaigns = list;
                CheckList(list, "campaigns");
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void domains(params string[] list)
            {
                _query.Domains = list;
                CheckList(list, "domains");
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void templates(params string[] list)
            {
                _query.Templates = list;
                CheckList(list, "templates");
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void sendingips(params string[] list)
            {
                _query.SendingIps = list;
                CheckList(list, "sending_ips");
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void ippools(params string[] list)
            {
                _query.IpPools = list;
                CheckList(list, "ip_pools");
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void sendingdomains(params string[] list)
            {
                _query.SendingDomains = list;
                CheckList(list, "sending_domains");
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void subaccounts(params string[] list)
            {
                _query.Subaccounts = list;
                CheckList(list, "subaccounts");
            }

            [Theory]
            [InlineData("apple")]
            [InlineData("apple", "banana", "carrot")]
            public void metrics(params string[] list)
            {
                _query.Metrics = list;
                CheckList(list, "metrics");
            }

            private void CheckList(IList<string> list, string k)
            {
                var cat = String.Join(",", list);
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(cat, dict[k]);
            }
        }

        public class MetricsResourceQueryTests
        {
            private DataMapper _mapper;
            private MetricsResourceQuery _query;
            private string _timeFormat = "yyyy-MM-ddTHH:mm";

            public MetricsResourceQueryTests()
            {
                _query = new MetricsResourceQuery();
                _mapper = new DataMapper("v1");
            }

            [Fact]
            public void from()
            {
                var from = DateTime.Parse("2013-11-29 14:26");
                _query.From = from;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(from.ToString(_timeFormat), dict["from"]);
            }

            [Fact]
            public void to()
            {
                var to = DateTime.Parse("2003-08-13 12:15");
                _query.To = to;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(to.ToString(_timeFormat), dict["to"]);
            }

            [Fact]
            public void timezone()
            {
                var tz = Guid.NewGuid().ToString();
                _query.Timezone = tz;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(tz, dict["timezone"]);
            }

            [Fact]
            public void match()
            {
                var p = Guid.NewGuid().ToString();
                _query.Match = p;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(p, dict["match"]);
            }

            [Fact]
            public void limit()
            {
                var r = new Random().Next();
                _query.Limit = r;
                var dict = _mapper.CatchAll(_query);
                Assert.Equal(r, dict["limit"]);
            }
        }
    }
}