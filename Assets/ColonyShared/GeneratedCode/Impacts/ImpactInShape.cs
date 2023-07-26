using Assets.Src.Arithmetic;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vector3 = SharedCode.Utils.Vector3;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.ColonyShared.GeneratedCode.Impacts.ShapeUtils;
using SharedCode.Entities;
using System.Linq;
using SharedCode.MovementSync;
using Shared.ManualDefsForSpells;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Impacts
{
    public class ImpactInShape : IImpactBinding<ImpactInShapeDef>, IImpactBinding<ImpactForAllInBoxDef>, IImpactBinding<ImpactNearestInBoxDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactInShapeDef def)
        {
            Transform casterTransform = default;
            Guid spaceId = default;
            VisibilityDataSample targetData = default;


            var casterOref = cast.Caster;
            var targetOref = cast.CastData.GetTarget();

            var data = await GetDataForQuerySystem(casterOref, targetOref, repo, def.Shape.Target.CheckTargetOnly);
            casterTransform = data.CasterTransform;
            spaceId = data.SpaceId;
            targetData = data.TargetData;
            
            var targetsBeforePredicate = await ShapeQuerySystem.FindAffectedTargets(
                cast.Caster,
                spaceId,
                casterTransform,
                cast,
                def.Shape,
                repo,
                targetData);
            
            CheckPredicateAndCast(cast, repo, targetsBeforePredicate, def.PredicateOnTarget, def.AppliedSpells);
        }

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactForAllInBoxDef def)
        {
            Transform casterTransform = default;
            Guid spaceId = default;
            VisibilityDataSample targetData = default;

            var data = await GetDataForQuerySystem(cast.Caster, cast.CastData.GetTarget(), repo, false);
            casterTransform = data.CasterTransform;
            spaceId = data.SpaceId;
            targetData = data.TargetData;

            var box = new BoxShapeDef();

            if (def.AttackBoxes != null && def.AttackBoxes.Length > 0)
            {
                box.Position = def.AttackBoxes[0].center;
                box.Extents = def.AttackBoxes[0].extents;
            }
            else
                Logger.IfError()?.Message("{0} have incorrect AttackBoxes def!", nameof(ImpactForAllInBox)).Write();

            var targetsBeforePredicate = await ShapeQuerySystem.FindAffectedTargets(
                cast.Caster,
                spaceId,
                casterTransform,
                cast,
                box,
                repo,
                targetData); 
            
            CheckPredicateAndCast(cast, repo, targetsBeforePredicate, def.PredicateOnTarget, def.AppliedSpells);
        }

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactNearestInBoxDef def)
        {
            Transform casterTransform = default;
            Guid spaceId = default;
            VisibilityDataSample targetData = default;

            var data = await GetDataForQuerySystem(cast.Caster, cast.CastData.GetTarget(), repo, true);
            casterTransform = data.CasterTransform;
            spaceId = data.SpaceId;
            targetData = data.TargetData;

            var box = new BoxShapeDef();

            if (def.AttackBoxes != null && def.AttackBoxes.Length > 0)
            {
                box.Position = def.AttackBoxes[0].center;
                box.Extents = def.AttackBoxes[0].extents;
            }
            else
                Logger.IfError()?.Message("{0} have incorrect AttackBoxes def!", nameof(ImpactNearestInBox)).Write();

            var targetsBeforePredicate = await ShapeQuerySystem.FindAffectedTargets(
                cast.Caster,
                spaceId,
                casterTransform,
                cast,
                box,
                repo,
                targetData);

            CheckPredicateAndCast(cast, repo, targetsBeforePredicate, def.PredicateOnTarget, def.AppliedSpells);
        }
        void CastToTargets(OuterRef<IWizardEntity> wizard, IEnumerable<OuterRef<IEntity>> targets, IEnumerable<ResourceRef<SpellDef>> spells, IEntitiesRepository repo)
        {
            foreach (var target in targets)
                AsyncUtils.RunAsyncTask(async () =>
                {
                    await CastToTarget(wizard, target, spells, repo);
                }, repo);
        }
        async Task CastToTarget(OuterRef<IWizardEntity> wizard, OuterRef<IEntity> target, IEnumerable<ResourceRef<SpellDef>> spells, IEntitiesRepository repo)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Cast to target | Target:{target} Spells:[{string.Join(", ", spells.Select(x => x.Target.____GetDebugAddress()))}]").Write();
            using (var wizardC = await repo.Get(wizard.TypeId, wizard.Guid))
            {
                foreach (var spell in spells)
                    if (spell != null)
                    {
                        var order = new SpellCastWithTarget(null) { Def = spell, Target = target };
                        var orderId = await wizardC.Get<IWizardEntityServer>(wizard.TypeId, wizard.Guid, ReplicationLevel.Server).CastSpell(order);
                    }
            }

        }
        public static async Task<(Transform CasterTransform, Guid SpaceId, VisibilityDataSample TargetData)> GetDataForQuerySystem(OuterRef<IEntity> casterOref, OuterRef<IEntity> targetOref, IEntitiesRepository repo, bool CheckTargetOnly, ReplicationLevel rl = ReplicationLevel.ClientBroadcast)
        {
            Transform casterTransform = default;
            Guid spaceId = default;
            VisibilityDataSample targetData = default;


            var batch = EntityBatch.Create();
            batch.Add(casterOref, rl);
            if (CheckTargetOnly)
                batch.Add(targetOref, rl);

            using (var casterAndTarget = await repo.Get(batch))
            {
                var positioned = PositionedObjectHelper.GetPositioned(casterAndTarget, casterOref.TypeId, casterOref.Guid);
                var spacedEntity = casterAndTarget.Get<IHasWorldSpacedClientBroadcast>(casterOref, rl)?.WorldSpaced;
                var targetPos = PositionedObjectHelper.GetPositioned(casterAndTarget, targetOref.TypeId, targetOref.Guid);


                if (positioned != null)
                    casterTransform = positioned.Transform;
                else
                    Logger.IfWarn()?.Message($"ImpactInShape: Unexpected - Caster({casterOref}) is not {nameof(IPositionedObjectClientBroadcast)}.").Write();

                if (spacedEntity != null)
                    spaceId = spacedEntity.OwnWorldSpace.Guid;
                else
                    Logger.IfWarn()?.Message($"ImpactInShape: Unexpected - Caster({casterOref}) is not {nameof(IHasWorldSpacedClientBroadcast)}.").Write();

                if (targetPos != null)
                {
                    targetData.Ref = targetOref;
                    targetData.Pos = targetPos.Transform.Position;
                }
            }

            return (casterTransform, spaceId, targetData);
        }

        public static async ValueTask<Guid> GetWorldSpaceId(OuterRef<IEntity> outerRef, IEntitiesRepository repo)
        {
            using (var wrapper = await repo.Get(outerRef))
            {
                var spacedEntity = wrapper.Get<IHasWorldSpacedClientBroadcast>(outerRef, ReplicationLevel.ClientBroadcast)?.WorldSpaced;
                if (spacedEntity == null)
                {
                    Logger.IfWarn()?.Message($"ImpactInShape: Unexpected - OuterRef({outerRef}) is not {nameof(IHasWorldSpacedClientBroadcast)}.").Write();
                    return default;
                }

                return spacedEntity.OwnWorldSpace.Guid;
            }
        }

        public static async ValueTask<Transform> GetWorldObjectTransform(OuterRef<IEntity> outerRef, IEntitiesRepository repo)
        {
            using (var wrapper = await repo.Get(outerRef))
            {
                var positioned = PositionedObjectHelper.GetPositioned(wrapper, outerRef.TypeId, outerRef.Guid);
                if (positioned == null)
                {
                    Logger.IfWarn()?.Message($"ImpactInShape: Unexpected - OuterRef({outerRef}) is not {nameof(IPositionedObjectClientBroadcast)}.").Write();
                    return default;
                }

                return positioned.Transform;
            }
        }

        void CheckPredicateAndCast(SpellWordCastData cast, IEntitiesRepository repo, List<OuterRef<IEntity>> targetsBeforePredicate, ResourceRef<PredicateDef> PredicateOnTarget, List<ResourceRef<SpellDef>> AppliedSpells)
        {
            var targets = targetsBeforePredicate.Where(x=>
            x.TypeId == LegionaryEntity.StaticTypeId || 
            x.TypeId == WorldCharacter.StaticTypeId || 
            x.TypeId == InteractiveEntity.StaticTypeId || 
            x.TypeId == MineableEntity.StaticTypeId || 
            x.TypeId == Obelisk.StaticTypeId ||
            x.TypeId == Altar.StaticTypeId);
            if (PredicateOnTarget.Target == null) 
                CastToTargets(cast.Wizard, targets, AppliedSpells, repo);
            else
            {
                foreach (var target in targets)
                {
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Check predicate | Target:{target} Predicate:{PredicateOnTarget} Cast:{cast}").Write();
                        if (await PredicateOnTarget.Target.CalcAsync(target, repo))
                            await CastToTarget(cast.Wizard, target, AppliedSpells, repo);
                    }, repo);
                }
            }
        }
    }

}
