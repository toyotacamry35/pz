using JetBrains.Annotations;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class EffectDotTraceDef : SpellEffectDef
    {
        public When StartAt { get; [UsedImplicitly] set; } = When.Attach;
        public When StopAt { get; [UsedImplicitly] set; } = When.Detach;
        
        public enum When { None, Attach, Detach }
    }
}