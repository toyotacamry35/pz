using System;
using System.Threading.Tasks;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using GeneratedCode.Custom.Config;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils;

namespace LocalServer.UnityMocks
{
    public class UnityServiceMock: IUnityService
    {
        public async Task<bool> UnloadLevel(IEntitiesRepository fromRepo)
        {

            return true;
        }
        public void NotifyOfClientAuthority(OuterRef<IEntity> entity)
        {
        }

        public void NotifyOfServerAuthority(OuterRef<IEntity> entity)
        {
        }

        public void NotifyOfClientAuthorityLost(OuterRef<IEntity> entity)
        {
        }

        public void NotifyOfServerAuthorityLost(OuterRef<IEntity> entity)
        {
        }

        public Task<bool> LoadLevel(IEntitiesRepository fromRepo, Guid sceneId, MapDef map)
        {
            return Task.FromResult(true);
        }

        public ValueTask<IUnityObjectHandle> InstantiateObject(IEntitiesRepository repo, OuterRef<IEntityObject> creator, IEntityObjectDef def, Vector3 pos, Quaternion rot)
        {
            return new ValueTask<IUnityObjectHandle>(new UnityObjectHandleMock(new object()));

        }

        public void DestroyObject(IEntitiesRepository repo, OuterRef<IEntityObject> creator, IEntity entity)
        {
        }

        public SuspendingAwaitable RunInUnityThread(Action action)
        {
            action();
            return new SuspendingAwaitable(Task.CompletedTask);
        }

        public ValueTask<bool> CanBuildHere(IEntityObjectDef entityObjectDef, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            return new ValueTask<bool>(true);
        }

        public ValueTask<Vector3> GetDropPosition(Vector3 playerPosition, Quaternion playerRotation)
        {
            return new ValueTask<Vector3>(playerPosition);
        }
        public void RegisterRepo(IEntitiesRepository entitiesRepository)
        {
        }

        public bool IsReady()
        {
            return true;
        }

        public void SetGCEnabled(bool enabled)
        {
        }

        public void DrawShape(ShapeDef shapeType, Color color, float duration)
        {
           
        }

        public void MainUnityThreadOnServerSleep(bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime)
        {
        }

        public Task<Transform> GetClosestPlayerSpawnPointTransform(IEntitiesRepository repo, Vector3 pos)
        {
            return default;
        }

        public void PostMessageFromMap(string name, string message)
        {

        }
    }
}
