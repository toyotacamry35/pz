using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateIsAFK : ICalcerBinding<PredicateIsAFKDef, bool>
    {
        public ValueTask<bool> Calc(PredicateIsAFKDef def, CalcerContext ctx)
        {
            var character = ctx.TryGetEntity<IWorldCharacterClientBroadcast>(ReplicationLevel.ClientBroadcast);
            return new ValueTask<bool>(character != null && character.IsAFK);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateIsAFKDef def) { yield return null; }
    }
}
