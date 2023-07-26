using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Aspects
{
    public class AttackStatOverrideDef : AttackModifierDef
    {
        public ResourceRef<StatResource> Stat;
        public float Value;
    }
}