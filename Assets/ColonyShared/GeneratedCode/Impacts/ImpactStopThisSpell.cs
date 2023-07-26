using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace ColonyShared.GeneratedCode.Impacts
{
    [UsedImplicitly]
    public class ImpactStopThisSpell : IImpactBinding<ImpactStopThisSpellDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactStopThisSpellDef def)
        {
            using (var targetEntityWrapper = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                if (targetEntityWrapper.TryGet<IHasWizardEntityServer>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Server, out var hasWizard))
                    using (var wizardEntityWrapper = await repo.Get(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id))
                    {
                        var wizardEntity = wizardEntityWrapper.Get<IWizardEntityServer>(hasWizard.Wizard.Id);
                        await wizardEntity.StopCastSpell(cast.SpellId, def.Reason == FinishReasonType.Success ? SpellFinishReason.SucessOnDemand : SpellFinishReason.FailOnDemand);
                    }
            }
        }
    }
}