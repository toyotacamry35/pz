using GeneratedCode.Custom.Config;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.ResourcesSystem.Base;
using System.Collections.Concurrent;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using SharedCode.Serializers;
using UnityEngine;
using Color = SharedCode.Utils.Color;
using Quaternion = SharedCode.Utils.Quaternion;
using Vector3 = SharedCode.Utils.Vector3;

namespace SharedCode.Entities.GameObjectEntities
{
    public interface IUnityService : IUnityCheatsHandler
    {
        void RegisterRepo(IEntitiesRepository repo);
        Task<bool> LoadLevel(IEntitiesRepository fromRepo, Guid sceneId, MapDef map);
        Task<bool> UnloadLevel(IEntitiesRepository fromRepo);
        ValueTask<Vector3> GetDropPosition(Vector3 playerPosition, Quaternion playerRotation);

        void DestroyObject(IEntitiesRepository repo, OuterRef<IEntityObject> creator, IEntity entity);

        // DO NOT USE
        SuspendingAwaitable RunInUnityThread(System.Action action);
        bool IsReady();
        void SetGCEnabled(bool enabled);
        void DrawShape(ShapeDef shapeType, Color color, float duration);
        void PostMessageFromMap(string name, string message);
    }

    // This i-face is just to explicitly show, all its heirs are bound - they do 1 work (by forwarding a call from 1-to-another in order)
    public interface IUnityCheatsHandler
    {
        void MainUnityThreadOnServerSleep(bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime);
        Task<Transform> GetClosestPlayerSpawnPointTransform(IEntitiesRepository repo, Vector3 pos);
    }

    public interface IUnityObjectHandle
    {

    }
    public static class EntitytObjectsUnitySpawnService
    {
        //#Note: `UnitySpawnService` not good name - it's actually Unity service (the only for now)
        public static IUnityService SpawnService;
    }
    
    public static class EventsForSauron
    {
        public static ConcurrentDictionary<OuterRef<IEntity>, (object meta, BaseResource resource)> Events = 
            new ConcurrentDictionary<OuterRef<IEntity>, (object meta, BaseResource resource)>();
        public static volatile bool Watched;
        public static void PostEvent(IDeltaObject obj, object meta, BaseResource resource)
        {
            if (!Watched)
                return;
            PostEvent(new OuterRef<IEntity>(obj.ParentEntityId, obj.ParentTypeId), meta, resource);
        }
        public static void PostEvent(OuterRef<IEntity> ent, object meta, BaseResource resource)
        {
            if (!Watched)
                return;
            Events[ent] = (meta, resource);
        }

        public static void Clear()
        {
            Events.Clear();
        }
        public static void Watch()
        {
            Watched = true;
        }
        public static void StopWatch()
        {
            Watched = false;
            Clear();
        }
    }

    public interface IEntityObject : IHasMapped, IHasWorldSpaced
    {
        [LockFreeReadonlyProperty]
        [ReplicationLevel(ReplicationLevel.Always)]
        IEntityObjectDef Def { get; set; }

    }

    public interface IEntityObjectWithCustomUnityInstantiation : IEntityObject
    {
        [ReplicationLevel(ReplicationLevel.Always), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        ValueTask<bool> MustBeInstantiatedHere();

        [ReplicationLevel(ReplicationLevel.Always), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        ValueTask<UnityRef<GameObject>> GetPrefab();
    }
}
