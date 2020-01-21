using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SES.Client.Tests
{
    public class HttpClientFactoryTests
    {
        [Fact]
        public void ProxySettingsAppliedCorrectlyTest()
        {
            var expectedProxyInstance = System.Net.WebRequest.GetSystemWebProxy();

            using (var i1 = HttpClientFactory.CreateHandler(false, new System.Net.WebProxy()))
            {
                Assert.False(i1.UseProxy);
                Assert.Null(i1.Proxy);
                Assert.True(i1.AutomaticDecompression.HasFlag(System.Net.DecompressionMethods.GZip) && i1.AutomaticDecompression.HasFlag(System.Net.DecompressionMethods.Deflate));

            }
            using (var i2 = HttpClientFactory.CreateHandler(true, null))
            {
                Assert.True(i2.UseProxy);
                Assert.Equal(expectedProxyInstance,i2.Proxy);
                Assert.True(i2.AutomaticDecompression.HasFlag(System.Net.DecompressionMethods.GZip) && i2.AutomaticDecompression.HasFlag(System.Net.DecompressionMethods.Deflate));
            }
            var customProxy = new System.Net.WebProxy();
            using (var i3 = HttpClientFactory.CreateHandler(true, customProxy))
            {
                Assert.True(i3.UseProxy);
                Assert.Equal(customProxy, i3.Proxy);
                Assert.True(i3.AutomaticDecompression.HasFlag(System.Net.DecompressionMethods.GZip) && i3.AutomaticDecompression.HasFlag(System.Net.DecompressionMethods.Deflate));
            }
        }
    }
}
