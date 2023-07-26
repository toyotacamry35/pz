using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.Predicates
{
    public class PredicateMutationNeedSync : IPredicateBinding<PredicateMutationNeedSyncDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateMutationNeedSyncDef def)
        {
            var selfDef = (PredicateMutationNeedSyncDef)def;

            if (selfDef.Target.Target == null)
                return false;

            var target = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!target.IsValid)
                return false;

            var source = cast.Caster;
            if(selfDef.Source.Target != null)
                source = await selfDef.Source.Target.GetOuterRef(cast, repository);

            if (!source.IsValid)
                source = cast.Caster;

            using (var wrapper = await repository.Get(source.TypeId, source.Guid))
            {
                var character = wrapper.Get<IWorldCharacterClientFull>(source.Guid);
                if (!await character.IsBakenActivated(target))
                    return false;

                var entity = wrapper.Get<IHasMutationMechanicsClientFull>(source.TypeId, source.Guid, ReplicationLevel.ClientFull);
                if (entity != null)
                {
                    return (entity.MutationMechanics.Stage != entity.MutationMechanics.NewStage && entity.MutationMechanics.NewStage != null) || (entity.Faction != entity.MutationMechanics.NewFaction && entity.MutationMechanics.NewFaction != null);
                }
            }

            return false;
        }
    }
}
