using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Factions.Template;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using SharedCode.DeltaObjects;
using SharedCode.Entities.Mineable;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem.Delta;
using SharedCode.Refs;
using SharedCode.Serializers.Protobuf;
using SharedCode.Utils;
using System;
using System.Threading.Tasks;

namespace SharedCode.Entities.Engine
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IQuestEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<QuestDef, IQuestObject> Quests { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task AddQuest(QuestDef quest);
        [ReplicationLevel(ReplicationLevel.Server)] Task AddQuestObject(QuestDef questDef);
        [ReplicationLevel(ReplicationLevel.Server)] Task ChangePhase(QuestDef questDef, int newPhaseIndex);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task RemoveQuest(QuestDef quest);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> RemoveAllQuests();
        [ReplicationLevel(ReplicationLevel.Server)] Task SetVisible(QuestDef quest, bool visible);

        [ReplicationLevel(ReplicationLevel.Server)] Task OnDatabaseLoad();
    }
    public enum QuestStatus
    {
        None,
        InProgress,
        Fail,
        Sucess
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IQuestObject : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)] QuestDef QuestDef { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] QuestStatus Status { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] int PhaseIndex { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] bool IsVisible { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] IQuestCounter PhaseSuccCounter { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] IQuestCounter PhaseFailCounter { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] bool HavePhaseSuccCounter { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] bool HavePhaseFailCounter { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)] Task SetStatus(QuestStatus newStatus);
        [ReplicationLevel(ReplicationLevel.Server)] Task AddPhaseCounters(IQuestCounter succCounter, IQuestCounter failCounter);
    }
}