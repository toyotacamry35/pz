// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IAttackEngineImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> StartAttackImpl(GeneratedCode.DeltaObjects.SpellPartCastId attackId, long finishTime, ColonyShared.SharedCode.Aspects.Misc.AttackDef attackDef, System.Collections.Generic.IReadOnlyList<ResourceSystem.Aspects.AttackModifierDef> modifiers);
        System.Threading.Tasks.Task FinishAttackImpl(GeneratedCode.DeltaObjects.SpellPartCastId attackId, long currentTime);
        System.Threading.Tasks.Task PushAttackTargetsImpl(GeneratedCode.DeltaObjects.SpellPartCastId attackId, System.Collections.Generic.List<ColonyShared.SharedCode.Aspects.Combat.AttackTargetInfo> targets);
        System.Threading.Tasks.Task SetAttackDoerImpl(ColonyShared.SharedCode.Aspects.Combat.IAttackDoer newDoer);
        System.Threading.Tasks.Task UnsetAttackDoerImpl(ColonyShared.SharedCode.Aspects.Combat.IAttackDoer oldDoer);
    }
}