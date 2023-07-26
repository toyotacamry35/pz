using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Impacts
{
    [UsedImplicitly]
    public class ImpactUnuseSlot : IImpactBinding<ImpactUnuseSlotDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactUnuseSlotDef def)
        {
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var entity = wrapper?.Get<IHasDollServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                if (entity != null)
                {
                    var slots = (def.Slots != null ? def.Slots : Enumerable.Empty<SlotDef>())
                        .Concat(def.SlotsList.Target?.Slots != null ? def.SlotsList.Target.Slots.Select(x => x.Target) : Enumerable.Empty<SlotDef>());
                    foreach (var slot in slots)
                        await entity.Doll.RemoveUsedSlot(GameResourcesHolder.Instance.GetID(slot));
                }
            }
        }
    }
}