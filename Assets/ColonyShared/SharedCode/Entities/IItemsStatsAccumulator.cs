using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using SharedCode.EntitySystem;
using MongoDB.Bson.Serialization.Attributes;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasItemsStatsAccumulator
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IItemsStatsAccumulator ItemsStatsAccumulator { get; set; }
    }

    // The main purpose is not shown by API. The Main purpose is in the implementation - it modifies stats on changes in any items container (incl. perksб etc.)
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IItemsStatsAccumulator : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]  bool HasActiveWeapon { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)]  DamageTypeDef ActiveWeaponDamageType { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)]  WeaponSizeDef ActiveWeaponSize { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]  Task<bool> SetHasActiveWeaponAndDamageType(bool has, DamageTypeDef dType, WeaponSizeDef size);
    }
}
