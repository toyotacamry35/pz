using Assets.Src.Aspects;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.Aspects.Science;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.Src.SpawnSystem;
using Assets.ColonyShared.SharedCode.Entities;
using SharedCode.Utils;
using System.Linq;
using System;
using SharedCode.Entities;
using SharedCode.Entities.Engine;

namespace Assets.Src.Impacts
{
    public class ImpactAddKnowledgeFromObj : IImpactBinding<ImpactAddKnowledgeFromObjDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactAddKnowledgeFromObjDef def)
        {
            var repositiory = repo;
            ImpactAddKnowledgeFromObjDef selfDef = (ImpactAddKnowledgeFromObjDef)def;

            var entRef = await selfDef.KnowledgeHolder.Target.GetOuterRef(cast, repo);
            using (var knowEntContainer = await repo.Get(entRef.TypeId, entRef.Guid))
            {
                var ent = knowEntContainer.Get<IEntityObjectClientBroadcast>(entRef.TypeId, entRef.Guid, ReplicationLevel.ClientBroadcast);
                var selectors = (ent.Def as IHasKnowledgeDef)?.KnowledgeSelectors;
                foreach (var selector in selectors.Where(s => s.IsValid).Select(s => s.Target))
                {
                    if (selector.Knowledge.IsValid)
                    {
                        var knowDef = selector.Knowledge.Target;
                        if (knowDef != null)
                        {
                            var characterId = cast.Caster.Guid;
                            if (await CheckPredicate(characterId, knowDef, repo))
                                continue;

                            var needToAddKnow = !selector.Predicate.IsValid;
                            if (!needToAddKnow)
                            {
                                if (!SpellPredicates.TryGetPredicateBinding(selector.Predicate.Target, out var predicate))
                                    throw new Exception($"Can't find predicate {selector.Predicate.Target} in {selector}");
//                                var predicateType = DefToType.GetType(selector.Predicate.Target.GetType());
//                                var predicate = (ISpellPredicate)Activator.CreateInstance(predicateType);
                                needToAddKnow = await predicate.True(cast, repo, selector.Predicate.Target);
                            }
                            if (needToAddKnow)
                            {
                                var knowledgeDef = selector.Knowledge.Target;
                                var rewardPoints = selector.RewardPoints;
                                using (var wrapper = await repositiory.Get<IWorldCharacter>(cast.Caster.Guid))
                                {
                                    var entity = wrapper.Get<IWorldCharacter>(cast.Caster.Guid);
                                    using (var wrapper2 = await repositiory.Get<IKnowledgeEngine>(entity.KnowledgeEngine.Id))
                                    {
                                        var entity2 = wrapper2.Get<IKnowledgeEngine>(entity.KnowledgeEngine.Id);
                                        await entity2.Explore(knowledgeDef, rewardPoints);
                                    }
                                }
                            }
                            }
                    }
                }
            }
        }

        async Task<bool> CheckPredicate(Guid characterId, KnowledgeDef knowledgeDef, IEntitiesRepository repo)
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