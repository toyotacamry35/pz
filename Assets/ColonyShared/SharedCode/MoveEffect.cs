using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using SharedCode.MovementSync;
using ColonyShared.ManualDefsForSpells;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;

namespace Assets.ColonyShared.SharedCode
{
    public class MoveEffect : IEffectBinding<MoveEffectDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, MoveEffectDef def)
        {
            if (cast.IsSlave)
                return;

            using (var wizardC = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                if (wizardC.TryGet(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull, out IHasMobMovementSyncClientFull mobWizard))
                {
                    var data = MovementData.StartData(cast.SpellId, def, (SpellCast) cast.CastData);
                    Logger.IfDebug()?.Message($"Attach SetMovementData with Data:{data}").Write();
                    await mobWizard.MovementSync.SetMovementData(data);
                }
                else Logger.IfWarn()?.Message(cast.Caster.Guid, "Attach | Entity is not IHasMobMovementSyncClientFull").Write();
            }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, MoveEffectDef def)
        {
            if (cast.IsSlave)
                return;

            using (var wizardC = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                if (wizardC.TryGet(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull, out IHasMobMovementSyncClientFull mobWizard))
                {
                    var data = MovementData.FinishData(cast.SpellId, def, (SpellCast) cast.CastData);
                    Logger.IfDebug()?.Message($"Detach SetMovementData with Data:{data}").Write();
                    await mobWizard.MovementSync.SetMovementData(data);
                }
                else if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message(cast.Caster.Guid, "Detach | Entity is not IHasMobMovementSyncClientFull").Write();
            }
        }
    }
}

