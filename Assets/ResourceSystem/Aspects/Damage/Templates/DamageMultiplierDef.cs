using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public class DamageMultiplierDef : SaveableCalcerFloatDef
    {
        public new static DamageMultiplierDef Empty => _Empty;

        private static readonly ResourceRef<DamageMultiplierDef> _Empty = new ResourceRef<DamageMultiplierDef>("/UtilPrefabs/EmptyDamageMultiplier");
    }

    public static class DamageMultiplierDefExtension
    {
        public static bool IsEmpty(this DamageMultiplierDef def) => def == null || def.Calcer == null || def == DamageMultiplierDef.Empty;
        public static CalcerDef<float> GetCalcer(this DamageMultiplierDef def) => def.IsEmpty() ? null : def.Calcer;
    }
}