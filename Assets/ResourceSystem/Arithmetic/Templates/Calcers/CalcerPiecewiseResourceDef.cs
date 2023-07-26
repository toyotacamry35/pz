using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerPiecewiseResourceDef : CalcerDef<BaseResource>
    {
        public RangeResource[] Ranges { get; [UsedImplicitly] set; } = { }; 
        public ResourceRef<BaseResource> Else { get; [UsedImplicitly] set; }
    }

    public struct RangeResource
    {
        [UsedImplicitly] public ResourceRef<PredicateDef> Condition;
        [UsedImplicitly] public ResourceRef<BaseResource> Value;
    }
}
