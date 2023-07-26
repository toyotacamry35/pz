using ColonyShared.ManualDefsForSpells;
using GeneratedCode.Custom.Containers;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace Src.Impacts
{
    public class ImpactDropItem : IImpactBinding<ImpactDropItemDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactDropItemDef def)
        {
            var targetRef = cast.Caster;

            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            var fromAddress = await ContainerUtils.GetPropertyAddress(targetRef, def.Container, repo);
            if (fromAddress?.IsValid() ?? false)
            {
                using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    var entity = wrapper?.Get<IHasContainerApiClientFull>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientFull);
                    if (entity == null)
                        return;

                    await entity.ContainerApi.Drop(fromAddress, def.Slot, def.Count);
                }
            }
        }
    }
}