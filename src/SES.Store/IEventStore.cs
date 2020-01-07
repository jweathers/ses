using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SES.Core;
namespace SES.Store
{
    public interface IEventStore
    {
        Task<IEnumerable<Event>> FetchAsync(string queue, ulong startIndex, uint count);
        Task StoreAsync(string queue, string data);
    }
}
