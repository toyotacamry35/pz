using System.Threading.Tasks;
using SharedCode.Wizardry;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Utils.Extensions;

namespace Assets.Src.Impacts
{
    public class ImpactStopAllSpellsOfGroups : IImpactBinding<ImpactStopAllSpellsOfGroupsDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactStopAllSpellsOfGroupsDef def)
        {
            var defSelf = (ImpactStopAllSpellsOfGroupsDef) def;
            var targetRef = cast.Caster;
            if (defSelf.Target.Target != null)
                targetRef = await defSelf.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            targetRef = targetRef.RefWithRepTypeId(ReplicationLevel.Server);

            using (var targetEntityWrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var targetEntity = targetEntityWrapper.Get<IHasWizardEntityServer>(targetRef.TypeId, targetRef.Guid);
                if (targetEntity != null)
                    using (var wizardEntityWrapper = await repo.Get(targetEntity.Wizard.TypeId, targetEntity.Wizard.Id))
                    {
                        var wizardEntity = wizardEntityWrapper.Get<IWizardEntityServer>(targetEntity.Wizard.Id);
                        foreach (var grp in defSelf.Groups)
                            await wizardEntity.StopAllSpellsOfGroup(grp, cast.SpellId, defSelf.Reason == FinishReasonType.Success ? SpellFinishReason.SucessOnDemand : SpellFinishReason.FailOnDemand);
                    }
            }
            }
    }
}
