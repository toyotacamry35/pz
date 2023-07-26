using System.Threading;
using System.Threading.Tasks;

namespace Assets.Src
{
    /// <summary>
    /// Нода содержащая логику, на время выполнения которой, загрузка следующих сцен должна приостанавливаться. 
    /// </summary>
    public interface IStartupIntermediateLogic
    {
        Task<bool> CanStart();

        /// <returns>true, если если сцену надо выгрузить после того как логика отработает</returns>
        bool MustBeUnloadedAfterLogic { get; }

        /// <summary>
        /// Делает логику.
        /// </summary>
        /// <param name="ct"></param>
        Task DoLogic(CancellationToken ct);
    }
}