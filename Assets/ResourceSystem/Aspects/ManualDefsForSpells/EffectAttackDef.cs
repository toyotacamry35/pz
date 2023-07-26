using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Misc;
using Newtonsoft.Json;
using ResourceSystem.Aspects.Misc;
using SharedCode.Wizardry;
using Src.ManualDefsForSpells;

namespace ColonyShared.SharedCode.Aspects.ManualDefsForSpells
{
    public class EffectAttackDef : SpellEffectDef
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<AttackDef> Attack;
        [JsonProperty(Required = Required.Always)] public ResourceRef<EffectAnimatorDef.StateDef> Animation;
        public ResourceRef<SpellIdDef> AnimationSpellId;
        public ResourceRef<GameObjectMarkerDef> ColliderMarker;
        public List<ResourceRef<GameObjectMarkerDef>> TrajectoryMarkers;
        public ResourceRef<SpellEntityDef> Target = new SpellTargetDef();
    }
}