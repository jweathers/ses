using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SES.Serialization
{
    public interface IAsyncEventSerializer
    {
        Task<string> SerializeAsync<T>(T sesevent);
        Task<T> DeserializeAsync<T>(string data);
        string ContentType { get; }
    }

}
