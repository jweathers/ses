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
        private readonly IHttpClientProxy httpClient;
        private readonly IAsyncEventSerializer serializer;

#pragma warning disable CA2000 //client is disposed of so no need in warning    
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] //nothing to test here
        public Publisher(PublisherOptions publisherOptions,IAsyncEventSerializer serializer) 
            : this(publisherOptions ?? throw new ArgumentNullException(nameof(publisherOptions)), 
                  serializer ?? throw new ArgumentNullException(nameof(serializer)), 
                  publisherOptions.CreateHttpClientProxy())
        { 
        }
#pragma warning restore CA2000
        internal Publisher(PublisherOptions publisherOptions, IAsyncEventSerializer serializer, IHttpClientProxy httpClient)
        {
            this.publisherOptions = publisherOptions ?? throw new ArgumentNullException(nameof(publisherOptions));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] //can't test as httpclient can't be mocked
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] //can't test as httpclient can't be mocked        
        protected virtual void Dispose(bool disposing)
        {
            httpClient?.Dispose();
        }

        public async Task<bool> PublishAsync(T @event,CancellationToken? token=null)
        {
            var uri = publisherOptions.MakePublishUri<T>();
            using (var content = new StringContent(await serializer.SerializeAsync(@event).ConfigureAwait(false), System.Text.Encoding.UTF8, serializer.ContentType))
            {
                var response = await httpClient.PostAsync(uri, content, token ?? CancellationToken.None).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
        }
    }
}