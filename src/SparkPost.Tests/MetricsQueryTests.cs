using System.Collections.Generic;
using Xunit;

namespace SparkPost.Tests
{
    public class MetricsQueryTests
    {
        private MetricsQuery _query;

        public MetricsQueryTests()
        {
            _query = new MetricsQuery();
        }

        private void Check(IList<string> list)
        {
            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public void It_should_have_a_default_campaigns_list() => Check(_query.Campaigns);

        [Fact]
        public void It_should_have_a_default_domains_list() => Check(_query.Domains);

        [Fact]
        public void It_should_have_a_default_metrics_list() => Check(_query.Metrics);

        [Fact]
        public void It_should_have_a_default_templates_list() => Check(_query.Templates);

        [Fact]
        public void It_should_have_a_default_sending_ips_list() => Check(_query.SendingIps);

        [Fact]
        public void It_should_have_a_default_ip_pools_list() => Check(_query.IpPools);

        [Fact]
        public void It_should_have_a_default_sending_domains_list() => Check(_query.SendingDomains);

        [Fact]
        public void It_should_have_a_default_subaccounts_list() => Check(_query.Subaccounts);
    }
}
