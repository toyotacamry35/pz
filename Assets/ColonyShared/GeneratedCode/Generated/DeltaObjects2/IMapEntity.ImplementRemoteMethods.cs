// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IMapEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<bool> SetMapEntityStateImpl(SharedCode.MapSystem.MapEntityState state);
        System.Threading.Tasks.Task<bool> ChangeChunkDescriptionImpl(System.Guid descriptionId, SharedCode.MapSystem.MapChunkState newState, System.Guid unityRepositoryId);
        System.Threading.Tasks.Task<bool> OnLastUserLeftImpl();
        System.Threading.Tasks.Task SpawnNewBotsImpl(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath);
        System.Threading.Tasks.Task<bool> TryAquireSpawnRightsForPointsSetImpl(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef);
        System.Threading.Tasks.Task<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPointImpl(SharedCode.Utils.Vector3 point);
        System.Threading.Tasks.Task<bool> NotifyAllCharactersViaChatImpl(string text);
    }
}