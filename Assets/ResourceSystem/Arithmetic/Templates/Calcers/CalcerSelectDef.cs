using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    // То же что CalcerPiecewise только generic
    public class CalcerSelectDef<T> : CalcerDef<T>
    {
        [JsonProperty(Required = Required.Always)] public Block[] Ranges { get; [UsedImplicitly] set; } = { };

        public ResourceRef<CalcerDef<T>> Default { get; [UsedImplicitly] set; }
        
        public struct Block
        {
            [UsedImplicitly] public ResourceRef<PredicateDef> Condition;
            [UsedImplicitly] public ResourceRef<CalcerDef<T>> Value;
        }
    }
}