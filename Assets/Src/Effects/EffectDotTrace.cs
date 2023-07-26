using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using Src.Debugging;

namespace Src.Effects
{
    [UsedImplicitly]
    public class EffectDotTrace  : IEffectBinding<EffectDotTraceDef>
    {
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectDotTraceDef def)
        {
            if (def.StartAt == EffectDotTraceDef.When.Attach)
                Start();
            if (def.StopAt == EffectDotTraceDef.When.Attach)
                Stop();
            return new ValueTask();
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectDotTraceDef def)
        {
            if (def.StartAt == EffectDotTraceDef.When.Detach)
                Start();
            if (def.StopAt == EffectDotTraceDef.When.Detach)
                Stop();
            return new ValueTask();
        }

        private void Start()
        {
            DotTraceController.Instance?.StartProfiling();
        }

        private void Stop()
        {
            DotTraceController.Instance?.StopProfiling();
        }

    }
}