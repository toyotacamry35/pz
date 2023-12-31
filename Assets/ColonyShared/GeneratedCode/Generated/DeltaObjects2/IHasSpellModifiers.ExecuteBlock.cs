// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SpellModifiersCollector
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ISpellModifiersCollector_AddModifiers_SpellModifiersCauser_PredicateDef_SpellModifierDef_Message")]
            internal static async System.Threading.Tasks.Task AddModifiers_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ColonyShared.SharedCode.Modifiers.SpellModifiersCauser causer;
                (causer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ColonyShared.SharedCode.Modifiers.SpellModifiersCauser>(__data__, __offset__, 0, chainContext, argumentRefs);
                Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.PredicateDef condition;
                (condition, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.PredicateDef>(__data__, __offset__, 1, chainContext, argumentRefs);
                ResourceSystem.Aspects.SpellModifierDef[] modifiers;
                (modifiers, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Aspects.SpellModifierDef[]>(__data__, __offset__, 2, chainContext, argumentRefs);
                var __result__ = await ((ColonyShared.SharedCode.Modifiers.ISpellModifiersCollector)__deltaObj__).AddModifiers(causer, condition, modifiers);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ISpellModifiersCollector_RemoveModifiers_SpellModifiersCauser_Message")]
            internal static async System.Threading.Tasks.Task RemoveModifiers_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ColonyShared.SharedCode.Modifiers.SpellModifiersCauser causer;
                (causer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ColonyShared.SharedCode.Modifiers.SpellModifiersCauser>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((ColonyShared.SharedCode.Modifiers.ISpellModifiersCollector)__deltaObj__).RemoveModifiers(causer);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}