using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Src.Impacts
{
    public class ImpactStopSpell : IImpactBinding<ImpactStopSpellDef>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactStopSpellDef def)
        {
            var defSelf = (ImpactStopSpellDef) def;
            var targetRef = cast.Caster;
            if (defSelf.Target.Target != null)
                targetRef = await defSelf.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            List<SpellDef> spells = null;
            if(defSelf.Spell.Target is SpellDef spell1)
                (spells = new List<SpellDef>()).Add(spell1);
            if(defSelf.Spells != null)
                foreach (var entry in defSelf.Spells)
                    if(entry.Target is SpellDef spell2)
                        (spells ?? (spells = new List<SpellDef>())).Add(spell2);

            List<BuffDef> buffs = null;
            if (defSelf.Spell.Target is BuffDef buff1)
                (buffs = new List<BuffDef>()).Add(buff1);
            if(defSelf.Spells != null)
                foreach (var entry in defSelf.Spells)
                    if (entry.Target is BuffDef buff2)
                        (buffs ?? (buffs = new List<BuffDef>())).Add(buff2);

            if (spells != null || buffs != null)
            {
                using (var targetEntityWrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    if (spells != null)
                    {
                        var targetEntity = targetEntityWrapper.Get<IHasWizardEntityServer>(
                            targetRef.TypeId,
                            targetRef.Guid,
                            ReplicationLevel.Server);
                        if (targetEntity != null)
                            using (var wizardEntityWrapper = await repo.Get(targetEntity.Wizard.TypeId, targetEntity.Wizard.Id))
                            {
                                var wizardEntity = wizardEntityWrapper.Get<IWizardEntityServer>(targetEntity.Wizard.Id);
                                foreach (var spell in spells)
                                    await wizardEntity.StopSpellByDef(
                                        spell,
                                        cast.SpellId,
                                        defSelf.Reason == FinishReasonType.Success
                                            ? SpellFinishReason.SucessOnDemand
                                            : SpellFinishReason.FailOnDemand);
                            }
                    }

                    if (buffs != null)
                    {
                        var hasBuffs = targetEntityWrapper.Get<IHasBuffsServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                        if (hasBuffs != null)
                        {
                            foreach (var buff in buffs)
                                await hasBuffs.Buffs.RemoveBuff(buff);
                        }
                    }
                }
            }
        }
    }
}
