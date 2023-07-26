using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Aspects
{
    public class AttackStatModifierDef : AttackModifierDef
    {
        public ResourceRef<StatResource> Stat;
        public float Multiplier = 1;
        public float Summand = 0;
    }
}