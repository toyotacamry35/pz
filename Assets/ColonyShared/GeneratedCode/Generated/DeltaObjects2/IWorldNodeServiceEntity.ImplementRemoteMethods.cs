// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IWorldNodeServiceEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> IsReadyImpl();
        System.Threading.Tasks.Task<bool> HostUnityMapChunkImpl(GeneratedCode.Custom.Config.MapDef mapChunk);
        System.Threading.Tasks.Task<bool> HostUnityMapChunkImpl(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId, System.Guid mapInstanceRepositoryId);
        System.Threading.Tasks.Task<bool> HostedUnityMapChunkImpl(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId);
        System.Threading.Tasks.Task<bool> SetStateImpl(SharedCode.Entities.Service.WorldNodeServiceState state);
        System.Threading.Tasks.Task<bool> InitializePortsImpl();
        System.Threading.Tasks.ValueTask<bool> CanBuildHereImpl(SharedCode.Entities.GameObjectEntities.IEntityObjectDef entityObjectDef, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 position, SharedCode.Utils.Vector3 scale, SharedCode.Utils.Quaternion rotation);
        System.Threading.Tasks.ValueTask<SharedCode.Utils.Vector3> GetDropPositionImpl(SharedCode.Utils.Vector3 playerPosition, SharedCode.Utils.Quaternion playerRotation);
    }
}