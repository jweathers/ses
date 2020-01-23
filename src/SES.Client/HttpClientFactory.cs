#pragma warning disable CA2000 // Dispose objects before losing scope
//the clientHandler is disposed of by the HttpClient

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SES.Client
{
    internal static class HttpClientFactory
    {
        internal static HttpClientHandler CreateHandler(bool proxyEnabled = false, IWebProxy customProxy = null)
        {
            var clientHandler = new HttpClientHandler
            {
                UseProxy = proxyEnabled
            };

            if (proxyEnabled)
            {
                clientHandler.Proxy = customProxy ?? System.Net.WebRequest.GetSystemWebProxy();
            }
            else
            {
                clientHandler.Proxy = null;
            }
            clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return clientHandler;
        }
        public static HttpClient NewClient(bool proxyEnabled=false, IWebProxy customProxy=null)
        {

            return new HttpClient(CreateHandler(proxyEnabled,customProxy),true);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        internal static HttpClient CreateHttpClient(this PublisherOptions publisherOptions) => NewClient(publisherOptions.ProxyEnabled, publisherOptions.Proxy);
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        internal static HttpClient CreateHttpClient(this SubscriptionOptions subscriptionOptions) => NewClient(subscriptionOptions.ProxyEnabled, subscriptionOptions.Proxy);
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        internal static HttpClientProxy CreateHttpClientProxy(this PublisherOptions publisherOptions) => new HttpClientProxy(NewClient(publisherOptions.ProxyEnabled, publisherOptions.Proxy));
    }


}
#pragma warning restore CA2000 // Dispose objects before losing scope