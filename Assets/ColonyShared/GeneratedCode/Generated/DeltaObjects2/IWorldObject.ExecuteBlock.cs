// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class LimitedLifetime
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ILimitedLifetime_GetLimitedLifetimeDef__Message")]
            internal static async System.Threading.Tasks.Task GetLimitedLifetimeDef_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.ILimitedLifetime)__deltaObj__).GetLimitedLifetimeDef();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ILimitedLifetime_GetLifetimeLimit__Message")]
            internal static async System.Threading.Tasks.Task GetLifetimeLimit_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.ILimitedLifetime)__deltaObj__).GetLifetimeLimit();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ILimitedLifetime_StartCountdown__Message")]
            internal static async System.Threading.Tasks.Task StartCountdown_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.ILimitedLifetime)__deltaObj__).StartCountdown();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class OpenMechanics
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IOpenMechanics_FirstOpenedOrLastClosedExternalCall_Boolean_Message")]
            internal static async System.Threading.Tasks.Task FirstOpenedOrLastClosedExternalCall_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                bool isOpened;
                (isOpened, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.IOpenMechanics)__deltaObj__).FirstOpenedOrLastClosedExternalCall(isOpened);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IOpenMechanics_TryOpen_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task TryOpen_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Utils.OuterRef outerRef;
                (outerRef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Utils.OuterRef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IOpenMechanics)__deltaObj__).TryOpen(outerRef);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IOpenMechanics_TryClose_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task TryClose_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Utils.OuterRef outerRef;
                (outerRef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Utils.OuterRef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IOpenMechanics)__deltaObj__).TryClose(outerRef);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IOpenMechanics_IsEmpty__Message")]
            internal static async System.Threading.Tasks.Task IsEmpty_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.IOpenMechanics)__deltaObj__).IsEmpty();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class Traumas
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ITraumas_ChangeTraumaPoints_String_Int32_Message")]
            internal static async System.Threading.Tasks.Task ChangeTraumaPoints_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string traumaKey;
                (traumaKey, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                int delta;
                (delta, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.Entities.ITraumas)__deltaObj__).ChangeTraumaPoints(traumaKey, delta);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ITraumas_StartTrauma_String_Message")]
            internal static async System.Threading.Tasks.Task StartTrauma_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string traumaKey;
                (traumaKey, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.ITraumas)__deltaObj__).StartTrauma(traumaKey);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ITraumas_StopTrauma_String_Message")]
            internal static async System.Threading.Tasks.Task StopTrauma_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string traumaKey;
                (traumaKey, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.ITraumas)__deltaObj__).StopTrauma(traumaKey);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ITraumas_StartTrauma_String_ITraumaGiver_Message")]
            internal static async System.Threading.Tasks.Task StartTrauma_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string traumaKey;
                (traumaKey, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                Src.Aspects.Impl.Stats.ITraumaGiver traumaGiver;
                (traumaGiver, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Src.Aspects.Impl.Stats.ITraumaGiver>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.Entities.ITraumas)__deltaObj__).StartTrauma(traumaKey, traumaGiver);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ITraumas_StopTrauma_String_ITraumaGiver_Message")]
            internal static async System.Threading.Tasks.Task StopTrauma_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string traumaKey;
                (traumaKey, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                Src.Aspects.Impl.Stats.ITraumaGiver traumaGiver;
                (traumaGiver, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Src.Aspects.Impl.Stats.ITraumaGiver>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.Entities.ITraumas)__deltaObj__).StopTrauma(traumaKey, traumaGiver);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ITraumas_SuspendTrauma_String_ITraumaGiver_Message")]
            internal static async System.Threading.Tasks.Task SuspendTrauma_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string traumaKey;
                (traumaKey, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                Src.Aspects.Impl.Stats.ITraumaGiver traumaGiver;
                (traumaGiver, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Src.Aspects.Impl.Stats.ITraumaGiver>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.Entities.ITraumas)__deltaObj__).SuspendTrauma(traumaKey, traumaGiver);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "ITraumas_RemoveTrauma_String_ITraumaGiver_Message")]
            internal static async System.Threading.Tasks.Task RemoveTrauma_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string traumaKey;
                (traumaKey, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                Src.Aspects.Impl.Stats.ITraumaGiver traumaGiver;
                (traumaGiver, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Src.Aspects.Impl.Stats.ITraumaGiver>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.ITraumas)__deltaObj__).RemoveTrauma(traumaKey, traumaGiver);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "ITraumas_HasActiveTraumas_String_Message")]
            internal static async System.Threading.Tasks.Task HasActiveTraumas_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string[] traumas;
                (traumas, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string[]>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.ITraumas)__deltaObj__).HasActiveTraumas(traumas);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "ITraumas_RecalculateTraumas__Message")]
            internal static async System.Threading.Tasks.Task RecalculateTraumas_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.ITraumas)__deltaObj__).RecalculateTraumas();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class OwnerInformation
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IOwnerInformation_SetOwner_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task SetOwner_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> owner;
                (owner, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.IOwnerInformation)__deltaObj__).SetOwner(owner);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IOwnerInformation_SetLockPredicate_AccessPredicateDef_Message")]
            internal static async System.Threading.Tasks.Task SetLockPredicate_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Aspects.AccessRights.AccessPredicateDef accessPredicate;
                (accessPredicate, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Aspects.AccessRights.AccessPredicateDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.IOwnerInformation)__deltaObj__).SetLockPredicate(accessPredicate);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class LocomotionOwner
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ILocomotionOwner_IsValid__Message")]
            internal static async System.Threading.Tasks.Task IsValid_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.ILocomotionOwner)__deltaObj__).IsValid();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ILocomotionOwner_SetLocomotion_ILocomotionEngineAgent_IDirectMotionProducer_IGuideProvider_Message")]
            internal static async System.Threading.Tasks.Task SetLocomotion_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.Engine.ILocomotionEngineAgent locomotion;
                (locomotion, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.Engine.ILocomotionEngineAgent>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Entities.Engine.IDirectMotionProducer directMotionProducer;
                (directMotionProducer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.Engine.IDirectMotionProducer>(__data__, __offset__, 1, chainContext, argumentRefs);
                SharedCode.Entities.Engine.IGuideProvider guideProvider;
                (guideProvider, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.Engine.IGuideProvider>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((SharedCode.Entities.ILocomotionOwner)__deltaObj__).SetLocomotion(locomotion, directMotionProducer, guideProvider);
            }
        }
    }
}