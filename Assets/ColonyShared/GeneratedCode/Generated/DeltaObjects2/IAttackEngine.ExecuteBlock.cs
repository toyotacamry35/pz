// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class AttackEngine
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IAttackEngine_StartAttack_SpellPartCastId_Int64_AttackDef_IReadOnlyList_Message")]
            internal static async System.Threading.Tasks.Task StartAttack_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                GeneratedCode.DeltaObjects.SpellPartCastId attackId;
                (attackId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<GeneratedCode.DeltaObjects.SpellPartCastId>(__data__, __offset__, 0, chainContext, argumentRefs);
                long finishTime;
                (finishTime, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<long>(__data__, __offset__, 1, chainContext, argumentRefs);
                ColonyShared.SharedCode.Aspects.Misc.AttackDef attackDef;
                (attackDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ColonyShared.SharedCode.Aspects.Misc.AttackDef>(__data__, __offset__, 2, chainContext, argumentRefs);
                System.Collections.Generic.IReadOnlyList<ResourceSystem.Aspects.AttackModifierDef> modifiers;
                (modifiers, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.IReadOnlyList<ResourceSystem.Aspects.AttackModifierDef>>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((ColonyShared.SharedCode.Aspects.Combat.IAttackEngine)__deltaObj__).StartAttack(attackId, finishTime, attackDef, modifiers);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IAttackEngine_FinishAttack_SpellPartCastId_Int64_Message")]
            internal static async System.Threading.Tasks.Task FinishAttack_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                GeneratedCode.DeltaObjects.SpellPartCastId attackId;
                (attackId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<GeneratedCode.DeltaObjects.SpellPartCastId>(__data__, __offset__, 0, chainContext, argumentRefs);
                long currentTime;
                (currentTime, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<long>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((ColonyShared.SharedCode.Aspects.Combat.IAttackEngine)__deltaObj__).FinishAttack(attackId, currentTime);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IAttackEngine_PushAttackTargets_SpellPartCastId_List_Message")]
            internal static async System.Threading.Tasks.Task PushAttackTargets_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                GeneratedCode.DeltaObjects.SpellPartCastId attackId;
                (attackId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<GeneratedCode.DeltaObjects.SpellPartCastId>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Collections.Generic.List<ColonyShared.SharedCode.Aspects.Combat.AttackTargetInfo> targets;
                (targets, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<ColonyShared.SharedCode.Aspects.Combat.AttackTargetInfo>>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((ColonyShared.SharedCode.Aspects.Combat.IAttackEngine)__deltaObj__).PushAttackTargets(attackId, targets);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IAttackEngine_SetAttackDoer_IAttackDoer_Message")]
            internal static async System.Threading.Tasks.Task SetAttackDoer_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ColonyShared.SharedCode.Aspects.Combat.IAttackDoer newDoer;
                (newDoer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ColonyShared.SharedCode.Aspects.Combat.IAttackDoer>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((ColonyShared.SharedCode.Aspects.Combat.IAttackEngine)__deltaObj__).SetAttackDoer(newDoer);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IAttackEngine_UnsetAttackDoer_IAttackDoer_Message")]
            internal static async System.Threading.Tasks.Task UnsetAttackDoer_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ColonyShared.SharedCode.Aspects.Combat.IAttackDoer oldDoer;
                (oldDoer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ColonyShared.SharedCode.Aspects.Combat.IAttackDoer>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((ColonyShared.SharedCode.Aspects.Combat.IAttackEngine)__deltaObj__).UnsetAttackDoer(oldDoer);
            }
        }
    }
}