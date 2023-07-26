using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    public interface IItem : IDeltaObject, IHasId, IHasStatsEngine, IHasHealthWithCustomMechanics
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] BaseItemResource ItemResource { get; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IContainer AmmoContainer { get; set; }
    }

    public interface IHasId
    {
        [ReplicationLevel(ReplicationLevel.Always)] Guid Id { get; set; }
    }
}
