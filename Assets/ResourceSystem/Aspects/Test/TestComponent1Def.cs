using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;

namespace Assets.Src.GameObjectAssembler.Editor
{
    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }

    public class TestComponent1Def : ComponentDef
    {
        public ResourceRef<TestComponent2Def> Comp2Ref { get; set; }
        public ResourceRef<ComponentDef> Ref { get; set; }
    }

    public class TestComponent2Def : ComponentDef
    {
        public float Field1 { get; set; }
    }

    public class TestEnumComponentDef : ComponentDef
    {
        public TestEnum Field { get; set; }
    }

    public class TestComponentPrototypeDef : ComponentDef
    {
        public ResourceRef<ComponentDef> Ref { get; set; }
        public ResourceRef<ComponentDef> Ref1 { get; set; }
        public ResourceRef<ComponentDef> Ref2 { get; set; }
        public ResourceRef<ComponentDef> Ref3 { get; set; }
        public float Value1 { get; set; }
        public float Value2 { get; set; }
        public List<ResourceRef<ComponentDef>> Components { get; set; }
    }

    public class TPCMStubDef : ComponentDef
    {
        public ResourceRef<PredicateDef> CanMoveAndRotate { get; set; }
        public ResourceRef<CalcerDef<float>> SpeedMultiplier { get; set; }
        public ResourceRef<StatResource> SpeedFactorStat { get; set; }

        public ResourceRef<CalcerDef<float>> CalcerFallDamage { get; set; }
    }
}


namespace Assets.Src.SpawnSystem
{
    public class TmpPlugEntityApiEgoComponentRetransatorDef : ComponentDef
    {
    }
}