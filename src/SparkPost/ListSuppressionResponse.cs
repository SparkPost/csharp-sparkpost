using System.Collections.Generic;

namespace SparkPost
{
    public class ListSuppressionResponse : Response
    {
        public ListSuppressionResponse()
        {
            Suppressions = new Suppression[] { };
            Links = new PageLink();
        }

        public IEnumerable<Suppression> Suppressions { get; set; }

        public PageLink Links { get; set; }

        public int TotalCount { get; set; }
    }
}