using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using ColonyShared.GeneratedCode.Manual.DeltaObjects;
using ColonyShared.SharedCode.Entities.Reactions;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourceSystem.Aspects.AccessRights;
using GeneratedDefsForSpells;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates
{
    [UsedImplicitly]
    public class PredicateCheckIfHasAccessRights : IPredicateBinding<PredicateCheckIfHasAccessRightsDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly ThreadLocal<List<ArgTuple>> _ArgsBuffer = new ThreadLocal<List<ArgTuple>>(() => new List<ArgTuple>());
        
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateCheckIfHasAccessRightsDef def)
        {
            if (def.Target.Target == null || def.Caster.Target == null)
                return false;

            OuterRef<IEntity> targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            OuterRef<IEntity> casterRef = await def.Caster.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid || !casterRef.IsValid)
                return false;

            return await CheckAccessRights(targetRef, casterRef, repo);
        }

        public static async ValueTask<bool> CheckAccessRights(OuterRef<IEntity> containerRef, OuterRef<IEntity> casterRef, IEntitiesRepository repo)
        {
            var nfo = await GetOwnerInformation(containerRef, repo);
            if (nfo.Owner.IsValid)
            {
                using (var cnt = await repo.Get(new EntityBatch().Add(casterRef, ReplicationLevel.ClientBroadcast).Add(nfo.Owner, ReplicationLevel.ClientBroadcast)))
                {
                    var relationshipContext = new Relationship.Context(thisEntity: containerRef, otherEntity: casterRef);
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(relationshipContext.ToString()).Write();
                    _ArgsBuffer.Value.Clear();
                    Relationship.MapRelationshipArgs(Constants.RelationshipConstants.ArgsMapping, relationshipContext, _ArgsBuffer.Value);
                    var calcerContext = new CalcerContext(cnt, casterRef, repo, args: _ArgsBuffer.Value.ToCalcerArgs());
                    if (nfo.LockPredicate != null && nfo.LockPredicate != AccessPredicateDef.Empty)
                    {
                        var result = await nfo.LockPredicate.Predicate.Target.CalcAsync(calcerContext);
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"LockPredicate:{nfo.LockPredicate.____GetDebugAddress()} Result:{result}").Write();
                        if (!result)
                            return false;
                    }
                    if (nfo.AccessPredicate != null && nfo.AccessPredicate != AccessPredicateDef.Empty)
                    {
                        var result = await nfo.AccessPredicate.Predicate.Target.CalcAsync(calcerContext);
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"AccessPredicate:{nfo.AccessPredicate.____GetDebugAddress()} Result:{result}").Write();
                        if (!result)
                            return false;
                    }
                }
            }
            return true;
        }

        public static async ValueTask<(OuterRef<IEntity> Owner, AccessPredicateDef AccessPredicate, AccessPredicateDef LockPredicate)> GetOwnerInformation(OuterRef<IEntity> containerRef, IEntitiesRepository repo)
        {
            if (containerRef.IsValid)
            {
                using (var wrapper = await repo.Get(containerRef))
                {
                    var hasOwnerEntity = wrapper.Get<IHasOwnerClientBroadcast>(containerRef, ReplicationLevel.ClientBroadcast);
                    if (hasOwnerEntity != null)
                        return (hasOwnerEntity.OwnerInformation.Owner, hasOwnerEntity.OwnerInformation.AccessPredicate, hasOwnerEntity.OwnerInformation.LockPredicate);
                }
            }
            return (OuterRef<IEntity>.Invalid, null, null);
        }
    }
}