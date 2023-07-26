using System.Threading.Tasks;
using GeneratorAnnotations;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace SharedCode.AI
{
    public interface IHasAiTargetRecipient
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        IAiTargetRecipient AiTargetRecipient { get; set; } 
    }
    
    [GenerateDeltaObjectCode]
    public interface IAiTargetRecipient : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        OuterRef Target { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<bool> SetTarget(OuterRef target);
    }
}