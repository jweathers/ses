using System;
namespace SES.Client
{
    public class SubscriptionOptions
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
}
