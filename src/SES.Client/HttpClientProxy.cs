using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SES.Client
{

    /// <summary>
    /// This class provides support for testability as HttpClient isn't mockable
    /// </summary>
    internal class HttpClientProxy:IDisposable
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// this constructor is provided solely for the purposes of testing and should not be used
        /// </summary>
        internal HttpClientProxy()
        {

        }
        public HttpClientProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        private void EnsureHttpClientExists()
        {
            if (this.httpClient == null) throw new InvalidOperationException("httpClient is null so impermissible constructor was used to create this instance.");
        }
        public virtual Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken? cancellationToken = null)
        {
            EnsureHttpClientExists();
            return httpClient.PostAsync(uri, content, cancellationToken ?? CancellationToken.None);
        }

        public virtual Task<string> GetStringAsync(Uri uri)
        {
            EnsureHttpClientExists();
            return httpClient.GetStringAsync(uri);
        }
        public HttpRequestHeaders DefaultRequestHeaders => httpClient.DefaultRequestHeaders;
        protected virtual void Dispose(bool disposing)
        {
            httpClient?.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
