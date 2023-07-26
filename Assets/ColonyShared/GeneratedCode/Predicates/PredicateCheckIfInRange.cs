using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.Wizardry;
using Uins;

namespace Assets.Src.Predicates
{
    [UsedImplicitly]
    public class PredicateCheckIfInRange : IPredicateBinding<PredicateCheckIfInRangeDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateCheckIfInRangeDef indef)
        {
            var inRange = false;
            var def = (PredicateCheckIfInRangeDef) indef;
            var casterRef = await def.Caster.Target.GetOuterRef(cast, repo);
            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            using (var container = await repo.Get(EntityBatch.Create()
                .Add(casterRef, ReplicationLevel.ClientBroadcast)
                .Add(targetRef, ReplicationLevel.ClientBroadcast)))
            {
                if (container.AssertIfNull(nameof(container)))
                    return inRange;

                var casterPos = PositionedObjectHelper.GetPosition(container, casterRef.TypeId, casterRef.Guid);
                var targetPos = PositionedObjectHelper.GetPosition(container, targetRef.TypeId, targetRef.Guid);

                if (targetPos.AssertIfNull(nameof(targetPos)) ||
                    casterPos.AssertIfNull(nameof(casterPos)))
                    return inRange;

                inRange = Vector3.GetDistance(casterPos.Value, targetPos.Value) < def.Range;
            }

            //UI.CallerLogInfoDefault($"<{GetType()}> inRange{inRange.AsSign()}"); //2del
            return inRange;
        }
    }
}