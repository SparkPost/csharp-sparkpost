using System.Collections.Generic;

namespace SparkPost
{
    public class PreviewTemplateResponse : Response
    {
        public PreviewTemplateResponse()
        {
            From = new Address();
            Headers = new Dictionary<string, string>();
        }

        public string Html { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
        public Address From { get; set; }
        public string ReplyTo { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}
