using System;
using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Reactions
{
    public abstract class ArgDef : BaseResource
    {
        public abstract Type ValueType { get; }
    }

    public class ArgDef<T> : ArgDef
    {
        public override Type ValueType => typeof(T);
    }
}
