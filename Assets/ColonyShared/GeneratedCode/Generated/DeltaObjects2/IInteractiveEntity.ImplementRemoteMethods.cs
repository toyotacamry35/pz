// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IInteractiveEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> DestroyImpl();
        System.Threading.Tasks.Task LifespanExpiredImpl();
        System.Threading.Tasks.Task<bool> NameSetImpl(string value);
        System.Threading.Tasks.Task<bool> PrefabSetImpl(string value);
    }
}

namespace GeneratedCode.DeltaObjects
{
    public interface ICorpseInteractiveEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveAllItemsImpl(SharedCode.EntitySystem.PropertyAddress source, SharedCode.EntitySystem.PropertyAddress destination);
        System.Threading.Tasks.Task<bool> NameSetImpl(string value);
        System.Threading.Tasks.Task<bool> PrefabSetImpl(string value);
        System.Threading.Tasks.Task<bool> DestroyImpl();
        System.Threading.Tasks.Task LifespanExpiredImpl();
        System.Threading.Tasks.Task<ResourceSystem.Utils.OuterRef> GetOpenOuterRefImpl(ResourceSystem.Utils.OuterRef oref);
    }
}