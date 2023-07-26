using Assets.ColonyShared.SharedCode.Entities;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Science;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Src.Predicates
{
    public class PredicateKnowledgeFromObj : IPredicateBinding<PredicateKnowledgeFromObjDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateKnowledgeFromObjDef def)
        {
            OuterRef<IEntity> entRef = default(OuterRef<IEntity>);
            if (((PredicateKnowledgeFromObjDef)def).KnowledgeHolder.Target is SpellCasterDef)
            {
                entRef = cast.Caster;
            }
            else if (((PredicateKnowledgeFromObjDef)def).KnowledgeHolder.Target is SpellTargetDef)
            {
                entRef = (cast.CastData as IWithTarget)?.Target ?? default(OuterRef<IEntity>);
            }
            if (!entRef.IsValid)
                return true;

            using (var knowEntContainer = await repo.Get(entRef.TypeId, entRef.Guid))
            {
                var ent = knowEntContainer.Get<IEntityObjectClientBroadcast>(entRef.TypeId, entRef.Guid, ReplicationLevel.ClientBroadcast);
                if (ent == null)
                    return true;
                var selectors = (ent.Def as IHasKnowledgeDef)?.KnowledgeSelectors;
                if (selectors != null)
                    foreach (var selector in selectors.Where(s => s.IsValid).Select(s => s.Target))
                    {
                        if (selector.Knowledge.IsValid)
                        {
                            var knowDef = selector.Knowledge.Target;
                            if (knowDef != null)
                            {
                                var characterId = cast.Caster.Guid;
                                if (await CheckKnowledge(characterId, knowDef, repo))
                                    continue;
                                if (!selector.Predicate.IsValid)
                                    return false;
                                if (!SpellPredicates.TryGetPredicateBinding(selector.Predicate.Target, out var predicate))
                                    throw new Exception($"Can't find predicate {selector.Predicate.Target} in {selector}");
//                              var predicateType = DefToType.GetType(selector.Predicate.Target.GetType());
//                              var predicate = (ISpellPredicate)Activator.CreateInstance(predicateType); // WTF тут было?
                                return !await predicate.True(cast, repo, selector.Predicate.Target);
                            }
                        }
                    }
            }
            return true;
        }


        async Task<bool> CheckKnowledge(Guid characterId, KnowledgeDef knowledgeDef, IEntitiesRepository repo)
        {
            if (repo == null || characterId == Guid.Empty)
                return false;

            using (var wrapper = await repo.Get<IKnowledgeEngineClientFull>(characterId))
            {
                var entity = wrapper?.Get<IKnowledgeEngineClientFull>(characterId);
                return entity?.KnownKnowledges?.Any(def => def == knowledgeDef) ?? false;
            }
        }
    }
}
