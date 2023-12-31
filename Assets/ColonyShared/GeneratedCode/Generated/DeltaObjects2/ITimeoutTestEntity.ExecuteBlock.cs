// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutTestEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ITimeoutTestEntity_LongUsage__Message")]
            internal static async System.Threading.Tasks.Task LongUsage_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((GeneratedCode.EntityModel.Test.ITimeoutTestEntity)__deltaObj__).LongUsage();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ITimeoutTestEntity_ShortUsage__Message")]
            internal static async System.Threading.Tasks.Task ShortUsage_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((GeneratedCode.EntityModel.Test.ITimeoutTestEntity)__deltaObj__).ShortUsage();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ITimeoutTestEntity_SetTestProperty_Int32_Message")]
            internal static async System.Threading.Tasks.Task SetTestProperty_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutTestEntity)__deltaObj__).SetTestProperty(value);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ITimeoutTestEntity_AwaitWriteTimeSec_Single_Message")]
            internal static async System.Threading.Tasks.Task AwaitWriteTimeSec_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float seconds;
                (seconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutTestEntity)__deltaObj__).AwaitWriteTimeSec(seconds);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ITimeoutTestEntity_AwaitWriteTimeSecAndSetTestProperty_Single_Int32_Message")]
            internal static async System.Threading.Tasks.Task AwaitWriteTimeSecAndSetTestProperty_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float seconds;
                (seconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                int value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutTestEntity)__deltaObj__).AwaitWriteTimeSecAndSetTestProperty(seconds, value);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ITimeoutTestEntity_AwaitReadTimeSec_Single_Message")]
            internal static async System.Threading.Tasks.Task AwaitReadTimeSec_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float seconds;
                (seconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutTestEntity)__deltaObj__).AwaitReadTimeSec(seconds);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "ITimeoutTestEntity_AwaitWriteTimeSecAndCallSubTestEntityRpcWithAwait_Single_Single_Int32_Message")]
            internal static async System.Threading.Tasks.Task AwaitWriteTimeSecAndCallSubTestEntityRpcWithAwait_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float seconds;
                (seconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                float subseconds;
                (subseconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 1, chainContext, argumentRefs);
                int value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutTestEntity)__deltaObj__).AwaitWriteTimeSecAndCallSubTestEntityRpcWithAwait(seconds, subseconds, value);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutSubTestEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ITimeoutSubTestEntity_LongUsage__Message")]
            internal static async System.Threading.Tasks.Task LongUsage_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity)__deltaObj__).LongUsage();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ITimeoutSubTestEntity_ShortUsage__Message")]
            internal static async System.Threading.Tasks.Task ShortUsage_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity)__deltaObj__).ShortUsage();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ITimeoutSubTestEntity_SetTestProperty_Int32_Message")]
            internal static async System.Threading.Tasks.Task SetTestProperty_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity)__deltaObj__).SetTestProperty(value);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ITimeoutSubTestEntity_AwaitWriteTimeSec_Single_Message")]
            internal static async System.Threading.Tasks.Task AwaitWriteTimeSec_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float seconds;
                (seconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity)__deltaObj__).AwaitWriteTimeSec(seconds);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ITimeoutSubTestEntity_AwaitWriteTimeSecAndSetTestProperty_Single_Int32_Message")]
            internal static async System.Threading.Tasks.Task AwaitWriteTimeSecAndSetTestProperty_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float seconds;
                (seconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                int value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity)__deltaObj__).AwaitWriteTimeSecAndSetTestProperty(seconds, value);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ITimeoutSubTestEntity_AwaitReadTimeSec_Single_Message")]
            internal static async System.Threading.Tasks.Task AwaitReadTimeSec_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float seconds;
                (seconds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity)__deltaObj__).AwaitReadTimeSec(seconds);
            }
        }
    }
}