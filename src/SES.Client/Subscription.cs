using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SES.Core;
using System.Linq;
using SES.Serialization;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;

namespace SES.Client
{
    public class Subscription : IDisposable
    {
        private readonly SubscriptionOptions subscriptionOptions;
        private readonly IAsyncEventSerializer serializer;
        private readonly System.Threading.CancellationTokenSource cts;
        private readonly List<ISubscriber> subscribers;
        private readonly HttpClient httpClient;
        public Subscription(SubscriptionOptions subscriptionOptions,IAsyncEventSerializer serializer)
        {
            this.subscriptionOptions = subscriptionOptions ?? throw new ArgumentNullException(nameof(subscriptionOptions));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.httpClient = HttpClientFactory.NewClient(subscriptionOptions.ProxyEnabled, subscriptionOptions.Proxy);
            this.subscribers = new List<ISubscriber>();
            this.cts = new System.Threading.CancellationTokenSource();
        }



        public void Dispose()
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
    public class SubscriptionException : System.Exception
    {
        public SubscriptionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    internal interface ISubscriber
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
    internal class Subscriber<T>:ISubscriber
    {
        private readonly HttpClient httpClient;
        private readonly SubscriptionOptions subscriptionOptions;
        private readonly ulong startindex;
        private readonly IAsyncEventSerializer serializer;
        private readonly Func<ulong, T, Task> onEventReceived;
        private readonly Action<ulong, Exception> onException;
        private readonly Random rng;
        public Subscriber(HttpClient httpClient, SubscriptionOptions subscriptionOptions, ulong startindex, IAsyncEventSerializer serializer, Func<ulong, T, Task> onEventReceived, Action<ulong, Exception> onException = null)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.subscriptionOptions = subscriptionOptions ?? throw new ArgumentNullException(nameof(subscriptionOptions));
            this.startindex = startindex;
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.onEventReceived = onEventReceived ?? throw new ArgumentNullException(nameof(onEventReceived));
            this.onException = onException ?? 
                                new Action<ulong,Exception>((i,e)=>throw new SubscriptionException($"An error has occured in a subscriber for type {typeof(T).FullName} on index {i}.",e));
            this.rng = new Random();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            ulong indexToFetch = startindex;

            while (!cancellationToken.IsCancellationRequested)
            {

                var fetchFromUri = subscriptionOptions.MakeFetchUri<T>(indexToFetch);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var eventBytes = await httpClient.GetStringAsync(fetchFromUri).ConfigureAwait(false);
                var startTime = DateTimeOffset.UtcNow;
                foreach (var e in await DeserializeEvents(eventBytes))
                {
                    try
                    {
                        await onEventReceived(e.Index, await serializer.DeserializeAsync<T>(e.Data).ConfigureAwait(false)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        var exCallback = onException ?? subscriptionOptions.OnExceptionCallback;
                        if (exCallback == null)
                        {
                            throw;
                        }
                        else
                        {
                            exCallback(indexToFetch, ex);
                        }
                    }
                    indexToFetch = e.Index + 1;
                }

                await Task.Delay(CalculateWaitTime(startTime));

            }
        }

        private async Task<Event[]> DeserializeEvents(string eventData)
        {
            Event[] events;
            if (eventData.Length > 0)
            {
                events = await serializer.DeserializeAsync<Event[]>(eventData);
            }
            else
            {
                events = new Event[] { };
            }
            return events;
        }

        private TimeSpan CalculateWaitTime(DateTimeOffset processingStartTime)
        {
            var processingTime = DateTimeOffset.UtcNow - processingStartTime;
            var nextPollDelayMS = Math.Max((subscriptionOptions.PollingInterval - processingTime).TotalMilliseconds, 0);
            if (subscriptionOptions.EnablePollIntervalRandomization)
            {
                nextPollDelayMS = nextPollDelayMS * rng.Next(75, 125) / 100;
            }
            return TimeSpan.FromMilliseconds(nextPollDelayMS);
        }
    }

}
