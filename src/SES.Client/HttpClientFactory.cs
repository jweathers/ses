using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SES.Client
{
    internal class HttpClientFactory
    {
        public static HttpClient NewClient(bool proxyEnabled=false, IWebProxy customProxy=null)
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.UseProxy = proxyEnabled;
            if(proxyEnabled)
            {
                clientHandler.Proxy = customProxy ?? System.Net.WebRequest.GetSystemWebProxy();
            }
            else
            {
                clientHandler.Proxy = null;
            }
            clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return new HttpClient(clientHandler, true);
        }
    }
}
