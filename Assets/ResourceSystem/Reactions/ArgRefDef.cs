using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace ResourceSystem.Reactions
{
    public class ArgRefDef<T> : VarDef<T>, IArg<T>
    {
        [UsedImplicitly] public ResourceRef<ArgDef<T>> Arg;
        public ArgDef<T> Def => Arg;
        ArgDef IArg.Def => Def;
        public override string ToString() => $"{base.ToString()} => {Def}";
    }
}