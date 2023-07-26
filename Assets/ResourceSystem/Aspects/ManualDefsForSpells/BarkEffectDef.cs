using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using UnityEngine;

namespace Shared.ManualDefsForSpells
{
    public class BarkEffectDef : SpellEffectDef
    {
        public UnityRef<Texture2D> TextureToShow { get; set; }
        public string Bark { get; set; }
        public ResourceRef<SpellCasterDef> Caster { get; set; }
    }
}