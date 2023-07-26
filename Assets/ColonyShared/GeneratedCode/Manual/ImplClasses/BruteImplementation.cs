using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using JetBrains.Annotations;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Core.Environment.Logging.Extension;
using Vector3 = SharedCode.Utils.Vector3;

namespace GeneratedCode.DeltaObjects
{
    public partial class Brute : IBruteImplementRemoteMethods
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        //#todo: may be use some aggression point offset (which probably could change on equip weapon (& may be even on every hit of different рше-types))
        public ValueTask<Vector3> GetAggressionPointImpl()
        {
            var worldObj = PositionedObjectHelper.GetPositioned(parentEntity);
            if (worldObj == null)
            {
                Logger.IfError()?.Message($"Entity {parentEntity.Id} (type: {parentEntity.TypeId}) is not {nameof(IPositionedObject)}.").Write();
                return new ValueTask<Vector3>(Vector3.Default);
            }

            return new ValueTask<Vector3>(worldObj.Transform.Position);
        }

        public ValueTask<PropertyAddress> GetSlotItemAddrImpl([CanBeNull] SlotDef weaponSlot)
        {
            if (weaponSlot == null)
                return new ValueTask<PropertyAddress>(null as PropertyAddress);

            var dollOwner = parentEntity as IHasDoll;
            if (dollOwner == null)
            {
                Logger.IfError()?.Message($"Entity is not {nameof(IHasDoll)}.").Write();
                return new ValueTask<PropertyAddress>(null as PropertyAddress);
            }

            if (!dollOwner.Doll.Items.ContainsKey(weaponSlot.SlotId))
                return new ValueTask<PropertyAddress>(null as PropertyAddress);

            return new ValueTask<PropertyAddress>(EntityPropertyResolver.GetPropertyAddress(dollOwner.Doll.Items[weaponSlot.SlotId].Item));
        }

        private IBruteDef BruteDef => (parentEntity as IEntityObject)?.Def as IBruteDef;

        public ValueTask<float> GetForwardDamageMultiplierImpl() => new ValueTask<float>(BruteDef?.ForwardDamageMultiplier ?? 0.0f);
        public ValueTask<float> GetSideDamageMultiplierImpl() => new ValueTask<float>(BruteDef?.SideDamageMultiplier ?? 0.0f);

        public ValueTask<float> GetBackwardDamageMultiplierImpl() => new ValueTask<float>(BruteDef?.BackwardDamageMultiplier ?? 0.0f);
        public ValueTask<StatisticType> GetObjectTypeImpl() => new ValueTask<StatisticType>(BruteDef?.ObjectType.Target);
    }
}
