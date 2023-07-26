// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class BuildPlace
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IBuildPlace_RemoveDelay_List_Message")]
            internal static async System.Threading.Tasks.Task RemoveDelay_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<SharedCode.DeltaObjects.Building.BuildType, System.Guid>> elements;
                (elements, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<SharedCode.DeltaObjects.Building.BuildType, System.Guid>>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Building.IBuildPlace)__deltaObj__).RemoveDelay(elements);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IBuildPlace_Check_BuildType_IPositionedBuildWrapper_Message")]
            internal static async System.Threading.Tasks.Task Check_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.DeltaObjects.Building.BuildType type;
                (type, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.DeltaObjects.Building.BuildType>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Entities.Building.IPositionedBuildWrapper buildWrapper;
                (buildWrapper, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.Building.IPositionedBuildWrapper>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Building.IBuildPlace)__deltaObj__).Check(type, buildWrapper);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IBuildPlace_Start_BuildType_IPositionedBuildWrapper_Message")]
            internal static async System.Threading.Tasks.Task Start_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.DeltaObjects.Building.BuildType type;
                (type, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.DeltaObjects.Building.BuildType>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Entities.Building.IPositionedBuildWrapper buildWrapper;
                (buildWrapper, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.Building.IPositionedBuildWrapper>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Building.IBuildPlace)__deltaObj__).Start(type, buildWrapper);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IBuildPlace_OnProgress_BuildType_Guid_Message")]
            internal static async System.Threading.Tasks.Task OnProgress_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.DeltaObjects.Building.BuildType type;
                (type, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.DeltaObjects.Building.BuildType>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid elementId;
                (elementId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Building.IBuildPlace)__deltaObj__).OnProgress(type, elementId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IBuildPlace_Operate_BuildType_Guid_Guid_OperationData_Message")]
            internal static async System.Threading.Tasks.Task Operate_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.DeltaObjects.Building.BuildType type;
                (type, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.DeltaObjects.Building.BuildType>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid callerId;
                (callerId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Guid elementId;
                (elementId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 2, chainContext, argumentRefs);
                SharedCode.Entities.Building.OperationData data;
                (data, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.Building.OperationData>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Building.IBuildPlace)__deltaObj__).Operate(type, callerId, elementId, data);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}