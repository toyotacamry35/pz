// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class ReactionsOwner
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IReactionsOwner_InvokeReaction_ReactionDef_ArgTuple_Message")]
            internal static async System.Threading.Tasks.Task InvokeReaction_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Reactions.ReactionDef reaction;
                (reaction, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Reactions.ReactionDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                ColonyShared.SharedCode.Entities.Reactions.ArgTuple[] args;
                (args, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ColonyShared.SharedCode.Entities.Reactions.ArgTuple[]>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((ColonyShared.SharedCode.Entities.Reactions.IReactionsOwner)__deltaObj__).InvokeReaction(reaction, args);
            }
        }
    }
}