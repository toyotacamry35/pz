// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class StatisticEngine
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IStatisticEngine_PostStatisticsEvent_StatisticEventArgs_Message")]
            internal static async System.Threading.Tasks.Task PostStatisticsEvent_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Quests.StatisticEventArgs arg;
                (arg, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Quests.StatisticEventArgs>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Engine.IStatisticEngine)__deltaObj__).PostStatisticsEvent(arg);
            }
        }
    }
}