using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using GeneratorAnnotations;

namespace SharedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface ICraftProgressInfo : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        long StartTime { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        float Duration { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        BaseItemResource ResultItem { get; set; }
    }
}