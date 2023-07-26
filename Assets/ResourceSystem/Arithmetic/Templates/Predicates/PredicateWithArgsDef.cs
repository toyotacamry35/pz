using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateWithArgsDef : CalcerWithArgsDef<bool>
    {
        [UsedImplicitly] public ResourceRef<CalcerDef<bool>> Predicate { get => Calcer.Target; set => Calcer = value; }
    }
}
