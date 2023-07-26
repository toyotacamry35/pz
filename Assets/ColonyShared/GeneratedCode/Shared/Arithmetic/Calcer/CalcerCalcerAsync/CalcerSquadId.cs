using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.EntitySystem;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerSquadId : ICalcerBinding<CalcerSquadIdDef, Guid>
    {
        public async ValueTask<Guid> Calc(CalcerSquadIdDef def, CalcerContext ctx)
        {
            var entity = await def.Entity.Target.CalcAsync(ctx);
            if(ctx.EntityContainer.TryGet<IHasSquadIdClientFull>(entity.TypeId, entity.Guid, ReplicationLevel.ClientFull, out var hasSquadId))
                return hasSquadId.SquadId;
            return Guid.Empty;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerSquadIdDef def) => def.Entity.Target.CollectStatNotifiers();
    }
}