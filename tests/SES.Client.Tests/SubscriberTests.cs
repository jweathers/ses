using Moq;
using SES.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SES.Client.Tests
{
    public class SubscriberTests
    {
        [Fact]
        public void ConstructorGuardsTest()
        {
            var httpClient = Mock.Of<HttpClientProxy>();
            var options = Mock.Of<SubscriptionOptions>();
            var serializer = Mock.Of<IAsyncEventSerializer>();
            Func<ulong, string, Task> onEventReceivedDelegate = (a, b) => Task.CompletedTask;

            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(null, options, 1, serializer, onEventReceivedDelegate));
            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(httpClient, null, 1, serializer, onEventReceivedDelegate));
            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(httpClient, options, 1, null, onEventReceivedDelegate));
            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(httpClient, options, 1, serializer, null));
        }

        

    }
}
