using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SES.Core;
using SES.Serialization;

namespace SES.Client
{
    public class Publisher<T>:IDisposable
    {
        private readonly PublisherOptions publisherOptions;
        private readonly HttpClient httpClient;
        private readonly IAsyncEventSerializer serializer;

        public Publisher(PublisherOptions publisherOptions,IAsyncEventSerializer serializer)
        {
            this.publisherOptions = publisherOptions??throw new ArgumentNullException(nameof(publisherOptions));
            this.httpClient = HttpClientFactory.NewClient(publisherOptions.ProxyEnabled, publisherOptions.Proxy);
            this.serializer = serializer ?? throw new ArgumentOutOfRangeException(nameof(serializer), "An event serializer must be provided.");
        }

        public void Dispose()
        {
          httpClient.Dispose();
        }

        public async Task<bool> PublishAsync(T @event,CancellationToken? token=null)
        {
            var uri = publisherOptions.MakePublishUri<T>();
            var content = new StringContent(await serializer.SerializeAsync(@event), System.Text.Encoding.UTF8, serializer.ContentType);
            var response = await httpClient.PostAsync(uri, content);
            return response.IsSuccessStatusCode;
        }
    }
}
