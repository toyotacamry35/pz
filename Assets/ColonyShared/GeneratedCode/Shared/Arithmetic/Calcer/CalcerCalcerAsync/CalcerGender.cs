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
    public class CalcerGender : ICalcerBinding<CalcerGenderDef, BaseResource>
    {
        public async ValueTask<BaseResource> Calc(CalcerGenderDef def, CalcerContext ctx)
        {
            var entityRef = await def.Entity.Target.CalcAsync(ctx);
            if (!entityRef.IsValid) throw new Exception("Entity Ref is invalid");
            IHasGenderClientBroadcast hasGender;
            if(ctx.EntityContainer.TryGet(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast, out hasGender))
                return hasGender.Gender;
            using (var cnt = await ctx.Repository.Get(entityRef.TypeId, entityRef.Guid))
                return cnt.TryGet(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast, out hasGender) ? hasGender?.Gender : null;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerGenderDef def) => def.Entity.Target.CollectStatNotifiers();
    }
}