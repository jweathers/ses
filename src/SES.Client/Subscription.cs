using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using SES.Serialization;
using System.IO;
using System.IO.Compression;

namespace SES.Client
{
    public class Subscription : IDisposable
    {
        private readonly SubscriptionOptions subscriptionOptions;
        private readonly IAsyncEventSerializer serializer;
        private readonly System.Threading.CancellationTokenSource cts;
        private readonly List<ISubscriber> subscribers;
        private readonly HttpClientProxy httpClient;
        public Subscription(SubscriptionOptions subscriptionOptions,IAsyncEventSerializer serializer) :
            this(subscriptionOptions,serializer,new HttpClientProxy(subscriptionOptions?.CreateHttpClient()))
        { }

        internal Subscription(SubscriptionOptions subscriptionOptions, IAsyncEventSerializer serializer, HttpClientProxy httpClient)
        {
            this.subscriptionOptions = subscriptionOptions ?? throw new ArgumentNullException(nameof(subscriptionOptions));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.httpClient = httpClient;
            this.subscribers = new List<ISubscriber>();
            this.cts = new System.Threading.CancellationTokenSource();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            httpClient.Dispose();
            cts.Dispose();
        }

        public void RegisterHandler<T>(ulong startindex, Func<ulong, T, Task> onEventReceived, Action<ulong, Exception> onException = null)
        {
            if (onEventReceived == default)
            {
                throw new ArgumentNullException(nameof(onEventReceived));
            }
            subscribers.Add(new Subscriber<T>(this.httpClient, this.subscriptionOptions, startindex,serializer, onEventReceived, onException));
        }
        public Task RunAsync()
        {
            return Task.WhenAll(subscribers.Select(s => s.RunAsync(cts.Token)));
        }
        public void Stop()
        {
            cts.Cancel();
        }
    }

}
