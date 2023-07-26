using System;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Utils;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerSquadIdDef : CalcerDef<Guid>
    {
        public ResourceRef<CalcerDef<OuterRef>> Entity;
    }
}