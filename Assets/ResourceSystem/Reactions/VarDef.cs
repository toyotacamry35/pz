using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Reactions
{
    public abstract class VarDef : BaseResource, IVar {}

    public abstract class VarDef<T>  : VarDef, IVar<T> {}
}
