using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectChangeAccumulatedStatDef : SpellEffectDef
    {
        public ResourceRef<StatResource> StatName { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<CalcerDef<float>> Summand { get; set; } =  new CalcerConstantDef<float>(0f);
        public float Multiplier { get; set; } = 0f;
        public bool UseClampMin { get; set; } = false;
        public float ClampMin { get; set; }
        public bool UseClampMax { get; set; } = false;
        public float ClampMax { get; set; }

        public override string ToString()
        {
            return $"<EffectChangeAccumulatedStatDef: {Target.Target?.ToString() ?? "null"} - {StatName.Target?.ToString() ?? "null"}>";
        }
    }
}
