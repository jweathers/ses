using System;
namespace SES.Client
{
    public class SubscriptionFactory
    {
        private readonly SubscriptionOptions defaultSubscriptionOptions;

        public SubscriptionFactory(SubscriptionOptions defaultSubscriptionOptions)
        {
            this.defaultSubscriptionOptions = defaultSubscriptionOptions ?? throw new ArgumentNullException(nameof(defaultSubscriptionOptions));
        }
        public Subscription<T> CreateSubscription<T>(EventHandler<T> subscriber, SubscriptionOptions subsriptionOptions=null)
        {
            return new Subscription<T>(subscriber,subsriptionOptions??defaultSubscriptionOptions);
        }

    }
}
