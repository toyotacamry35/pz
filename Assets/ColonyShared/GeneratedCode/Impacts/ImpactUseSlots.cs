﻿using ResourcesSystem.Loader;
using ColonyHelpers;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace Src.Impacts
{
    public class ImpactUseSlots : IImpactBinding<ImpactUseSlotsDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactUseSlotsDef def)
        {
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast,repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var entity = wrapper?.Get<IHasDollServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                foreach (var slot in def.Slots.Shuffle())
                {
                    var slotResourceIDFull = GameResourcesHolder.Instance.GetID(slot.Target);
                    if (await entity.Doll.AddUsedSlot(slotResourceIDFull))
                        return;
                }
            }
        }
    }

    public class PredicateCanUseSlots : IPredicateBinding<PredicateCanUseSlotsDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateCanUseSlotsDef def)
        {
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var entity = wrapper?.Get<IHasDollServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                foreach (var slot in def.Slots.Shuffle())
                {
                    var slotResourceIDFull = GameResourcesHolder.Instance.GetID(slot.Target);
                    if (await entity.Doll.CanAddUsedSlot(slotResourceIDFull))
                        return true;
                }
            }
            return false;
        }
    }
}