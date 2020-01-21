using System;
using System.Net;

namespace SES.Client
{
    public class SubscriptionOptions
    {
        public static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(60);
        public static readonly TimeSpan MinimumPollingInterval = TimeSpan.FromSeconds(5);
        public TimeSpan PollingInterval { get; set; } = DefaultPollingInterval;

        public const int MaxBatchSize = 1000;
        public const int DefaultBatchSize = MaxBatchSize / 20;
        public int PreferredBatchSize { get; set; } = DefaultBatchSize;

        public string Endpoint { get; set; }
        public bool ProxyEnabled { get; set; } = false;
        public IWebProxy Proxy { get; set; }
        public Action<ulong, Exception> OnExceptionCallback { get; set; } = null;
        public bool EnablePollIntervalRandomization { get; set; } = true;
        public SubscriptionGroupOptions GroupOptions{get;set;}=new SubscriptionGroupOptions();
    }
}
