using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SES.Client
{
    internal interface IHttpClientProxy
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }

        void Dispose();
        Task<string> GetStringAsync(Uri uri);
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken? cancellationToken = null);
    }

    /// <summary>
    /// This class provides support for testability as HttpClient isn't mockable
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] //nothing to really test here
    internal class HttpClientProxy : IDisposable, IHttpClientProxy
    {
        private readonly HttpClient httpClient;
        private readonly bool shouldDisposeClient;

        public HttpClientProxy(HttpClient httpClient, bool shouldDisposeClient = true)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.shouldDisposeClient = shouldDisposeClient;
        }

        public virtual Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken? cancellationToken = null)
        {
            return httpClient.PostAsync(uri, content, cancellationToken ?? CancellationToken.None);
        }

            public virtual Task<string> GetStringAsync(Uri uri)
        {
            return httpClient.GetStringAsync(uri);
        }
        public HttpRequestHeaders DefaultRequestHeaders => httpClient.DefaultRequestHeaders;

        protected virtual void Dispose(bool disposing)
        {
            if (shouldDisposeClient)
            {
                httpClient?.Dispose();
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
