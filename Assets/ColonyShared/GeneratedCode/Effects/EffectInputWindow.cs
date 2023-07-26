using System.Threading.Tasks;
using ColonyShared.SharedCode.Aspects.ManualDefsForSpells;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Utils.DebugCollector;
using SharedCode.Wizardry;

namespace Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectInputWindow : IEffectBinding<EffectInputWindowDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(EffectInputLayer));

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputWindowDef def)
        {
            if (!cast.OnClientWithAuthority() && !cast.OnServerMaster())
                return;
 
            if (def.Handlers == null)
                return;    
                
            using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var hasInputHandlers = cnt.Get<IHasInputActionHandlersClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull);
                if (hasInputHandlers != null)
                {
                    var delay = SyncTime.FromSeconds(def.Delay);
                    var activationTime = cast.WordTimeRange.Start + (!def.DelayIsBeforeEnd ? delay : cast.CastData.Duration - delay);
                    hasInputHandlers.InputActionHandlers.LayersStack.ModifyLayer(cast.SpellId, def.Layer, layer =>
                    {
                        var causer = cast.WordCastId(def);
                        if (cast.OnClient())
                            Collect.IfActive?.EventBgn($"EffectInputWindow.{def.____GetDebugAddress()}", cast.Caster, causer);
                        foreach (var tuple in def.Handlers)
                        {
                            var ctx = new InputActionHandlerContext(currentSpell: def.BreakCurrentSpell ? cast.SpellId : SpellId.Invalid);
                            layer.AddBinding(causer, tuple.Key, tuple.Value.Target, ctx);
                            layer.AddBinding(causer, tuple.Key, new InputActionHandlerInputWindowDescriptor(activationTime), ctx);
                        }
                    });
                }
                else
                    Logger.IfWarn()?.Message($"Using with incompatible caster | Def:{def} CasterId:{cast.Caster.Guid} CasterType:{cast.Caster.TypeId}").Write();
            }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputWindowDef def)
        {
            if (!cast.OnClientWithAuthority() && !cast.OnServerMaster())
                return;

            if (def.Handlers == null)
                return;
            
            using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                cnt.TryGet<IHasInputActionHandlersClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull, out var hasInputHandlers);
                if (hasInputHandlers != null)
                {
                    var causer = cast.WordCastId(def);
                    if (cast.OnClient())
                        Collect.IfActive?.EventEnd(causer);
                    hasInputHandlers.InputActionHandlers?.LayersStack.ModifyLayer(cast.SpellId, def.Layer, layer => layer.RemoveBindings(causer));
                }
                else
                    Logger.IfWarn()?.Message($"Using with incompatible caster | Def:{def} CasterId:{cast.Caster.Guid} CasterType:{cast.Caster.TypeId}").Write();
            }
        }
    }
}