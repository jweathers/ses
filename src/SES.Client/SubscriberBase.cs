using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SES.Client
{
    public class SubsriptionOptions
    {
        public static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(60);
        public static readonly TimeSpan MinimumPollingInterval = TimeSpan.FromSeconds(5);
        public TimeSpan PollingInterval{get;set;}=DefaultPollingInterval;

        public ulong InitialIndex{get;set;}

        public const int MAX_BATCH_SIZE=1000;
        public const int DEFAULT_BATCH_SIZE=MAX_BATCH_SIZE/20;
        public int PreferredBatchSize{get;set;}=DEFAULT_BATCH_SIZE;

        public string Endpoint{get;set;}
    }

    internal static class FetchQueryBuilder
    {
        public static string MakeFetchUrl<T>(this SubsriptionOptions options, ulong startIndex)
        {
            var endpoint = (options.Endpoint.EndsWith("/") || options.Endpoint.EndsWith("\\"))?options.Endpoint.Substring(0,options.Endpoint.Length-1):options.Endpoint;
            return $"{endpoint}/{typeof(T).FullName}/fetch/{startIndex}?count={options.PreferredBatchSize}";
        }
    }
    public interface ISubscriber<in T>
    {
        Task Process(ulong index, T @event);
    }
    public class Subscription<T>:IDisposable
    {
        private readonly SubsriptionOptions subsriptionOptions;
        private readonly ISubscriber<T> subscriber;
        private readonly System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();
        private readonly HttpClient httpClient;
        public Subscription(ISubscriber<T> subscriber,SubsriptionOptions subsriptionOptions)
        {
            this.subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            this.subsriptionOptions = subsriptionOptions??throw new ArgumentNullException(nameof(subsriptionOptions));
            this.httpClient=new HttpClient();
        }

        public void Dispose()
        {
            httpClient.Dispose();
            cts.Dispose();
        }

        public void Start()
        {
            //Task.
        }
        public void Stop(){}

        private async Task Poll(CancellationToken? cancellationToken=null)
        {
            var ct=cancellationToken??CancellationToken.None;
            var pollingInterval= subsriptionOptions.PollingInterval;
            var batchSize = subsriptionOptions.PreferredBatchSize;
            while(!ct.IsCancellationRequested)
            {
                var executionState = (SubsriptionOptions options, HttpClient client);
                
                Task.Delay(pollingInterval,ct).ContinueWith(async (completedTask,state)=>
                    {
                        httpClient.GetStringAsync(state.)
                    }
                ,executionState);
            }
        }
    }
    public class SubscriptionFactory
    {
        private readonly SubsriptionOptions defaultSubscriptionOptions;

        public SubscriptionFactory(SubsriptionOptions defaultSubscriptionOptions)
        {
            this.defaultSubscriptionOptions = defaultSubscriptionOptions ?? throw new ArgumentNullException(nameof(defaultSubscriptionOptions));
        }
        public Subscription<T> CreateSubscription<T>(ISubscriber<T> subscriber, SubsriptionOptions subsriptionOptions=null)
        {
            //return new Subscription<T>(){SubsriptionOptions=subsriptionOptions??defaultSubscriptionOptions,Subscriber=subscriber};
        }

    }
}
