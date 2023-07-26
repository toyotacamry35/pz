using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourceSystem.Arithmetic.Templates.Calcers;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerOwner : ICalcerBinding<CalcerOwnerDef, OuterRef>
    {
        public async ValueTask<OuterRef> Calc(CalcerOwnerDef def, CalcerContext ctx)
        {
            var entityRef = await def.Entity.Target.CalcAsync(ctx);
            if (!entityRef.IsValid) throw new Exception("Entity Ref is invalid");
            if (ctx.EntityContainer.TryGet<IHasOwnerAlways>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Always, out var hasOwner))
                return hasOwner.OwnerInformation.Owner;
            using (var cnt = await ctx.Repository.Get(entityRef.TypeId, entityRef.Guid))
                return cnt.Get<IHasOwnerAlways>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Always).OwnerInformation.Owner;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerOwnerDef def) => def.Entity.GetModifiers();
    }
}