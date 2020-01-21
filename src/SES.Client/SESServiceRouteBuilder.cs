namespace SES.Client
{
    internal static class SESServiceRouteBuilder
    {
        public static System.Uri MakeFetchUri<T>(this SubscriptionOptions options, ulong startIndex)
        {
            var endpoint = (options.Endpoint.EndsWith("/",System.StringComparison.InvariantCultureIgnoreCase) || options.Endpoint.EndsWith("\\", System.StringComparison.InvariantCultureIgnoreCase))?options.Endpoint.Substring(0,options.Endpoint.Length-1):options.Endpoint;
            return new System.Uri($"{endpoint}/queues/{typeof(T).FullName}/fetch/{startIndex}?count={options.PreferredBatchSize}");
        }

        public static System.Uri MakePublishUri<T>(this PublisherOptions options)
        {
            var endpoint = (options.Endpoint.EndsWith("/", System.StringComparison.InvariantCultureIgnoreCase) || options.Endpoint.EndsWith("\\", System.StringComparison.InvariantCultureIgnoreCase))?options.Endpoint.Substring(0,options.Endpoint.Length-1):options.Endpoint;
            return new System.Uri($"{endpoint}/queues/{typeof(T).FullName}/publish");

        }
    }
}
