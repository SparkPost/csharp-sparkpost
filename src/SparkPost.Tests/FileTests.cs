using System;
using System.Text;
using Xunit;

namespace SparkPost.Tests
{
    public class FileTests
    {
        [Fact]
        public void It_should_create_correct_type()
        {
            byte[] content = null;
            Assert.IsType<Attachment>(File.Create<Attachment>(content));
            Assert.IsType<InlineImage>(File.Create<InlineImage>(content));
        }

        [Theory]
        [InlineData("This is some test data.")]
        [InlineData("This is some other data.")]
        public void It_should_encode_data_correctly(string s)
        {
            var b = GetBytes(s);
            var attach = File.Create<Attachment>(b);
            Assert.Equal(EncodeString(s), attach.Data);
        }

        [Theory]
        [InlineData("foo.png", "image/png")]
        [InlineData("foo.txt", "text/plain")]
        [InlineData("sf", "application/octet-stream")]
        [InlineData("", "application/octet-stream")]
        public void It_should_set_name_and_type_correctly(string filename, string mimeType)
        {
            var b = GetBytes("Some Test Data");
            var attach = File.Create<Attachment>(b, filename);
            Assert.Equal(filename, attach.Name);
            Assert.Equal(mimeType, attach.Type);
        }

        private byte[] GetBytes(string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        private string EncodeString(string input)
        {
            return Convert.ToBase64String(GetBytes(input));
        }

    }
}
