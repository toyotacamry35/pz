using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Src.Impacts
{
    public class ImpactSetQuestVisible : IImpactBinding<ImpactSetQuestVisibleDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSetQuestVisibleDef indef)
        {
            var def = (ImpactSetQuestVisibleDef)indef;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            await SetQuestVisible(def.Quest, targetRef, def.Visible, repo);
        }

        public static async Task SetQuestVisible(QuestDef quest, OuterRef<IEntity> targetRef, bool visible, IEntitiesRepository repo)
        {
            using (var ec = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var questEngine = ec.Get<IHasQuestEngineServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                await questEngine.Quest.SetVisible(quest, visible);
            }
        }
    }
}