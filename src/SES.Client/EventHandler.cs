using System.Threading;
using System.Threading.Tasks;
namespace SES.Client
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class EventHandler<T>
    {
        internal async Task ProcessAsync(ulong index, T @event,CancellationToken cancellationToken)=>await OnEventReceivedAsync(index,@event,cancellationToken).ConfigureAwait(false);
        protected abstract Task OnEventReceivedAsync(ulong index, T sesevent, CancellationToken cancellationToken);
    }
}
