using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Text;
using Xunit;

namespace SparkPost.Tests
{
    public class TransmissionTests
    {
        public class ConstructorTests
        {
            private readonly Transmission transmission;

            public ConstructorTests()
            {
                transmission = new Transmission();
            }

            [Fact]
            public void It_should_not_be_missing_a_recipients_list()
            {
                Assert.NotNull(transmission.Recipients);
            }

            [Fact]
            public void It_should_not_be_missing_content()
            {
                Assert.NotNull(transmission.Content);
            }

            [Fact]
            public void It_should_not_be_missing_metadata()
            {
                Assert.NotNull(transmission.Metadata);
            }

            [Fact]
            public void It_should_not_be_missing_substition_data()
            {
                Assert.NotNull(transmission.SubstitutionData);
            }
        }

        public class ParseTests
        {
            private MailMessage mailMessage;
            private Transmission transmission;

            public ParseTests()
            {
                mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("jim@example.com", "Jim Example");
                mailMessage.To.Add(new MailAddress("bob@example.com", "Bob Example"));
                mailMessage.CC.Add(new MailAddress("susan@example.com", "Susan Example"));
                mailMessage.Bcc.Add(new MailAddress("richard@example.com", "Richard Example"));
                mailMessage.ReplyToList.Add(new MailAddress("rebecca@example.com", "Rebecca Example"));
                mailMessage.Body = "Unit test message";
                mailMessage.Subject = "Test ssubject";

                transmission = Transmission.Parse(mailMessage);
            }

            [Fact]
            public void From_should_match()
            {
                Assert.Equal(mailMessage.From.DisplayName, transmission.Content.From.Name);
                Assert.Equal(mailMessage.From.Address, transmission.Content.From.Email);
            }

            [Fact]
            public void It_should_have_three_recipients()
            {
                Assert.Equal(3, transmission.Recipients.Count);
            }

            [Fact]
            public void To_should_match()
            {
                var to = transmission.Recipients.SingleOrDefault(r => r.Type == RecipientType.To);
                Assert.NotNull(to);
                Assert.Equal(mailMessage.To.First().DisplayName, to.Address.Name);
                Assert.Equal(mailMessage.To.First().Address, to.Address.Email);
            }

            [Fact]
            public void Cc_should_match()
            {
                var cc = transmission.Recipients.SingleOrDefault(r => r.Type == RecipientType.CC);
                Assert.NotNull(cc);
                Assert.Equal(mailMessage.CC.First().DisplayName, cc.Address.Name);
                Assert.Equal(mailMessage.CC.First().Address, cc.Address.Email);
            }

            [Fact]
            public void Bcc_should_match()
            {
                var bcc = transmission.Recipients.SingleOrDefault(r => r.Type == RecipientType.BCC);
                Assert.NotNull(bcc);
                Assert.Equal(mailMessage.Bcc.First().DisplayName, bcc.Address.Name);
                Assert.Equal(mailMessage.Bcc.First().Address, bcc.Address.Email);
            }

            [Fact]
            public void Replyto_should_match()
            {
                Assert.Equal(mailMessage.ReplyToList.First().Address, transmission.Content.ReplyTo);
            }

            [Fact]
            public void Subject_should_match()
            {
                Assert.Equal(mailMessage.Subject, transmission.Content.Subject);
            }

            [Fact]
            public void Text_body_should_match()
            {
                Assert.Equal(mailMessage.Body, transmission.Content.Text);
            }

            [Fact]
            public void Html_body_should_match()
            {
                mailMessage.IsBodyHtml = true;
                transmission = Transmission.Parse(mailMessage);
                Assert.Equal(mailMessage.Body, transmission.Content.Html);
            }

            [Fact]
            public void It_should_use_alternate_html_view()
            {
                var html = "<p>Html body</p>";
                var view = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Add(view);
                transmission = Transmission.Parse(mailMessage);

                Assert.Equal(html, transmission.Content.Html);
            }

            [Fact]
            public void It_should_use_alternate_text_view()
            {
                mailMessage.IsBodyHtml = true;
                var text = "Text body";
                var view = AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain);
                mailMessage.AlternateViews.Add(view);
                transmission = Transmission.Parse(mailMessage);

                Assert.Equal(text, transmission.Content.Text);
            }

            [Fact]
            public void It_should_use_both_alternate_views()
            {
                var text = "Alternate text";
                var html = "Alternate html";
                var view = AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain);
                mailMessage.AlternateViews.Add(view);
                view = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Add(view);
                transmission = Transmission.Parse(mailMessage);

                Assert.Equal(text, transmission.Content.Text);
                Assert.Equal(html, transmission.Content.Html);
            }

            [Fact]
            public void It_should_copy_attachments()
            {
                var text = "This is an attachment";
                var name = "foo.txt";
                var type = "text/plain";
                var ms = new MemoryStream(Encoding.ASCII.GetBytes(text));
                mailMessage.Attachments.Add(new System.Net.Mail.Attachment(ms, name, type));
                transmission = Transmission.Parse(mailMessage);

                Assert.Equal(1, transmission.Content.Attachments.Count);
                Assert.Equal(type, transmission.Content.Attachments.First().Type);
                Assert.Equal(name, transmission.Content.Attachments.First().Name);

                var bytes = Convert.FromBase64String(transmission.Content.Attachments.First().Data);
                var decoded = Encoding.ASCII.GetString(bytes);
                Assert.Equal(text, decoded);
            }
        }
    }
}