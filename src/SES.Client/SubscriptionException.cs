using System;

namespace SES.Client
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SubscriptionException : System.Exception
    {
        public SubscriptionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SubscriptionException()
        {
        }

        public SubscriptionException(string message) : base(message)
        {
        }
    }

}
