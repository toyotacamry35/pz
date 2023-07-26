using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Utils;

namespace ResourceSystem.Arithmetic.Templates.Calcers
{
    public class CalcerFactionDef : CalcerDef<BaseResource>
    {
        public ResourceRef<CalcerDef<OuterRef>> Entity;
    }
}