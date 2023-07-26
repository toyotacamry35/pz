using SharedCode.Entities;
using SharedCode.EntitySystem;
using GeneratorAnnotations;

namespace SharedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface ISlotItem: IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int Stack { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IItem Item { get; set; }
    }
}
