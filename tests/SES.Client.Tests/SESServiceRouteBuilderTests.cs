using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SES.Client.Tests
{
    public class SESServiceRouteBuilderTests
    {
        [Fact]
        public void FetchUriTest()
        {
            const string expecteduritext = "http://myendpoint/queues/SES.Client.Tests.SESServiceRouteBuilderTests/fetch/1?count=50";
            string[] endpointVariations = { "http://myendpoint", "http://myendpoint/", "http://myendpoint\\" };
            foreach (var e in endpointVariations)
            {
                var options = new SubscriptionOptions()
                {
                    Endpoint = e,
                    PreferredBatchSize = 50,
                };
                var result = SESServiceRouteBuilder.MakeFetchUri<SESServiceRouteBuilderTests>(options, 1);
                Assert.Equal(expecteduritext, result.ToString());
            }
        }

        [Fact]
        public void PublishUriTest()
        {
            const string expecteduritext = "http://myendpoint/queues/SES.Client.Tests.SESServiceRouteBuilderTests/publish";
            string[] endpointVariations = { "http://myendpoint", "http://myendpoint/", "http://myendpoint\\" };
            foreach (var e in endpointVariations)
            {
                var options = new PublisherOptions()
                {
                    Endpoint = e
                };
                var result = SESServiceRouteBuilder.MakePublishUri<SESServiceRouteBuilderTests>(options);
                Assert.Equal(expecteduritext, result.ToString());
            }
        }
    }
}
