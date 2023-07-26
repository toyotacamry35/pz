using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using UnityEngine;
using Color = SharedCode.Utils.Color;

namespace Shared.ManualDefsForSpells
{
    public class EffectShowDebugTextureDef : SpellEffectDef
    {
        public UnityRef<Texture> Texture { get; set; }
        public Color Color { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}