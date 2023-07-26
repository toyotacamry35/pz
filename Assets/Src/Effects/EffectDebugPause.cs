using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectDebugPause : IClientOnlyEffectBinding<EffectDebugPauseDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugPauseDef def)
        {
            if (!def.OnFinish && cast.OnClientWithAuthority())
                UnityQueueHelper.RunInUnityThreadNoWait(UnityEngine.Debug.Break);
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugPauseDef def)
        {
            if (def.OnFinish && cast.OnClientWithAuthority())
                UnityQueueHelper.RunInUnityThreadNoWait(UnityEngine.Debug.Break);
            return new ValueTask();
        }
    }
}