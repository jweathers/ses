using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SES.Store;
using SES.Core;
using System.IO;

namespace SES.Server.Controllers
{
    [ApiController]
    [Route("queues")]

    //    return $"{endpoint}/{typeof(T).FullName}/fetch/{startIndex}?count={options.PreferredBatchSize}";
    public class SESController : ControllerBase
    {
        private readonly IEventStore eventStore;

        public SESController(IEventStore eventStore)
        {
            this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }
        [HttpGet("{queueName}/fetch/{startIndex}")]
        public async Task<IActionResult> Fetch([FromRoute]string queueName, [FromRoute]ulong startIndex, [FromQuery]uint count=50)
        {
            var results = await eventStore.FetchAsync(queueName,startIndex,count);
            return Ok(results);
        }

        [HttpPost("{queueName}/publish")]
        public async Task<IActionResult> Publish([FromRoute]string queueName)
        {
            using(var sr = new StreamReader(Request.Body))
            {
                var data = await sr.ReadToEndAsync();
                await eventStore.StoreAsync(queueName,data);
                return Accepted();
            }
        }
    }
}
