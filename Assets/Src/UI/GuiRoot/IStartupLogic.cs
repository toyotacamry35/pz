using Infrastructure.Cloud;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Src
{
    public interface IStartupLogic
    {
        Task<bool> CanStart();
        Task DoStart(Task probufInitTask, InnerProcess inner, CancellationToken ct);
    }
}
