using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Net.Http;

namespace SES.Client.Tests
{
    public class HttpClientProxyTests
    {
        [Fact]
        public void ConstructorThrowExceptionOnNullHttpClientTest()
        {
            Assert.Throws<ArgumentNullException>(()=>new HttpClientProxy(null));
        }

    }
}