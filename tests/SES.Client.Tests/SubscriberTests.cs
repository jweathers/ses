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
            var httpClient = new Mock<IHttpClientProxy>();
            var options = new Mock<SubscriptionOptions>();
            var serializer = new Mock<IAsyncEventSerializer>();
            Func<ulong, string, Task> onEventReceivedDelegate = (a, b) => Task.CompletedTask;

            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(null, options.Object, 1, serializer.Object, onEventReceivedDelegate));
            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(httpClient.Object, null, 1, serializer.Object, onEventReceivedDelegate));
            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(httpClient.Object, options.Object, 1, null, onEventReceivedDelegate));
            Assert.Throws<ArgumentNullException>(() => new Subscriber<string>(httpClient.Object, options.Object, 1, serializer.Object, null));
        }

        

    }
}
