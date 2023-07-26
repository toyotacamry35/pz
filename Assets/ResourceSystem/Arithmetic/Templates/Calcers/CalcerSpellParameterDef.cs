using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerSpellParameterDef<T> : CalcerDef<T>
    {
        public ResourceRef<SpellContextValueDef> Parameter { get; [UsedImplicitly] set; }
    }
}