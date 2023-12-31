// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IRealmsCollectionEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> AddRealmImpl(System.Guid mapId, SharedCode.Aspects.Sessions.RealmRulesDef realmDef);
        System.Threading.Tasks.Task<bool> RemoveRealmImpl(System.Guid mapId);
    }
}

namespace GeneratedCode.DeltaObjects
{
    public interface IRealmEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> TryAttachImpl(System.Guid account);
        System.Threading.Tasks.Task<bool> EnterImpl(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> account);
        System.Threading.Tasks.Task<bool> LeaveImpl(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> account);
        System.Threading.Tasks.Task<bool> AddMapImpl(System.Guid mapId, SharedCode.MapSystem.MapMeta mapMeta);
        System.Threading.Tasks.Task<bool> RemoveMapImpl(System.Guid mapId);
        System.Threading.Tasks.Task<bool> SetActiveImpl(bool active);
        System.Threading.Tasks.Task<bool> SetMapDeadImpl(System.Guid mapId);
    }
}