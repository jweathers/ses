using SES.Serialization;
using System;
using System.Net;

namespace SES.Client
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class PublisherOptions
    {
       public string Endpoint{get;set;}
        public bool ProxyEnabled { get; set; } = false;
        public IWebProxy Proxy { get; set; }
    }
}
