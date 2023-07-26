
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedCode.Wizardry
{
    public static class SpellCastHelpers
    {
        public static async ValueTask<SpellId[]> CastSpells(IEntity entity, IReadOnlyList<ResourceRef<SpellDef>> spells)
        {
            var hasWizard = entity as IHasWizardEntity;
            if (hasWizard == null)
                return Array.Empty<SpellId>();
            if(spells == null || spells.Count == 0)
                return Array.Empty<SpellId>();

            var repo = entity.EntitiesRepository;
            var outRef = hasWizard.Wizard.OuterRef;

            using (var wrapper = await repo.Get(outRef))
            {
                var wizard = wrapper?.Get<IWizardEntityServer>(outRef, ReplicationLevel.Server);

                if (wizard == null)
                    return Array.Empty<SpellId>();

                var ids = new SpellId[spells.Count];
                for (int i = 0; i < spells.Count; ++i)
                    ids[i] = await wizard.CastSpell(new SpellCastWithTarget(null) { Def = spells[i], Target = outRef });
                return ids;
            }
        }

        public static async ValueTask<SpellId> CastSpell(IEntity entity, SpellDef spell)
        {
            if (spell == null)
                return SpellId.Invalid;

            var hasWizard = entity as IHasWizardEntity;
            if (hasWizard == null)
                return SpellId.Invalid;

            var repo = entity.EntitiesRepository;
            var outRef = hasWizard.Wizard.OuterRef;

            using (var wrapper = await repo.Get(outRef))
            {
                var wizard = wrapper?.Get<IWizardEntityServer>(outRef, ReplicationLevel.Server);

                if (wizard == null)
                    return SpellId.Invalid;

                return await wizard.CastSpell(new SpellCastWithTarget(null) { Def = spell, Target = outRef });
            }
        }

        public static async ValueTask<bool> StopSpell(IEntity entity, SpellId spell)
        {
            var hasWizard = entity as IHasWizardEntity;
            if (hasWizard == null)
                return true;

            var repo = entity.EntitiesRepository;
            var outRef = hasWizard.Wizard.OuterRef;
            using (var wrapper = await repo.Get(outRef))
            {
                var wizard = wrapper?.Get<IWizardEntityServer>(outRef, ReplicationLevel.Server);
                if (wizard == null)
                    return true;

                return await wizard.StopCastSpell(spell, SpellFinishReason.FailOnDemand);
            }
        }

        public static async ValueTask<bool[]> StopSpells(IEntity entity, IReadOnlyList<SpellId> spells)
        {
            var hasWizard = entity as IHasWizardEntity;
            if (hasWizard == null)
                return Array.Empty<bool>();

            var repo = entity.EntitiesRepository;
            var outRef = hasWizard.Wizard.OuterRef;
            using (var wrapper = await repo.Get(outRef))
            {
                var wizard = wrapper?.Get<IWizardEntityServer>(outRef, ReplicationLevel.Server);
                if (wizard == null)
                    return Array.Empty<bool>();

                var results = new bool[spells.Count];
                for (int i = 0; i < spells.Count; ++i)
                    results[i] = await wizard.StopCastSpell(spells[i], SpellFinishReason.FailOnDemand);

                return results;
            }
        }

    }
}
