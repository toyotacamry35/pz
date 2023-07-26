using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Utils;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateSameFactionDef : PredicateDef
    {
        public ResourceRef<CalcerDef<OuterRef>> Entity;
    }
}