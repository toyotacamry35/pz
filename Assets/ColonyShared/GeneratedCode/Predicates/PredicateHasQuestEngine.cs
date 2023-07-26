using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates
{
    [UsedImplicitly]
    public class PredicateHasQuestEngine : IPredicateBinding<PredicateHasQuestEngineDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateHasQuestEngineDef indef)
        {
            var def = (PredicateHasQuestEngineDef)indef;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            if(!targetRef.IsValid)
                targetRef = cast.Caster;

            var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(targetRef.TypeId);
            return EntitiesRepository.IsImplements<IHasQuestEngine>(masterTypeId);
        }
    }
}