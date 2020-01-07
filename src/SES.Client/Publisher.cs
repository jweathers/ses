using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SES.Core;
namespace SES.Client
{
    public class Publisher<T>:IDisposable
    {
        private readonly PublisherOptions publisherOptions;
        private readonly HttpClient httpClient;
        public Publisher(PublisherOptions publisherOptions)
        {
            this.publisherOptions = publisherOptions??throw new ArgumentNullException(nameof(publisherOptions));
            this.httpClient=new HttpClient();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task<bool> PublishAsync(T @event,CancellationToken? token=null)
        {

            var response = await httpClient.PostAsync(publisherOptions.MakePublishUrl<T>(),new StringContent(JsonConvert.SerializeObject(@event)),token??CancellationToken.None).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
