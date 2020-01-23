namespace SES.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SESEvent
    {
        public ulong Index{get;set;}
        public string Data{get;set;}
        public string QueueName{get;set;}
    }
}