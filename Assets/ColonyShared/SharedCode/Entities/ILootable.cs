using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using SharedCode.Entities;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasLootable
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ILootable Lootable { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ILootable : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<LootTableBaseDef> GetLootTable();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<IEnumerable<ItemResourcePack>> GetLootList(Guid looterId);
    }
}
