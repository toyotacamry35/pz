using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerWithArgsDef<ReturnType> : CalcerDef<ReturnType>
    {
        public ResourceRef<CalcerDef<ReturnType>> Calcer { get; [UsedImplicitly] set; }
        public Dictionary<string,ResourceRef<CalcerDef>> Args { get; [UsedImplicitly] set; }
    }
}
