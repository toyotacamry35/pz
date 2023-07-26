using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using ResourceSystem.Reactions;
using ResourceSystem.Utils;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    [KnownToGameResources]
    public class AttackArgsMappingDef
    {
        public ResourceRef<ArgDef<OuterRef>> Attacker;
        public ResourceRef<ArgDef<OuterRef>> Victim;
        public ResourceRef<ArgDef<float>> RecoilTime;
        public ResourceRef<ArgDef<float>> StaggerTime;
        public ResourceRef<ArgDef<float>> Damage;
        public ResourceRef<ArgDef<Vector2>> HitDirection;
        public ResourceRef<ArgDef<Vector3>> HitPoint;
        public ResourceRef<ArgDef<Vector3>> HitLocalPoint;
        public ResourceRef<ArgDef<Quaternion>> HitRotation;
        public ResourceRef<ArgDef<Quaternion>> HitLocalRotation;
        public ResourceRef<ArgDef<string>> HitObject;
        public ResourceRef<ArgDef<BaseResource>> DamageType;
        public ResourceRef<ArgDef<BaseResource>> WeaponSize;
        public ResourceRef<ArgDef<BaseResource>> AttackType;
        public ResourceRef<ArgDef<BaseResource>> HitMaterial;
    }
}