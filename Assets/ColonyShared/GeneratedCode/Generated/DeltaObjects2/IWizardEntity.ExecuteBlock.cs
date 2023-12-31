// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WizardEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IWizardEntity_NewSpellId__Message")]
            internal static async System.Threading.Tasks.Task NewSpellId_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).NewSpellId();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IWizardEntity_ConnectToHostAsReplica_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task ConnectToHostAsReplica_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.Wizardry.IWizardEntity> host;
                (host, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.Wizardry.IWizardEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).ConnectToHostAsReplica(host);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IWizardEntity_CastSpellFromHost_SpellId_SpellCast_Message")]
            internal static async System.Threading.Tasks.Task CastSpellFromHost_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellId id;
                (id, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellCast spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellCast>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).CastSpellFromHost(id, spell);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IWizardEntity_StopSpellFromHost_SpellId_SpellFinishReason_Int64_Message")]
            internal static async System.Threading.Tasks.Task StopSpellFromHost_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellId id;
                (id, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellFinishReason reason;
                (reason, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellFinishReason>(__data__, __offset__, 1, chainContext, argumentRefs);
                long timeStamp;
                (timeStamp, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<long>(__data__, __offset__, 2, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).StopSpellFromHost(id, reason, timeStamp);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IWizardEntity_SpellFinishedDelay_SpellId_Message")]
            internal static async System.Threading.Tasks.Task SpellFinishedDelay_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellId spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).SpellFinishedDelay(spell);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IWizardEntity_OnLostPossiblyImportantEntity_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task OnLostPossiblyImportantEntity_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent;
                (ent, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).OnLostPossiblyImportantEntity(ent);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "IWizardEntity_WatchdogUpdate__Message")]
            internal static async System.Threading.Tasks.Task WatchdogUpdate_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).WatchdogUpdate();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "IWizardEntity_StopAllSpellsOfGroup_SpellGroupDef_SpellId_SpellFinishReason_Message")]
            internal static async System.Threading.Tasks.Task StopAllSpellsOfGroup_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellGroupDef group;
                (group, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellGroupDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellId except;
                (except, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 1, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellFinishReason reason;
                (reason, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellFinishReason>(__data__, __offset__, 2, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).StopAllSpellsOfGroup(group, except, reason);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "IWizardEntity_StopSpellByDef_SpellDef_SpellId_SpellFinishReason_Message")]
            internal static async System.Threading.Tasks.Task StopSpellByDef_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellDef spellDef;
                (spellDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellId except;
                (except, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 1, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellFinishReason reason;
                (reason, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellFinishReason>(__data__, __offset__, 2, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).StopSpellByDef(spellDef, except, reason);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "IWizardEntity_StopSpellByCauser_SpellPartCastId_SpellFinishReason_Message")]
            internal static async System.Threading.Tasks.Task StopSpellByCauser_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                GeneratedCode.DeltaObjects.SpellPartCastId causer;
                (causer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<GeneratedCode.DeltaObjects.SpellPartCastId>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellFinishReason reason;
                (reason, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellFinishReason>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).StopSpellByCauser(causer, reason);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "IWizardEntity_HasActiveSpell_SpellDef_Message")]
            internal static async System.Threading.Tasks.Task HasActiveSpell_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellDef spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).HasActiveSpell(spell);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(11, "IWizardEntity_HasActiveSpellGroup_SpellGroupDef_Message")]
            internal static async System.Threading.Tasks.Task HasActiveSpellGroup_11(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellGroupDef group;
                (group, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellGroupDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).HasActiveSpellGroup(group);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(12, "IWizardEntity_DumpEvents__Message")]
            internal static async System.Threading.Tasks.Task DumpEvents_12(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).DumpEvents();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(13, "IWizardEntity_LocalUpdateTimeLineData__Message")]
            internal static async System.Threading.Tasks.Task LocalUpdateTimeLineData_13(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).LocalUpdateTimeLineData();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(14, "IWizardEntity_CheckSpellCastPredicates_Int64_SpellCast_List_PredicateIgnoreGroupDef_Message")]
            internal static async System.Threading.Tasks.Task CheckSpellCastPredicates_14(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                long currentTime;
                (currentTime, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<long>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellCast spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellCast>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Collections.Generic.List<SharedCode.Wizardry.SpellPredicateDef> failedPredicates;
                (failedPredicates, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<SharedCode.Wizardry.SpellPredicateDef>>(__data__, __offset__, 2, chainContext, argumentRefs);
                Assets.ResourceSystem.Arithmetic.Templates.Predicates.PredicateIgnoreGroupDef predicateIgnoreGroupDef;
                (predicateIgnoreGroupDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ResourceSystem.Arithmetic.Templates.Predicates.PredicateIgnoreGroupDef>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).CheckSpellCastPredicates(currentTime, spell, failedPredicates, predicateIgnoreGroupDef);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(15, "IWizardEntity_HasSpellsPreventingThisFromStart_SpellCast_Message")]
            internal static async System.Threading.Tasks.Task HasSpellsPreventingThisFromStart_15(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellCast spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellCast>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).HasSpellsPreventingThisFromStart(spell);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(16, "IWizardEntity_CastSpell_SpellCast_Message")]
            internal static async System.Threading.Tasks.Task CastSpell_16(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellCast spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellCast>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).CastSpell(spell);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(17, "IWizardEntity_CastSpell_SpellCast_SpellId_Message")]
            internal static async System.Threading.Tasks.Task CastSpell_17(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellCast spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellCast>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellId clientSpellId;
                (clientSpellId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).CastSpell(spell, clientSpellId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(18, "IWizardEntity_CastSpell_SpellCast_SpellId_SpellId_Message")]
            internal static async System.Threading.Tasks.Task CastSpell_18(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellCast spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellCast>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellId clientSpellId;
                (clientSpellId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 1, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellId prevSpell;
                (prevSpell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 2, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).CastSpell(spell, clientSpellId, prevSpell);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(19, "IWizardEntity_StopCastSpell_SpellId_SpellFinishReason_Message")]
            internal static async System.Threading.Tasks.Task StopCastSpell_19(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellId spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Wizardry.SpellFinishReason reason;
                (reason, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellFinishReason>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).StopCastSpell(spell, reason);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(20, "IWizardEntity_StopCastSpell_SpellId_Message")]
            internal static async System.Threading.Tasks.Task StopCastSpell_20(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellId spell;
                (spell, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).StopCastSpell(spell);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(21, "IWizardEntity_Update__Message")]
            internal static async System.Threading.Tasks.Task Update_21(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).Update();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(22, "IWizardEntity_Update_SpellId_Message")]
            internal static async System.Threading.Tasks.Task Update_22(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellId spellId;
                (spellId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).Update(spellId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(23, "IWizardEntity_GetDebugData__Message")]
            internal static async System.Threading.Tasks.Task GetDebugData_23(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).GetDebugData();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(24, "IWizardEntity_WizardHasDied__Message")]
            internal static async System.Threading.Tasks.Task WizardHasDied_24(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).WizardHasDied();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(25, "IWizardEntity_WizardHasRisen__Message")]
            internal static async System.Threading.Tasks.Task WizardHasRisen_25(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).WizardHasRisen();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(26, "IWizardEntity_GetBackFromIdleMode__Message")]
            internal static async System.Threading.Tasks.Task GetBackFromIdleMode_26(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).GetBackFromIdleMode();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(27, "IWizardEntity_GoIntoIdleMode__Message")]
            internal static async System.Threading.Tasks.Task GoIntoIdleMode_27(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).GoIntoIdleMode();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(28, "IWizardEntity_CancelSpell_SpellId_Message")]
            internal static async System.Threading.Tasks.Task CancelSpell_28(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Wizardry.SpellId spellId;
                (spellId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Wizardry.SpellId>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).CancelSpell(spellId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(29, "IWizardEntity_SetIsInterestingEnoughToLog_Boolean_Message")]
            internal static async System.Threading.Tasks.Task SetIsInterestingEnoughToLog_29(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                bool enable;
                (enable, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Wizardry.IWizardEntity)__deltaObj__).SetIsInterestingEnoughToLog(enable);
            }
        }
    }
}