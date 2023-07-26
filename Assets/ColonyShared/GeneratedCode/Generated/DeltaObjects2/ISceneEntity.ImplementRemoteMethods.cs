// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface ISceneEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> SpawnImpl();
        System.Threading.Tasks.Task<bool> DespawnImpl();
        System.Threading.Tasks.Task<bool> SetLoadableObjImpl(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> obj, System.Guid fromStatic, SharedCode.Utils.Vector3 wsPos);
        System.Threading.Tasks.Task<bool> RemoveObjectImpl(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> obj);
        System.Threading.Tasks.Task<bool> LoadEntityImpl(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> obj);
        System.Threading.Tasks.Task<bool> FinishLoadingImpl();
    }
}

namespace GeneratedCode.DeltaObjects
{
    public interface IEventPointImplementRemoteMethods
    {
        System.Threading.Tasks.Task<SharedCode.Entities.GameObjectEntities.IEntityObjectDef> LoadEventImpl();
        System.Threading.Tasks.Task<bool> AssignEventImpl(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> newEvent, SharedCode.Entities.GameObjectEntities.EventInstanceDef eventDef, Scripting.ScriptingContext ctx);
        System.Threading.Tasks.Task<bool> RemoveEventImpl();
    }
}

namespace GeneratedCode.DeltaObjects
{
    public interface IStorytellerImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> TickImpl();
        System.Threading.Tasks.Task<bool> RegisterFromStaticSceneImpl();
    }
}

namespace GeneratedCode.DeltaObjects
{
    public interface IEventInstanceImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> StopImpl();
    }
}