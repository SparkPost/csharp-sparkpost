using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Text;

namespace SparkPost.Tests
{
    public class TransmissionTests
    {
        [TestFixture]
        public class ConstructorTests
        {
            [SetUp]
            public void Setup()
            {
                transmission = new Transmission();
            }

            private Transmission transmission;

            [Test]
            public void It_should_not_be_missing_a_recipients_list()
            {
                transmission.Recipients.ShouldNotBeNull();
            }

            [Test]
            public void It_should_not_be_missing_content()
            {
                transmission.Content.ShouldNotBeNull();
            }

            [Test]
            public void It_should_not_be_missing_metadata()
            {
                transmission.Metadata.ShouldNotBeNull();
            }

            [Test]
            public void It_should_not_be_missing_substition_data()
            {
                transmission.SubstitutionData.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class ParseTests
        {
            private MailMessage mailMessage;
            private Transmission transmission;

            [SetUp]
            public void Setup()
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

            [Test]
            public void From_should_match()
            {
                transmission.Content.From.Name.ShouldBe(mailMessage.From.DisplayName);
                transmission.Content.From.Email.ShouldBe(mailMessage.From.Address);
            }

            [Test]
            public void It_should_have_three_recipients()
            {
                Assert.That(transmission.Recipients.Count, Is.EqualTo(3));
            }

            [Test]
            public void To_should_match()
            {
                var to = transmission.Recipients.SingleOrDefault(r => r.Type == RecipientType.To);
                Assert.That(to, Is.Not.Null);
                Assert.That(to.Address.Name == mailMessage.To.First().DisplayName);
                Assert.That(to.Address.Email == mailMessage.To.First().Address);
            }

            [Test]
            public void Cc_should_match()
            {
                var cc = transmission.Recipients.SingleOrDefault(r => r.Type == RecipientType.CC);
                Assert.That(cc, Is.Not.Null);
                Assert.That(cc.Address.Name == mailMessage.CC.First().DisplayName);
                Assert.That(cc.Address.Email == mailMessage.CC.First().Address);
            }

            [Test]
            public void Bcc_should_match()
            {
                var bcc = transmission.Recipients.SingleOrDefault(r => r.Type == RecipientType.BCC);
                Assert.That(bcc, Is.Not.Null);
                Assert.That(bcc.Address.Name == mailMessage.Bcc.First().DisplayName);
                Assert.That(bcc.Address.Email == mailMessage.Bcc.First().Address);
            }

            [Test]
            public void Replyto_should_match()
            {
                Assert.That(transmission.Content.ReplyTo, Is.EqualTo(mailMessage.ReplyToList.First().Address));
            }

            [Test]
            public void Subject_should_match()
            {
                Assert.That(transmission.Content.Subject, Is.EqualTo(mailMessage.Subject));
            }

            [Test]
            public void Text_body_should_match()
            {
                Assert.That(transmission.Content.Text, Is.EqualTo(mailMessage.Body));
            }

            [Test]
            public void Html_body_should_match()
            {
                mailMessage.IsBodyHtml = true;
                transmission = Transmission.Parse(mailMessage);
                Assert.That(transmission.Content.Html, Is.EqualTo(mailMessage.Body));
            }

            [Test]
            public void It_should_use_alternate_html_view()
            {
                var html = "<p>Html body</p>";
                var view = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Add(view);
                transmission = Transmission.Parse(mailMessage);

                Assert.That(transmission.Content.Html, Is.EqualTo(html));
            }

            [Test]
            public void It_should_use_alternate_text_view()
            {
                mailMessage.IsBodyHtml = true;
                var text = "Text body";
                var view = AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain);
                mailMessage.AlternateViews.Add(view);
                transmission = Transmission.Parse(mailMessage);

                Assert.That(transmission.Content.Text, Is.EqualTo(text));
            }

            [Test]
            public void It_should_use_both_alternate_views()
            {
                var text = "Alternate text";
                var html = "Alternate html";
                var view = AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain);
                mailMessage.AlternateViews.Add(view);
                view = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Add(view);
                transmission = Transmission.Parse(mailMessage);

                Assert.That(transmission.Content.Text, Is.EqualTo(text));
                Assert.That(transmission.Content.Html, Is.EqualTo(html));
            }

            [Test]
            public void It_should_copy_attachments()
            {
                var text = "This is an attachment";
                var name = "foo.txt";
                var type = "text/plain";
                var ms = new MemoryStream(Encoding.ASCII.GetBytes(text));
                mailMessage.Attachments.Add(new System.Net.Mail.Attachment(ms, name, type));
                transmission = Transmission.Parse(mailMessage);

                Assert.That(transmission.Content.Attachments.Count, Is.EqualTo(1));
                Assert.That(transmission.Content.Attachments.First().Type, Is.EqualTo(type));
                Assert.That(transmission.Content.Attachments.First().Name, Is.EqualTo(name));

                var bytes = Convert.FromBase64String(transmission.Content.Attachments.First().Data);
                var decoded = Encoding.ASCII.GetString(bytes);
                Assert.That(decoded, Is.EqualTo(text));
            }
        }
    }
}