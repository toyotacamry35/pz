using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;

namespace Assets.Src.Impacts
{
    public class ImpactActivateBaken : IImpactBinding<ImpactActivateBakenDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ImpactActivateBaken");

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactActivateBakenDef def)
        {
            var selfDef = (ImpactActivateBakenDef)def;

            if (selfDef.Target.Target == null)
                return;

            var target = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!target.IsValid)
                return;

            var source = cast.Caster;
            if (selfDef.Source.Target != null)
                source = await selfDef.Source.Target.GetOuterRef(cast, repository);

            if(!source.IsValid)
                source = cast.Caster;

            if (selfDef.CommonBaken)
            {
                using (var wrapper = await repository.Get<IWorldCharacter>(source.Guid))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterServer>(source.Guid);
                    var targetE = repository.TryGetLockfree<IScenicEntityServer>(target, ReplicationLevel.Server);
                    if (targetE != null) 
                        await worldCharacter.ActivateCommonBaken(targetE.StaticIdFromExport);
                }
                
            }
            else
            {
                using (var wrapper = await repository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
                {
                    var bakenCoordinator = wrapper.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                    if (await bakenCoordinator.ActivateBaken(source.Guid, target))
                        using (var wc = await repository.Get<IWorldCharacter>(source.Guid))
                        {
                            var worldCharacter = wc.Get<IWorldCharacterServer>(source.Guid);
                            await worldCharacter.ActivateCommonBaken(Guid.Empty);
                        }
                }
            }
        }
    }
}
