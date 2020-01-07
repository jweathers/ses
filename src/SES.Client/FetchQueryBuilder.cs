namespace SES.Client
{
    internal static class FetchQueryBuilder
    {
        public static string MakeFetchUrl<T>(this SubscriptionOptions options, ulong startIndex)
        {
            var endpoint = (options.Endpoint.EndsWith("/") || options.Endpoint.EndsWith("\\"))?options.Endpoint.Substring(0,options.Endpoint.Length-1):options.Endpoint;
            return $"{endpoint}/queues/{typeof(T).FullName}/fetch/{startIndex}?count={options.PreferredBatchSize}";
        }

        public static string MakePublishUrl<T>(this PublisherOptions options)
        {
            var endpoint = (options.Endpoint.EndsWith("/") || options.Endpoint.EndsWith("\\"))?options.Endpoint.Substring(0,options.Endpoint.Length-1):options.Endpoint;
            return $"{endpoint}/queues/{typeof(T).FullName}/publish";

        }
    }
}
