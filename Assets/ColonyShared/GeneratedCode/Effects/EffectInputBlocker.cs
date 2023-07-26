using System;
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

    public class EffectInputBlocker : IEffectBinding<EffectInputBlockerDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputBlockerDef def)
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
                        var exceptions = new InputActionDef[(def.Except?.Length ?? 0) + (def.ExceptList.Target?.AllActions?.Length ?? 0)];
                        {
                            int i = 0;
                            if (def.Except != null)
                                foreach (var a in def.Except)
                                    exceptions[i++] = a.Target;
                            if (def.ExceptList.Target?.AllActions != null)
                                foreach (var a in def.ExceptList.Target?.AllActions)
                                    exceptions[i++] = a;
                        }
                        var causer = cast.WordCastId(def);
                        if (def.Block != null)
                            foreach (var action in def.Block)
                                if (Array.IndexOf(exceptions, action.Target) == -1)
                                    layer.AddBinding(causer, action.Target, new InputActionBlockerDescriptor(), new InputActionHandlerContext( currentSpell: cast.SpellId ));
                        if (def.BlockList.Target?.AllActions != null)
                            foreach (var action in def.BlockList.Target.AllActions)
                                if (Array.IndexOf(exceptions, action) == -1)
                                    layer.AddBinding(causer, action, new InputActionBlockerDescriptor(), new InputActionHandlerContext( currentSpell: cast.SpellId ));
                    });
                }
                else
                    Logger.IfWarn()?.Message($"Using with incompatible caster | Def:{def} CasterId:{cast.Caster.Guid} CasterType:{cast.Caster.TypeId}").Write();
            }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputBlockerDef def)
        {
            if (!cast.OnClientWithAuthority() && !cast.OnServerMaster())
                return;

            using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                cnt.TryGet<IHasInputActionHandlersClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull, out var hasInputHandlers);
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