using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Utils;

namespace ResourceSystem.Arithmetic.Templates.Calcers
{
    public class CalcerOwnerDef : CalcerDef<OuterRef>
    {
        public ResourceRef<CalcerDef<OuterRef>> Entity;
    }
}