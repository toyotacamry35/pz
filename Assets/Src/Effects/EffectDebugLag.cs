using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectDebugLag : IEffectBinding<EffectDebugLagDef>
    {
        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugLagDef def)
        {
            if (!def.OnFinish && (def.Where & Where(cast)) != 0)
                await Task.Delay(def.Delay);
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugLagDef def)
        {
            if (def.OnFinish && (def.Where & Where(cast)) != 0)
                await Task.Delay(def.Delay);
        }

        private EffectDebugLagDef.Side Where(SpellWordCastData cast)
        {
            EffectDebugLagDef.Side where = 0;
            if (cast.OnClient())
                where |= EffectDebugLagDef.Side.Client;
            if (cast.OnClientWithAuthority())
                where |= EffectDebugLagDef.Side.ClientWithAuthority;
            if (cast.OnServerSlave())
                where |= EffectDebugLagDef.Side.ServerSlave;
            if (cast.OnServerMaster())
                where |= EffectDebugLagDef.Side.ServerMaster;
            return where;
        }
    }
}