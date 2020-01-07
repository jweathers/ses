using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SES.Store
{
    public interface IEventStore
    {
        Task<IEnumerable<byte[]>> Fetch(string queue, ulong startIndex, uint count);
        Task Store(string queue,byte[] @eventData);
    }
}
