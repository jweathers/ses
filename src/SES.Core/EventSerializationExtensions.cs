using System;
using Newtonsoft.Json;
namespace SES.Core
{
    internal static class EventSerializationExtensions
    {
        public static object DeserializeEventData(this Event @event)
        {
            return JsonConvert.DeserializeObject(@event.Data);
        }
    }
}