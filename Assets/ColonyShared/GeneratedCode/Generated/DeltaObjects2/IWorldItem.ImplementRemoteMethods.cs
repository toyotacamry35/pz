// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IItemImplementRemoteMethods
    {
        System.Threading.Tasks.ValueTask<Assets.ColonyShared.SharedCode.Aspects.Damage.DamageResult> ReceiveDamageInternalImpl(Assets.ColonyShared.SharedCode.Aspects.Damage.Damage incomingDamage, System.Guid attackerId, int attackerTypeId);
        System.Threading.Tasks.ValueTask<bool> ChangeHealthInternalImpl(float health);
    }
}