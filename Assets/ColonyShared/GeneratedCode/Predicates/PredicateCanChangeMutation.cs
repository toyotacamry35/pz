using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.Predicates
{
    public class PredicateCanChangeMutation : IPredicateBinding<PredicateCanChangeMutationDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateCanChangeMutationDef def)
        {
            var selfDef = (PredicateCanChangeMutationDef)def;
            var target = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!target.IsValid)
                target = cast.Caster;

            using (var wrapper = await repository.Get(target.TypeId, target.Guid))
            {
                var entity = wrapper.Get<IHasMutationMechanicsClientFull>(target.TypeId, target.Guid, ReplicationLevel.ClientFull);
                if (entity != null)
                {
                    return await entity.MutationMechanics.CanChangeMutation(selfDef.DeltaValue, selfDef.Faction);
                }
            }

            return true;
        }
    }
}