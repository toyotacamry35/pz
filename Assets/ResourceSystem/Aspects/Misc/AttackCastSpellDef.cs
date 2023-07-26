using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ResourcesSystem.Loader;
using ResourceSystem.Reactions;
using ResourceSystem.Utils;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public class AttackCastSpellDef : AttackActionDef
    {
        [UsedImplicitly][JsonProperty(Required = Required.Always)] public ResourceRef<SpellDef> Spell { get; set; }
        
        [UsedImplicitly] public SpellParams Params { get; set; }

        [KnownToGameResources]
        public class SpellParams
        {
            public ResourceRef<ISpellParameterDef<float>> RecoilTime;
            public ResourceRef<ISpellParameterDef<float>> StaggerTime;
            public ResourceRef<ISpellParameterDef<float>> Damage;
            public ResourceRef<ISpellParameterDef<Vector2>> HitDirection;
            public ResourceRef<ISpellParameterDef<OuterRef>> Attacker;
            public ResourceRef<ISpellParameterDef<OuterRef>> Victim;
            public ResourceRef<ISpellParameterDef<Vector3>> HitPoint;
            public ResourceRef<ISpellParameterDef<Vector3>> HitLocalPoint;
            public ResourceRef<ISpellParameterDef<Quaternion>> HitRotation;
            public ResourceRef<ISpellParameterDef<Quaternion>> HitLocalRotation;
            public ResourceRef<ISpellParameterDef<string>> HitObject;
            public ResourceRef<ISpellParameterDef<BaseResource>> DamageType;
            public ResourceRef<ISpellParameterDef<BaseResource>> WeaponSize;
            public ResourceRef<ISpellParameterDef<BaseResource>> AttackType;
            public ResourceRef<ISpellParameterDef<BaseResource>> HitMaterial;
        }

        public override string ToString() => $"When:{When} Spell:{Spell.Target.____GetDebugAddress()}";

        public override BaseResource IdResource => Spell;
    }
}