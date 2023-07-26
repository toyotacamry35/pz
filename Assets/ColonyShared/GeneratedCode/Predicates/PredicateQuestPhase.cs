using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;

namespace Assets.Src.Predicates
{
    public class PredicateQuestPhase : IPredicateBinding<PredicateQuestPhaseDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateQuestPhaseDef def)
        {
            var selfDef = (PredicateQuestPhaseDef)def;

            var targetRef = cast.Caster;
            if (selfDef.Target.Target != null)
                targetRef = await selfDef.Target.Target.GetOuterRef(cast, repository);

            if(!targetRef.IsValid)
                targetRef = cast.Caster;

            int? questPhase = null;
            using (var wrapper = await repository.Get(targetRef.TypeId, targetRef.Guid))
            {
                var entity = wrapper?.Get<IHasQuestEngineClientFull>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientFull);
                if (entity != null && entity.Quest.Quests.TryGetValue(selfDef.Quest, out IQuestObjectClientFull questObject))
                    questPhase = questObject?.PhaseIndex;
            }

            if (questPhase.HasValue)
            {
                var currVal = questPhase.Value;
                var value = selfDef.Phases;
                switch (selfDef.Type)
                {
                    case ComprasionType.More:
                        return currVal > value;
                    case ComprasionType.Less:
                        return currVal < value;
                    case ComprasionType.Equal:
                        return currVal == value;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }
    }
}