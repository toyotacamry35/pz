using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Impacts;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.Entities;

namespace Assets.Src.Effects
{
    [UsedImplicitly]
    class EffectActivateQuest : IEffectBinding<EffectActivateQuestDef>
    {
        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectActivateQuestDef def)
        {
            var selfDef = (EffectActivateQuestDef)def;
            OuterRef<IEntity> targetRef = cast.Caster;
            if (selfDef.Target.Target != null)
                targetRef = await selfDef.Target.Target.GetOuterRef(cast, repo);
            if (!targetRef.IsValid)
                targetRef = cast.Caster;
            await ImpactActivateQuest.ActivateQuest(selfDef.Quest, targetRef.To<IHasQuestEngineServer>(), false, repo);
        }
        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectActivateQuestDef def)
        {
            var selfDef = (EffectActivateQuestDef)def;
            OuterRef<IEntity> targetRef = cast.Caster;
            if (selfDef.Target.Target != null)
                targetRef = await selfDef.Target.Target.GetOuterRef(cast, repo);
            if (!targetRef.IsValid)
                targetRef = cast.Caster;
            await ImpactActivateQuest.ActivateQuest(selfDef.Quest, targetRef.To<IHasQuestEngineServer>(), true, repo);
        }
    }
}
