using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SES.Core;
namespace SES.Client
{
    public class Subscription<T>:IDisposable
    {
        private readonly SubscriptionOptions subscriptionOptions;
        private readonly EventHandler<T> eventHandler;
        private readonly System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();
        private readonly HttpClient httpClient;
        internal Subscription(EventHandler<T> eventHandler,SubscriptionOptions subscriptionOptions)
        {
            this.eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
            this.subscriptionOptions = subscriptionOptions??throw new ArgumentNullException(nameof(subscriptionOptions));
            this.httpClient=new HttpClient();
        }

        public void Dispose()
        {
            httpClient.Dispose();
            cts.Dispose();
        }

        public async Task StartAsync()
        {
            ulong indexToFetch = subscriptionOptions.InitialIndex;
            while(!cts.IsCancellationRequested)
            {

                var response = await httpClient.GetStringAsync(subscriptionOptions.MakeFetchUrl<T>(indexToFetch));
                var events = JsonConvert.DeserializeObject<Event[]>(response);
                foreach(var e in events)
                {
                    await eventHandler.ProcessAsync(e.Index,e.DeserializeEventData(),cts.Token);
                    indexToFetch=e.Index+1;
                }
                await Task.Delay(subscriptionOptions.PollingInterval);
            }
        }
        public void Stop()
        {
            cts.Cancel();
        }
    }
}
