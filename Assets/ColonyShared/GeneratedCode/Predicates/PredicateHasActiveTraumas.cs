using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.Predicates
{
    public class PredicateHasActiveTraumas : IPredicateBinding<PredicateHasActiveTraumasDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateHasActiveTraumasDef def)
        {
            var selfDef = (PredicateHasActiveTraumasDef)def;

            using (var wrapper = await repository.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var entity = wrapper.Get<IHasTraumasClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull);
                if (entity != null)
                    return await entity.Traumas.HasActiveTraumas(selfDef.TraumaTypes);
            }

            return false;
        }
    }
}
