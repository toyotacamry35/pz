using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasBrute : IHasSpecificStats
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IBrute Brute { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IBrute : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Server)] [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetForwardDamageMultiplier();
        [ReplicationLevel(ReplicationLevel.Server)] [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetSideDamageMultiplier();
        [ReplicationLevel(ReplicationLevel.Server)] [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetBackwardDamageMultiplier();

        // Smthng like hit point (but, из-за рассинхрона анимации и спеллов может оказаться где угодно на траектории движения оружия. Поэтому пока передаём просто позицию игрока)
        [ReplicationLevel(ReplicationLevel.Server)]  ValueTask<Vector3> GetAggressionPoint();
        [ReplicationLevel(ReplicationLevel.Server)]  ValueTask</*IItem*/PropertyAddress> GetSlotItemAddr([CanBeNull] SlotDef weaponSlot);
    }
}
