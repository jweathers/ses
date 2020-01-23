using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SES.Core;
using SES.Serialization;
using System.Net.Http.Headers;

namespace SES.Client
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Subscriber<T> : ISubscriber
    {
        private readonly IHttpClientProxy httpClient;
        private readonly SubscriptionOptions subscriptionOptions;
        private readonly ulong startindex;
        private readonly IAsyncEventSerializer serializer;
        private readonly Func<ulong, T, Task> onEventReceived;
        private readonly Action<ulong, Exception> onException;
        private readonly Random rng;
        private readonly Stopwatch stopwatch;
        public Subscriber(IHttpClientProxy httpClient, SubscriptionOptions subscriptionOptions, ulong startindex, IAsyncEventSerializer serializer, Func<ulong, T, Task> onEventReceived, Action<ulong, Exception> onException = null)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.subscriptionOptions = subscriptionOptions ?? throw new ArgumentNullException(nameof(subscriptionOptions));
            this.startindex = startindex;
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.onEventReceived = onEventReceived ?? throw new ArgumentNullException(nameof(onEventReceived));
            this.onException = onException ??
                                new Action<ulong, Exception>((i, e) => throw new SubscriptionException($"An error has occured in a subscriber for type {typeof(T).FullName} on index {i}.", e));
            this.rng = new Random();
            stopwatch = new Stopwatch();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            ulong indexToFetch = startindex;

            while (!cancellationToken.IsCancellationRequested)
            {
                string eventBytes = await FetchNextBatch(indexToFetch).ConfigureAwait(false);
                stopwatch.Start(true);
                var totalItemsProcessed = await ProcessEvents(await DeserializeEvents(eventBytes).ConfigureAwait(false)).ConfigureAwait(false);
                indexToFetch += totalItemsProcessed;
                var waitTime = CalculateWaitTime(stopwatch.Stop());
                await Task.Delay(waitTime).ConfigureAwait(false);

            }
        }

        private async Task<ulong> ProcessEvents(SESEvent[] events)
        {
            int processedEvents = 0;
            foreach (var e in events )
            {
                if (subscriptionOptions?.GroupOptions?.ShouldProcessEvent(e.Index) ?? true)
                {
                    await ProcessEvent(e.Index, e).ConfigureAwait(false);
                }
                //even if it isn't eligible for processing we should increment because there is no reason to get it again
                processedEvents++;
            }

            return (ulong)processedEvents;
        }

        private async Task ProcessEvent(ulong indexToFetch, SESEvent e)
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
        }

        private async Task<string> FetchNextBatch(ulong indexToFetch)
        {
            var fetchFromUri = subscriptionOptions.MakeFetchUri<T>(indexToFetch);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var eventBytes = await httpClient.GetStringAsync(fetchFromUri).ConfigureAwait(false);
            return eventBytes;
        }

        private async Task<SESEvent[]> DeserializeEvents(string eventData)
        {
            SESEvent[] events;
            if (eventData.Length > 0)
            {
                events = await serializer.DeserializeAsync<SESEvent[]>(eventData).ConfigureAwait(false);
            }
            else
            {
                events = Array.Empty<SESEvent>();
            }
            return events;
        }
        
        private TimeSpan CalculateWaitTime(TimeSpan processingTime)
        {
            var nextPollDelayMS = Math.Max((subscriptionOptions.PollingInterval - processingTime).TotalMilliseconds, 0);
            if (subscriptionOptions.EnablePollIntervalRandomization)
            {
                #pragma warning disable SCS0005 //Weak random generator.  
                                                //Not a problem as we're merely trying to 
                                                //randomize the next fetch time to avoid 
                                                //overlap with other unknown subscribers
                nextPollDelayMS = nextPollDelayMS * rng.Next(75, 125) / 100;
                #pragma warning restore SCS0005 //Weak random generator
            }
            return TimeSpan.FromMilliseconds(nextPollDelayMS);
        }
    }

}
