using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace SES.Client
{
    public abstract class EventHandler<T>
    {
        internal async Task ProcessAsync(ulong index, object @event,CancellationToken cancellationToken)=>await OnEventReceivedAsync(index,(T)@event,cancellationToken);
        protected abstract Task OnEventReceivedAsync(ulong index, T @event, CancellationToken cancellationToken);
    }
}
