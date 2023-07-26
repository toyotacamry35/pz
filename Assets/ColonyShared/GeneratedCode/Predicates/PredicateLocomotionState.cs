using System.Threading.Tasks;
using ColonyShared.SharedCode.GeneratedDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;

namespace Assets.Src.Predicates
{
    public class PredicateLocomotionState : IPredicateBinding<PredicateLocomotionStateDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateLocomotionStateDef indef)
        {
            if (cast.OnClient())
            {
                var def = (PredicateLocomotionStateDef) indef;
                using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
                {
                    var hasLocomotion = cnt.Get<IHasLocomotionOwnerClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull);
                    if (hasLocomotion?.LocomotionOwner?.Locomotion != null)
                        return (hasLocomotion.LocomotionOwner.Locomotion.Flags & def.State) != 0;
                }
            }
            return !indef.Inversed; // предикат всегда должен быть истинным если нет locomotion'а или не на клиенте
        }
    }
}