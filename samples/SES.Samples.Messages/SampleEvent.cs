using System;

namespace SES.Samples.Messages
{
    public class SampleEvent
    {
        public string Data { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    }
}
