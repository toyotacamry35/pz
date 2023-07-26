using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace Src.SpellSystem
{
    [UsedImplicitly]
    public class EffectBark : IEffectBinding<BarkEffectDef>
    {
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, BarkEffectDef def) => new ValueTask();
        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, BarkEffectDef def) => new ValueTask();
    }
}