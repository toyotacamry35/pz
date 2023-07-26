using System;
using SharedCode.EntitySystem;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using SharedCode.Entities.GameObjectEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Entities;
using ProtoBuf;
using SharedCode.MovementSync;
using GeneratorAnnotations;
using JetBrains.Annotations;
using SharedCode.Entities.Engine;
using SharedCode.AI;

namespace SharedCode.Entities.Mineable
{
    [GenerateDeltaObjectCode]
    public interface IMineableEntity : IEntity, IEntityObject, IWorldObject, IHasHealthWithCustomMechanics, IHasDestroyable, 
                                       IHasMortal, IHasBrute, IHasStatsEngine, IHasLifespan, IHasComputableStateMachine, 
                                       IHasSimpleMovementSync, IHasSpawnedObject, IIsDummyLegionary, IMountable
                                       , IHasOwner
    {
        // After this time elapsed, progress 'll be deleted
        float CurrProgressActualTime { get; }

        [ReplicationLevel(ReplicationLevel.Master)]  Task<LootTableBaseDef> GetLootTable();

        // `ClientBroadcast`(not `Server`), 'cos used in `MineableObject.IsDamageTypeEffectiveForMining` 
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  Task<IEnumerable<ProbabilisticItemPack>> GetMineableLootList(LootListRequest request);
        // It's probably no need & should be deleted:
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  Task<bool> IsDamageTypeEffectiveForMining(DamageTypeDef type);

        // --- Possible move to Implementation class ---------------------------
        [ReplicationLevel(ReplicationLevel.Server)]  Task<DamageTypeDef> GetDefaultDamageType();
        [ReplicationLevel(ReplicationLevel.Server)]  Task<ItemResourcePack> GetFillerResourcePack();
    }

    [ProtoContract]
    public struct ProbabilisticItemPack : IEquatable<ProbabilisticItemPack>
    //`struct` to be able to send via net
    {
        [ProtoMember(1)] public ItemResourcePack ItemPack { get; [UsedImplicitly]set; }
        [ProtoMember(2)] public float Chance { get; [UsedImplicitly]set; }

        public ProbabilisticItemPack(ItemResourcePack itemPack, float chance)
        {
            ItemPack = itemPack;
            Chance = chance;
        }

        public override string ToString()
        {
            return $"{nameof(ItemPack)}: {ItemPack.ToString() ?? "null"}, {nameof(Chance)}: {Chance}";
        }

        public override bool Equals(object obj)
        {
            return obj is ProbabilisticItemPack && Equals((ProbabilisticItemPack)obj);
        }

        public bool Equals(ProbabilisticItemPack other)
        {
            return EqualityComparer<ItemResourcePack>.Default.Equals(ItemPack, other.ItemPack) &&
                   Chance == other.Chance &&
                   IsDefault == other.IsDefault;
        }

        //TODO non readonly property Chance 
        public override int GetHashCode()
        {
            var hashCode = 829723942;
            hashCode = hashCode * -1521134295 + EqualityComparer<ItemResourcePack>.Default.GetHashCode(ItemPack);
            hashCode = hashCode * -1521134295 + Chance.GetHashCode();
            hashCode = hashCode * -1521134295 + IsDefault.GetHashCode();
            return hashCode;
        }

        public bool IsDefault => !ItemPack.IsValid && Chance.Equals(0f);

        public static bool operator ==(ProbabilisticItemPack pack1, ProbabilisticItemPack pack2)
        {
            return pack1.Equals(pack2);
        }

        public static bool operator !=(ProbabilisticItemPack pack1, ProbabilisticItemPack pack2)
        {
            return !(pack1 == pack2);
        }
    }
}
