using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace GeneratedCode.EntityModel.Test
{
    public interface IToucherTestBaseEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int IntProperty { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetIntProperty(int i);
    }
}
