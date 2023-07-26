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
    public class EffectInputLayer : IEffectBinding<EffectInputLayerDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(EffectInputLayer));

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputLayerDef def)
        {
            if (!cast.OnClientWithAuthority() && !cast.OnServerMaster())
                return;
 
            using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var hasInputHandlers = cnt.Get<IHasInputActionHandlersClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull);
                if (hasInputHandlers != null)
                    hasInputHandlers.InputActionHandlers.LayersStack.PushLayer(cast.SpellId, def.Layer, def.Handlers);
                else
                    Logger.IfWarn()?.Message($"Using with incompatible caster | Def:{def} CasterId:{cast.Caster.Guid} CasterType:{cast.Caster.TypeId}").Write();
            }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectInputLayerDef def)
        {
            if (!cast.OnClientWithAuthority() && !cast.OnServerMaster())
                return;
 
            using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                cnt.TryGet<IHasInputActionHandlersClientFull>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.ClientFull, out var hasInputHandlers);
                if (hasInputHandlers != null)
                    hasInputHandlers.InputActionHandlers.LayersStack.DeleteLayer(cast.SpellId, def.Layer);
                //else
                //    Logger.IfWarn()?.Message($"Using with incompatible caster | Def:{def} CasterId:{cast.Caster.Guid} CasterType:{cast.Caster.TypeId}").Write();
            }
        }
    }
}