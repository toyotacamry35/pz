using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.GameObjectAssembler.Res;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using Src.Aspects.Impl.Stats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedCode.Entities.Engine
{
    public interface IHasStatsEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IStatsEngine Stats { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IStatsEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] event Func<Task> StatsReparsedEvent;
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask InvokeOnStatsReparsedEvent();

        [ReplicationLevel(ReplicationLevel.Master)] ValueTask SetToDefault(bool hardReset);
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask RecalculateStats();
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask AddProxyStat(StatResource statRes, PropertyAddress propertyAddress);

        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)] StatsDef StatsDef { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<StatResource, ITimeStat> TimeStats { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<StatResource, ITimeStat> TimeStatsBroadcast { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<StatResource, IValueStat> ValueStats { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<StatResource, IValueStat> ValueStatsBroadcast { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<StatResource, IProxyStat> ProxyStats { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<StatResource, IProceduralStat> ProceduralStats { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<StatResource, IAccumulatedStat> AccumulatedStats { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<StatResource, IProceduralStat> ProceduralStatsBroadcast { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<StatResource, IAccumulatedStat> AccumulatedStatsBroadcast { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> ChangeValue(StatResource stat, float delta);
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> SetModifiers(StatModifierData[] modifiers, ModifierCauser causer);
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> UpdateModifiers(StatModifierData[] modifiers, ModifierCauser causer);
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> RemoveModifiers(StatModifierInfo[] modifiers, ModifierCauser causer);
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> RemoveModifiers(ModifierCauser causer);
        [ReplicationLevel(ReplicationLevel.Master), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask Copy(IStatsEngine statsEngine); // TODOA переделать по нормальному

        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<List<StatModifierProto>> GetSnapshot(StatType statType);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<(bool, float)> TryGetValue(StatResource statResource);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<IStat> GetStat(StatResource statResource);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<IStat> GetBroadcastStat(StatResource statResource);        

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] ValueTask<string> DumpStats(bool compactString);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<string> DumpStatsLocal(bool compactString);

        [ReplicationLevel(ReplicationLevel.Master)] Task GetBackFromIdleMode();
        [ReplicationLevel(ReplicationLevel.Master)] Task GoIntoIdleMode();
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientBroadcast)] long TimeWhenIdleStarted { get; set; }
    }
}