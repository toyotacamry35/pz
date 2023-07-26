using System.Threading.Tasks;
using ColonyShared.SharedCode.Entities.Reactions;

namespace ColonyShared.SharedCode.Reactions
{
    public interface IReactionHandler
    {
        Task Invoke(ArgTuple[] args);
    }
}
