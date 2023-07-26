using System;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using ResourceSystem.Reactions;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerArgDef<T> : CalcerDef<T>
    {
        [Obsolete] public string Arg { get; [UsedImplicitly] set; }
        public ResourceRef<ArgDef<T>> ArgDef { get; [UsedImplicitly] set; }
    }
}
