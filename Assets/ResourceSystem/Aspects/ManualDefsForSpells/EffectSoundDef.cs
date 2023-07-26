using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Wizardry;

namespace Src.ManualDefsForSpells
{
    public class EffectSoundDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> Target { get; [UsedImplicitly] set; }
        public string Event { get; [UsedImplicitly] set; }
        public Dictionary<string, ResourceRef<CalcerDef>> Params { get; [UsedImplicitly] set; }
        public bool Detach { get; [UsedImplicitly] set; } = true;
    }
}