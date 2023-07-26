using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Aspects.ManualDefsForSpells;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;

namespace Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectInputSpellBreaker : IEffectBinding<EffectInputSpellBreakerDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputSpellBreakerDef def)
        {
            if (!cast.OnClientWithAuthority() && !cast.OnServerMaster())
                return;

            using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var hasInputHandlers = cnt.Get<IHasInputActionHandlersClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull);
                if (hasInputHandlers != null)
                {
                    hasInputHandlers.InputActionHandlers.LayersStack.ModifyLayer(cast.SpellId, def.Layer, layer =>
                    {
                        var handlerDescriptor = new InputActionHandlerSpellBreakerDescriptor {When = def.When, FinishReason = def.FinishReason};
                        var context = new InputActionHandlerContext(currentSpell: cast.SpellId);
                        var causer = cast.WordCastId(def);
                        if (def.Actions != null)
                            foreach (var act in def.Actions)
                                layer.AddBinding(causer, act, handlerDescriptor, context);
                        if (def.List != null)
                            foreach (var act in def.List.Target.AllActions)
                                if (act is InputActionTriggerDef)
                                    layer.AddBinding(causer, act, handlerDescriptor, context);
                    });
                }
                else
                    Logger.IfWarn()?.Message($"Using with incompatible caster | Def:{def} CasterId:{cast.Caster.Guid} CasterType:{cast.Caster.TypeId}").Write();
            }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputSpellBreakerDef def)
        {
            if (!cast.OnClientWithAuthority() && !cast.OnServerMaster())
                return;

            using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var hasInputHandlers = cnt.Get<IHasInputActionHandlersClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull);
                if (hasInputHandlers != null)
                {
                    hasInputHandlers.InputActionHandlers.LayersStack.ModifyLayer(cast.SpellId, def.Layer, layer => layer.RemoveBindings(cast.WordCastId(def)));
                }
                else
                    Logger.IfWarn()?.Message($"Using with incompatible caster | Def:{def} CasterId:{cast.Caster.Guid} CasterType:{cast.Caster.TypeId}").Write();
            }
        }
    }
}