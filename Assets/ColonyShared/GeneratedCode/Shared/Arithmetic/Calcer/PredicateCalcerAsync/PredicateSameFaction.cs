using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Shared.Arithmetic.Calcer.PredicateCalcerAsync
{
    public class PredicateSameFaction : ICalcerBinding<PredicateSameFactionDef, bool>
    {
        [UsedImplicitly]
        public async ValueTask<bool> Calc(PredicateSameFactionDef def, CalcerContext ctx)
        {
            var hasFaction = ctx.EntityContainer.Get<IHasFactionClientFull>(ctx.EntityRef, ReplicationLevel.ClientFull);
            if (hasFaction == null)
                return false;
            var target = await def.Entity.Target.CalcAsync(ctx);
            if( !ctx.EntityContainer.TryGet<IHasFactionClientFull>(target.TypeId, target.Guid, ReplicationLevel.ClientFull, out var hasFactionOther))
                return false;
            return hasFaction.Faction == hasFactionOther.Faction;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateSameFactionDef def) => Enumerable.Empty<StatResource>();
    }
}