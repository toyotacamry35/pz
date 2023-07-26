using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateHasTraumas : ICalcerBinding<PredicateHasTraumasDef, bool>
    {
        public ValueTask<bool> Calc(PredicateHasTraumasDef def, CalcerContext ctx)
        {
            var entity = ctx.TryGetEntity<IEntity>(ReplicationLevel.ClientFull);
            if (entity == null) 
                return new ValueTask<bool>(false);
            var masterEntity = entity is IBaseDeltaObjectWrapper wrapper ? wrapper.GetBaseDeltaObject() : entity;
            var traumaEntity = masterEntity.GetReplicationLevel(ReplicationLevel.ClientFull) as GeneratedCode.DeltaObjects.ReplicationInterfaces.IHasTraumasClientFull;
            if (def.TraumaTypes == null || traumaEntity == null)
                return new ValueTask<bool>(false);
            return traumaEntity.Traumas.HasActiveTraumas(def.TraumaTypes);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateHasTraumasDef def)  { yield return null; }
    }
}
