using Assets.Src.Character.Events;
using Assets.Src.ManualDefsForSpells;
using Assets.Src.Shared.Impl;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;

namespace Assets.Src.Effects.FX
{
    [UsedImplicitly, PredictableEffect]
    class EffectPostVisualEventOnTarget : IClientOnlyEffectBinding<EffectPostVisualEventOnTargetDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPostVisualEventOnTargetDef def)
        {
            var castergo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(cast.Caster);
            OuterRef<IEntity> targetEntityRef = cast.CastData.GetTarget();
            var targetgo = targetEntityRef.IsValid ? GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(targetEntityRef) : null;
            var @params = await EffectPostVisualEvent.MapParameters(cast, def.Params, repo);
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var vep = targetgo?.GetComponent<VisualEventProxy>();
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Def:{def} CasterGO:{castergo} TargetRef:{targetEntityRef} TargetGO:{targetgo} VEP:{vep}").Write();
                if (vep)
                    vep.PostEvent(EffectPostVisualEvent.CreateEvent(def.TriggerName, castergo, targetgo, targetEntityRef, targetgo?.transform, repo, @params));
            });
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPostVisualEventOnTargetDef indef)
        {
            return new ValueTask();
        }
    }
}