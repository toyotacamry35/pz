using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.Serializers.Protobuf;
using System;
using System.Threading.Tasks;

namespace SharedCode.Entities.Engine
{
    [GenerateDeltaObjectCode]
    public interface IQuestCounter : IDeltaObject
    {
        event Func<QuestDef, IQuestCounter, Task> OnCounterCompleted;
        event Func<IQuestCounter, Task> OnCounterChanged;

        QuestDef QuestDef { get; set; }
        [LockFreeReadonlyProperty]
        QuestCounterDef CounterDef { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] int Count { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] int CountForClient { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] bool Completed { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)] bool PreventOnComplete { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] Task PreventOnCompleteEvent();

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task OnInit(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository);

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task OnDatabaseLoad(IEntitiesRepository repository);

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task OnDestroy(IEntitiesRepository repository);
    }
}
