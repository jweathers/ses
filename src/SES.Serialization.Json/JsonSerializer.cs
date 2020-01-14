using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
namespace SES.Serialization.Json
{
    public class JsonSerializer : IAsyncEventSerializer
    {
        

        public Task<T> DeserializeAsync<T>(string data)
        {
            return Task.FromResult(JsonConvert.DeserializeObject<T>(data));
        }
        public Task<string> SerializeAsync<T>(T @event)
        {
            return Task.FromResult(JsonConvert.SerializeObject(@event));
        }


        public string ContentType => "application/json";
    }
}
