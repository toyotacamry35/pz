using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.Predicates
{
    public class PredicateBakenCanBeActivated : IPredicateBinding<PredicateBakenCanBeActivatedDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateBakenCanBeActivatedDef def)
        {
            var selfDef = (PredicateBakenCanBeActivatedDef)def;

            var target = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!target.IsValid)
                return false;

            var source = await selfDef.Source.Target.GetOuterRef(cast, repository);
            if (!source.IsValid)
                source = cast.Caster;
            if (selfDef.CommonBaken)
            {
                using (var wrapper = await repository.Get<IWorldCharacterClientFull>(source.Guid))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(source.Guid);
                    var targetE = repository.TryGetLockfree<IScenicEntityClientBroadcast>(target, ReplicationLevel.ClientBroadcast);
                    if(targetE == null)
                        return false;
                    else
                        return !worldCharacter.ActivatedCommonBakens.ContainsKey(targetE.StaticIdFromExport);
                }
            }
            else
            {
                using (var wrapper = await repository.Get<IBakenCharacterEntityClientFull>(source.Guid))
                {
                    var coordinatorServer = wrapper?.Get<IBakenCharacterEntityClientFull>(source.Guid);
                    if (coordinatorServer == null)
                        return false;

                    return await coordinatorServer.BakenCanBeActivated(target);
                }
            }
        }
    }
}
