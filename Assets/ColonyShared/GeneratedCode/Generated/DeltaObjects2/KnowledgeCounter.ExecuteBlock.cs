// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class KnowledgeCounter
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IKnowledgeCounter_PreventOnCompleteEvent__Message")]
            internal static async System.Threading.Tasks.Task PreventOnCompleteEvent_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObj__).PreventOnCompleteEvent();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IKnowledgeCounter_OnInit_QuestDef_QuestCounterDef_IEntitiesRepository_Message")]
            internal static async System.Threading.Tasks.Task OnInit_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef;
                (questDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.Aspects.Impl.Factions.Template.QuestDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef;
                (counterDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef>(__data__, __offset__, 1, chainContext, argumentRefs);
                SharedCode.EntitySystem.IEntitiesRepository repository;
                (repository, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.IEntitiesRepository>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObj__).OnInit(questDef, counterDef, repository);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IKnowledgeCounter_OnDatabaseLoad_IEntitiesRepository_Message")]
            internal static async System.Threading.Tasks.Task OnDatabaseLoad_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.IEntitiesRepository repository;
                (repository, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.IEntitiesRepository>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObj__).OnDatabaseLoad(repository);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IKnowledgeCounter_OnDestroy_IEntitiesRepository_Message")]
            internal static async System.Threading.Tasks.Task OnDestroy_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.IEntitiesRepository repository;
                (repository, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.IEntitiesRepository>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObj__).OnDestroy(repository);
            }
        }
    }
}