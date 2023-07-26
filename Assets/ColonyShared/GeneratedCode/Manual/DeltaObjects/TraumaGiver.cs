using System;
using ColonyShared.SharedCode.Utils;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Wizardry;
using Assets.Src.Aspects.Impl.Traumas.Template;
using Core.Environment.Logging.Extension;
using Scripting;
using Src.Aspects.Impl.Stats;

namespace GeneratedCode.DeltaObjects
{
    public partial class TraumaGiver
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public async Task<bool> StartTraumaImpl(IEntity entity)
        {
                switch (Def)
                {
                    case TraumaGiverDef def:
                        if (entity is IHasWizardEntity wizardEntityContainer)
                        {
                            using (var wrapper = await entity.EntitiesRepository.Get(wizardEntityContainer.Wizard.TypeId, wizardEntityContainer.Wizard.Id))
                            {
                                var wizardEntity = wrapper.Get<IWizardEntity>(wizardEntityContainer.Wizard.Id);
                                SpellId newSpellId = await wizardEntity.CastSpell(new SpellCast { Def = def.DebuffSpellRef, StartAt = SyncTime.Now });
                                if (!newSpellId.IsValid)
                                    return false;
                                if (this.HasActiveSpell())
                                    await wizardEntity.StopCastSpell(new SpellId(SpellId), SpellFinishReason.SucessOnDemand);
                                SpellId = newSpellId.Counter;
                                CurrentTraumaPoints = Def.TraumaPoints;
                            }
                        }
                        else
                            return false;
                        break;

                    case SaveableTraumaDef def:
                        if (entity is IHasBuffs hasBuffs)
                        {
                            if (def.SpellOnStart != null && entity is IHasWizardEntity hasWizard)
                                using (var wrapper = await entity.EntitiesRepository.Get(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id))
                                {
                                    var wizardEntity = wrapper.Get<IWizardEntity>(hasWizard.Wizard.Id);
                                    var id = await wizardEntity.CastSpell(new SpellCast {Def = def.SpellOnStart, StartAt = SyncTime.Now});
                                    if (!id.IsValid) // если спелл не запустился, то травма не активируется
                                        return false;
                                }
                            var ctx = new ScriptingContext {Host = new OuterRef<IEntity>(ParentEntityId, ParentTypeId)};
                            var newSpellId = await hasBuffs.Buffs.TryAddBuff(ctx, def.Buff);
                            if (!newSpellId.IsValid)
                                return false;
                            if (this.HasActiveSpell())
                                await hasBuffs.Buffs.RemoveBuff(new SpellId(SpellId));
                            SpellId = newSpellId.Counter;
                            CurrentTraumaPoints = Def.TraumaPoints;
                        }
                        else
                            return false;
                        break;
                    
                    default: 
                        throw new NotImplementedException($"{Def}");
                }
            Logger.IfDebug()?.Message($"Trauma started {this}").Write();
            return true;
        }

        public async Task StopTraumaImpl(IEntity entity)
        {
            Logger.IfDebug()?.Message($"Stop trauma {Def}").Write();
            if (this.HasActiveSpell())
                switch (Def)
                {
                    case TraumaGiverDef _:
                        if (entity is IHasWizardEntity wizardEntityContainer)
                            using (var wrapper = await entity.EntitiesRepository.Get(wizardEntityContainer.Wizard.TypeId, wizardEntityContainer.Wizard.Id))
                            {
                                var wizardEntity = wrapper.Get<IWizardEntity>(wizardEntityContainer.Wizard.Id);
                                if (wizardEntity != null)
                                    await wizardEntity.StopCastSpell(new SpellId((ulong) SpellId), SpellFinishReason.FailOnDemand);
                                SpellId = SharedCode.Wizardry.SpellId.Invalid.Counter;
                            }
                        break;
                    
                    case SaveableTraumaDef _:
                        if (entity is IHasBuffs hasBuffs)
                            await hasBuffs.Buffs.RemoveBuff(new SpellId(SpellId));
                        SpellId = SharedCode.Wizardry.SpellId.Invalid.Counter;
                        break;
                        
                    default: 
                        throw new NotImplementedException($"{Def}");
                }
        }

        public override string ToString()
        {
            switch (Def)
            {
                case TraumaGiverDef def:
                    return Def.____GetDebugShortName() + ", [" + def.DebuffSpellRef.Target.____GetDebugShortName() + ": " + SpellId + "], " + CurrentTraumaPoints;
                case SaveableTraumaDef def:
                    return Def.____GetDebugShortName() + ", [" + def.Buff.Target.____GetDebugShortName() + ": " + SpellId + "], " + CurrentTraumaPoints;
                default: 
                    throw new NotImplementedException($"{Def}");
            }
        }
    }
}
