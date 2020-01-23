using Moq;
using SES.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SES.Client.Tests
{

    public class PublisherTests
    {
        internal class Dummy
        { }
        Mock<IAsyncEventSerializer> mockSerializer = new Mock<IAsyncEventSerializer>();
        PublisherOptions mockOptions = new PublisherOptions() { Endpoint = "http://test", ProxyEnabled = false };

        [Fact]
        public void ConstructorTests()
        {

            Assert.Throws<ArgumentNullException>(() => new Publisher<Dummy>(null, mockSerializer.Object));
            Assert.Throws<ArgumentNullException>(() => new Publisher<Dummy>(mockOptions, null));
        }

        [Fact]
        public async Task PublishTests()
        {
            using (var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Accepted))
            {
                var sesevent = new Dummy();
                mockSerializer.Reset();
                var _1 = mockSerializer.Setup(m => m.SerializeAsync(sesevent)).Returns(Task.FromResult("{}"));
                var _2 = mockSerializer.Setup(m => m.ContentType).Returns("application/json");
                var httpClient = new Mock<IHttpClientProxy>();
                var _3 = httpClient
                            .Setup(c => c.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
                            .Returns(Task.FromResult(httpResponseMessage));
                using (var publisher = new Publisher<Dummy>(mockOptions, mockSerializer.Object, httpClient.Object))
                {
                    var result = await publisher.PublishAsync(sesevent);
                    Assert.True(result);
                }
            }
        }
    }
}
