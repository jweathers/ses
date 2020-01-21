using System.Threading;
using System.Threading.Tasks;

namespace SES.Client
{
    internal interface ISubscriber
    {
        Task RunAsync(CancellationToken cancellationToken);
    }

}
