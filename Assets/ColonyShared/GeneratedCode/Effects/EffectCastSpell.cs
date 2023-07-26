using Assets.Src.Aspects;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;

namespace Assets.Src.Effects
{
    public class EffectCastSpell : IEffectBinding<EffectCastSpellDef>
    {

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectCastSpellDef def)
        {
            if (cast.IsSlave)
                return;
            using (var wrapper = await repo.Get(cast.Wizard.TypeId, cast.Wizard.Guid))
            {
                var causer = cast.WordCastId(def);
                var wizardEntity = wrapper.Get<IWizardEntityServer>(cast.Wizard.Guid);
                var spellCast = new SpellCastWithParameters {
                    Def = def.Spell,
                    StartAt = SyncTime.Now, 
                    Parameters = new SpellCastParameter[]{new SpellCastParameterCauser { Causer = causer }}};
                await wizardEntity.CastSpell(spellCast);
            }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectCastSpellDef def)
        {
            if (cast.IsSlave)
                return;
            using (var wrapper = await repo.Get(cast.Wizard.TypeId, cast.Wizard.Guid))
            {
                var causer = cast.WordCastId(def);
                var wizardEntity = wrapper.Get<IWizardEntityServer>(cast.Wizard.Guid);
                await wizardEntity.StopSpellByCauser(causer, SpellFinishReason.SucessOnDemand);
            }
        }
    }
}
