using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;

namespace Assets.Src.RubiconAI.KnowledgeSystem
{
    public interface IKnowledgeSource
    {
        BaseResource GetId();
        KnowledgeSourceDef Def { get; set; }
        ValueTask LoadDef(Knowledge knowledge, KnowledgeSourceDef def);
        ValueTask UpdateKnowledge();
        KnowledgeCategoryDef Category { get; }
        Dictionary<OuterRef<IEntity>, VisibilityDataSample> Legionaries { get; }
        event Func<OuterRef<IEntity>, ValueTask> OnLearnedAboutLegionary;
        event Func<OuterRef<IEntity>, ValueTask> OnForgotAboutLegionary;

    }

    public static class InvokeExt
    {
        public static async ValueTask InvokeAndWaitAll(this Func<OuterRef<IEntity>, ValueTask> func, OuterRef<IEntity> arg)
        {
            if (func != null)
                foreach (var f in func.GetInvocationList())
                {
                    await ((Func<OuterRef<IEntity>, ValueTask>)f).Invoke(arg);
                }
        }
    }
}
