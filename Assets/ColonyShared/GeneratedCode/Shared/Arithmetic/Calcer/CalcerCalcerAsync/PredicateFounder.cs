using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Src.Arithmetic
{
    public class PredicateFounder : ICalcerBinding<PredicateFounderDef, bool>
    {
        public ValueTask<bool> Calc(PredicateFounderDef def, CalcerContext ctx)
        {
            return new ValueTask<bool>(false);
            var character = ctx.TryGetEntity<IWorldCharacterClientFull>(ReplicationLevel.ClientFull);
            if (character == null)
                return new ValueTask<bool>(false);
            return new ValueTask<bool>(character.FounderPack.Packs?.ContainsKey(def.Pack) ?? false);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateFounderDef def) => Array.Empty<StatResource>();
    }
}
