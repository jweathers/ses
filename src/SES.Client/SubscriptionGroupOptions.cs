using System;

namespace SES.Client
{
    public class SubscriptionGroupOptions
    {
        public SubscriptionGroupOptions()
        {
            Name = "Default";
            TotalSubscribersInGroup = 1;
            GroupMembershipPosition = 1;
        }
        public string Name { get; set; }
        public int TotalSubscribersInGroup { get; set; }

        private int groupMembershipPosition;
        public int GroupMembershipPosition
        {
            get => groupMembershipPosition;
            set
            {
                if(value > 0 && value<=TotalSubscribersInGroup)
                {
                    groupMembershipPosition = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        internal bool ShouldProcessEvent(ulong index)
        {
            //if no group options are provided or there is only one subscriber in the group
            //then we should always process
            if (TotalSubscribersInGroup == 1)
            {
                return true;
            }
            //if (index % subscriber_count)+1 equals this members postion in the subscriber group
            //then this event should pe processed by this subscriber
            //for example
            //  given 3 subscribers in a group and subscriber A with position 1
            //  when e.Index=1 then (1 % 3)+1 == 2 and Subscriber A WILL NOT receive this event because its position is 1
            //  when e.Index=2 then (2 % 3)+1 == 3 and Subscriber A WILL NOT receive this event because its position is 1
            //  when e.Index=32 then (3 % 3)+1 == 1 and Subscriber A WILL receive this event because its position is 1
            return (index % (ulong)(TotalSubscribersInGroup)) + 1 == (ulong)(GroupMembershipPosition);

        }
    }
}
