using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using System.Threading.Tasks;

namespace SharedCode.Entities
{
    public interface IBank : IHasBankEngine
    {
    }

    public interface IHasBankEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IBankEngine Bank { get; set; }
    }
}

