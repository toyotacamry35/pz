using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourceSystem.Arithmetic.Templates.Calcers;
using SharedCode.EntitySystem;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerFaction : ICalcerBinding<CalcerFactionDef, BaseResource>
    {
        public async ValueTask<BaseResource> Calc(CalcerFactionDef def, CalcerContext ctx)
        {
            var entityRef = await def.Entity.Target.CalcAsync(ctx);
            if (!entityRef.IsValid) throw new Exception("Entity Ref is invalid");
            IHasFactionClientBroadcast hasFaction;
            if(ctx.EntityContainer.TryGet(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast, out hasFaction))
                return hasFaction.Faction;
            using (var cnt = await ctx.Repository.Get(entityRef.TypeId, entityRef.Guid))
                return cnt.TryGet(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast, out hasFaction) ? hasFaction?.Faction : null;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerFactionDef def) => def.Entity.Target.CollectStatNotifiers();
    }
}