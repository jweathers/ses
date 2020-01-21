using System;
using Xunit;

namespace SES.Client.Tests
{
    public class SubscriptionGroupOptionsTests
    {
        [Fact]
        public void TestEventIsEligibleForProcessingBySubscriber()
        {
            var defaultSubscriptionGroupOptions = new SubscriptionGroupOptions();
            Assert.True(defaultSubscriptionGroupOptions.ShouldProcessEvent(0)); //this should always be true
            Assert.True(defaultSubscriptionGroupOptions.ShouldProcessEvent(1)); //this should always be true
            Assert.True(defaultSubscriptionGroupOptions.ShouldProcessEvent(2)); //this should always be true
            Assert.True(defaultSubscriptionGroupOptions.ShouldProcessEvent(3)); //this should always be true
            Assert.True(defaultSubscriptionGroupOptions.ShouldProcessEvent(4)); //this should always be true

            var subscriptionGroupOptions = new SubscriptionGroupOptions
            {
                GroupMembershipPosition = 1,
                TotalSubscribersInGroup = 3
            };


            Assert.True(subscriptionGroupOptions.ShouldProcessEvent(3));
            Assert.False(subscriptionGroupOptions.ShouldProcessEvent(2));
            Assert.False(subscriptionGroupOptions.ShouldProcessEvent(1));
        }


        [Fact]
        public void TestCannotProvideGroupMembershipNumberOutsideOfGroupMembershipCount()
        {
            var subscriptionGroupOptions = new SubscriptionGroupOptions
            {
                TotalSubscribersInGroup = 3
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => subscriptionGroupOptions.GroupMembershipPosition = 9);
            Assert.Throws<ArgumentOutOfRangeException>(() => subscriptionGroupOptions.GroupMembershipPosition = 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => subscriptionGroupOptions.GroupMembershipPosition = -1);
        }
    }
}
