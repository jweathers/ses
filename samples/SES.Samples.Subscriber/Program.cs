using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SES.Client;
using SES.Samples.Messages;
using SES.Serialization;

namespace SES.Samples.Subscriber
{
    class Program
    {
        static FileStream indexlog= File.Open("indexlog.txt",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.Read);
       
        static async Task HandleSampleEventMessage(ulong index, SampleEvent @event)
        {
            await SaveIndexAsync(index);
            Console.WriteLine($"Received event with passed index {index} content {@event.Data}");
        }

        static void LogException(ulong index, Exception ex)
        {
            Console.WriteLine($"Exception occured: {ex.ToString()}");
        }

        static async Task Main(string[] args)
        {
            IAsyncEventSerializer serializer = new SES.Serialization.Json.JsonSerializer();
            var subscription = new Subscription(new SubscriptionOptions()
            {
                Endpoint = "http://localhost:5000",
                PollingInterval = TimeSpan.FromSeconds(15),
                ProxyEnabled = true,
                Proxy = new System.Net.WebProxy("localhost", 8888),
                OnExceptionCallback = LogException,
                PreferredBatchSize = 1500
            }, serializer) ;


            ulong lastProcessedIndex = await GetSavedIndexAsync();
            ulong initialIndex = lastProcessedIndex + 1;
            subscription.RegisterHandler<SampleEvent>(initialIndex, HandleSampleEventMessage);
 
            
            Console.WriteLine("Subscriber starting....");
            await Task.Delay(10000);


            await subscription.RunAsync();


            Console.WriteLine("Subscriber started");
            Console.WriteLine("Press Control-x to exit.");





            Console.CancelKeyPress += (s, o) =>
            {
                subscription.Stop();
                Environment.Exit(0);
            };
        }

        static async Task SaveIndexAsync(ulong index)
        {
            indexlog.Seek(0, SeekOrigin.Begin);
            string formattedString = String.Format("{0,22:D22}", index);
            await indexlog.WriteAsync(System.Text.Encoding.UTF8.GetBytes(formattedString)).ConfigureAwait(false);
            await indexlog.FlushAsync().ConfigureAwait(false);
        }

        static async Task<ulong> GetSavedIndexAsync()
        {
            var result = new byte[22];
            indexlog.Seek(0, SeekOrigin.Begin);
            await indexlog.ReadAsync(result).ConfigureAwait(false);
            if (result.All(b => b == 0)) { return 0; }
            return ulong.Parse(System.Text.Encoding.UTF8.GetString(result));
        }
        
    }
}

