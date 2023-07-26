using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectRaycastDef : SpellEffectDef
    {
        public List<ResourceRef<SpellDef>> AppliedSpells { get; set; }
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public UnityRef<UnityEngine.GameObject> ShotFX { get; set; }
        public UnityRef<UnityEngine.GameObject> MuzzleFX { get; set; }
        public float Distance { get; set; }
    }
}