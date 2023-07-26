using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Scripting;

namespace Assets.Src.Impacts
{
    public class ImpactCastSpell : IImpactBinding<ImpactCastSpellDef>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactCastSpellDef indef)
        {
            var def = (ImpactCastSpellDef)indef;
            //TODO: CHECK BUGS LATER
            OuterRef<IEntity> casterRef = cast.Caster;
            OuterRef<IEntity> targetRef = default;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            else
                targetRef = casterRef;

            if (def.Caster.Target != null)
                casterRef = await def.Caster.Target.GetOuterRef(cast, repo);

            if (!casterRef.IsValid)
                casterRef = cast.Caster;

            using (var entcont = await repo.Get(casterRef.TypeId, casterRef.Guid))
            {
                var hasWizard = entcont.Get<IHasWizardEntityServer>(casterRef.TypeId, casterRef.Guid, ReplicationLevel.Server);
                if (!hasWizard.AssertIfNull(nameof(hasWizard)))
                {
                    using (var entcont2 = await repo.Get(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id))
                    {
                        var wizard = entcont2.Get<IWizardEntityServer>(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id, ReplicationLevel.Server);
                        if (!wizard.AssertIfNull(nameof(wizard)))
                        {
                            var spellBuilder = new SpellCastBuilder()
                                .SetDirectionIfNotNull(def.Direction.Target?.GetVector(cast))
                                .SetTargetIfValid(targetRef);

                            var ctx = await indef.Context.Target.CalcFromDef(casterRef, cast.CastData.Context, repo);
                            if (def.Spell.Target != null)
                                await wizard.CastSpell(spellBuilder.SetSpell(def.Spell).AddContext(ctx).Build());
                            if(def.Spells != null)
                                foreach (var spell in def.Spells)
                                    await wizard.CastSpell(spellBuilder.SetSpell(spell).AddContext(ctx).Build());

                            var calcer = def.ProcSpell.Target;
                            if (calcer != null)
                            {
                                var returnedRes = await calcer.CalcAsync(new CalcerContext(entcont, casterRef, repo, cast));
                                if (returnedRes is SpellDef procSpell)
                                    await wizard.CastSpell(spellBuilder.SetSpell(procSpell).AddContext(ctx).Build());
                                else if(returnedRes != null)
                                    Logger.IfError()?.Message($"Tried to use the calcer to replace the spell, but it doesn't return Spell Def, [{returnedRes}]").Write();
                            }
                        }
                    }
                }
            }
            }
    }
}
