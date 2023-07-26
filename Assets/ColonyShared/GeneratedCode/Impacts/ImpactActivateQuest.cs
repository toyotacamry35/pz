using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using SharedCode.Logging;
using ColonyShared.GeneratedCode.Shared.Aspects;
using JetBrains.Annotations;

namespace Src.Impacts
{
    [UsedImplicitly]
    public class ImpactActivateQuest : IImpactBinding<ImpactActivateQuestDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactActivateQuestDef indef)
        {
            var def = (ImpactActivateQuestDef)indef;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            await ActivateQuest(def.Quest, targetRef.To<IHasQuestEngineServer>(), def.RemoveQuest, repo);
        }

        public static async Task ActivateQuest(QuestDef quest, OuterRef<IHasQuestEngineServer> targetRef, bool removeQuest, IEntitiesRepository repo)
        {
            using (var ec = await repo.Get(targetRef))
            {
                ec.TryGet<IHasQuestEngineServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server, out var questEngine);
                if (questEngine != null)
                {
                    if (removeQuest)
                        await questEngine.Quest.RemoveQuest(quest);
                    else
                        await questEngine.Quest.AddQuest(quest);
                }
            }
        }
    }
}