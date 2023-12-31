// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public class SpawnDaemonAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonAlways
    {
        public SpawnDaemonAlways(SharedCode.Entities.GameObjectEntities.ISpawnDaemon deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnDaemon __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnDaemon)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.SpawnDaemonSceneDef SceneDef => __deltaObject__.SceneDef;
        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedAlways WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedAlways)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncAlways MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncAlways)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public System.Threading.Tasks.Task<bool> TryPlaceObjectNear(SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef pointType, SharedCode.Entities.GameObjectEntities.IEntityObjectDef objDef, SharedCode.Utils.Vector3 pos, bool ignoreGeometry)
        {
            return __deltaObject__.TryPlaceObjectNear(pointType, objDef, pos, ignoreGeometry);
        }

        public System.Threading.Tasks.Task<bool> ActivateTemplatePointsBatch(System.Collections.Generic.List<SharedCode.Entities.GameObjectEntities.SpawnTemplateDef> def)
        {
            return __deltaObject__.ActivateTemplatePointsBatch(def);
        }

        public System.Threading.Tasks.Task NotifyOfEntityDissipation(SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, System.Guid guid, int typeId)
        {
            return __deltaObject__.NotifyOfEntityDissipation(pos, rot, point, guid, typeId);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 13:
                    currProperty = SceneDef;
                    break;
                case 14:
                    currProperty = Def;
                    break;
                case 15:
                    currProperty = MapOwner;
                    break;
                case 16:
                    currProperty = StaticIdFromExport;
                    break;
                case 17:
                    currProperty = WorldSpaced;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 2031090926;
    }

    public class SpawnDaemonClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonClientBroadcast
    {
        public SpawnDaemonClientBroadcast(SharedCode.Entities.GameObjectEntities.ISpawnDaemon deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnDaemon __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnDaemon)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.SpawnDaemonSceneDef SceneDef => __deltaObject__.SceneDef;
        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientBroadcast WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientBroadcast)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ILinksEngineClientBroadcast LinksEngine => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ILinksEngineClientBroadcast)__deltaObject__.LinksEngine?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientBroadcast MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientBroadcast)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public System.Threading.Tasks.Task<bool> TryPlaceObjectNear(SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef pointType, SharedCode.Entities.GameObjectEntities.IEntityObjectDef objDef, SharedCode.Utils.Vector3 pos, bool ignoreGeometry)
        {
            return __deltaObject__.TryPlaceObjectNear(pointType, objDef, pos, ignoreGeometry);
        }

        public System.Threading.Tasks.Task<bool> ActivateTemplatePointsBatch(System.Collections.Generic.List<SharedCode.Entities.GameObjectEntities.SpawnTemplateDef> def)
        {
            return __deltaObject__.ActivateTemplatePointsBatch(def);
        }

        public System.Threading.Tasks.Task NotifyOfEntityDissipation(SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, System.Guid guid, int typeId)
        {
            return __deltaObject__.NotifyOfEntityDissipation(pos, rot, point, guid, typeId);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 13:
                    currProperty = SceneDef;
                    break;
                case 14:
                    currProperty = Def;
                    break;
                case 15:
                    currProperty = MapOwner;
                    break;
                case 16:
                    currProperty = StaticIdFromExport;
                    break;
                case 17:
                    currProperty = WorldSpaced;
                    break;
                case 18:
                    currProperty = LinksEngine;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1725855683;
    }

    public class SpawnDaemonClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonClientFullApi
    {
        public SpawnDaemonClientFullApi(SharedCode.Entities.GameObjectEntities.ISpawnDaemon deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnDaemon __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnDaemon)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1374437197;
    }

    public class SpawnDaemonClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonClientFull
    {
        public SpawnDaemonClientFull(SharedCode.Entities.GameObjectEntities.ISpawnDaemon deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnDaemon __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnDaemon)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.SpawnDaemonSceneDef SceneDef => __deltaObject__.SceneDef;
        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientFull WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientFull)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ILinksEngineClientFull LinksEngine => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ILinksEngineClientFull)__deltaObject__.LinksEngine?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFull MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFull)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public System.Threading.Tasks.Task<bool> TryPlaceObjectNear(SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef pointType, SharedCode.Entities.GameObjectEntities.IEntityObjectDef objDef, SharedCode.Utils.Vector3 pos, bool ignoreGeometry)
        {
            return __deltaObject__.TryPlaceObjectNear(pointType, objDef, pos, ignoreGeometry);
        }

        public System.Threading.Tasks.Task<bool> ActivateTemplatePointsBatch(System.Collections.Generic.List<SharedCode.Entities.GameObjectEntities.SpawnTemplateDef> def)
        {
            return __deltaObject__.ActivateTemplatePointsBatch(def);
        }

        public System.Threading.Tasks.Task NotifyOfEntityDissipation(SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, System.Guid guid, int typeId)
        {
            return __deltaObject__.NotifyOfEntityDissipation(pos, rot, point, guid, typeId);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 13:
                    currProperty = SceneDef;
                    break;
                case 14:
                    currProperty = Def;
                    break;
                case 15:
                    currProperty = MapOwner;
                    break;
                case 16:
                    currProperty = StaticIdFromExport;
                    break;
                case 17:
                    currProperty = WorldSpaced;
                    break;
                case 18:
                    currProperty = LinksEngine;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -15017785;
    }

    public class SpawnDaemonServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonServerApi
    {
        public SpawnDaemonServerApi(SharedCode.Entities.GameObjectEntities.ISpawnDaemon deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnDaemon __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnDaemon)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1093813818;
    }

    public class SpawnDaemonServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonServer
    {
        public SpawnDaemonServer(SharedCode.Entities.GameObjectEntities.ISpawnDaemon deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnDaemon __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnDaemon)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.SpawnDaemonSceneDef SceneDef => __deltaObject__.SceneDef;
        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedServer WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedServer)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ILinksEngineServer LinksEngine => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ILinksEngineServer)__deltaObject__.LinksEngine?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServer MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServer)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public System.Threading.Tasks.Task<bool> NotifyOfObjectDestruction(System.Guid id, int typeId)
        {
            return __deltaObject__.NotifyOfObjectDestruction(id, typeId);
        }

        public System.Threading.Tasks.Task<bool> ResetDaemon()
        {
            return __deltaObject__.ResetDaemon();
        }

        public System.Threading.Tasks.Task<bool> TryPlaceObjectNear(SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef pointType, SharedCode.Entities.GameObjectEntities.IEntityObjectDef objDef, SharedCode.Utils.Vector3 pos, bool ignoreGeometry)
        {
            return __deltaObject__.TryPlaceObjectNear(pointType, objDef, pos, ignoreGeometry);
        }

        public System.Threading.Tasks.Task<bool> ActivateTemplatePointsBatch(System.Collections.Generic.List<SharedCode.Entities.GameObjectEntities.SpawnTemplateDef> def)
        {
            return __deltaObject__.ActivateTemplatePointsBatch(def);
        }

        public System.Threading.Tasks.Task NotifyOfEntityDissipation(SharedCode.Utils.Vector3 pos, SharedCode.Utils.Quaternion rot, SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point, System.Guid guid, int typeId)
        {
            return __deltaObject__.NotifyOfEntityDissipation(pos, rot, point, guid, typeId);
        }

        public System.Threading.Tasks.Task<bool> AwaitUnityThread()
        {
            return __deltaObject__.AwaitUnityThread();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 13:
                    currProperty = SceneDef;
                    break;
                case 14:
                    currProperty = Def;
                    break;
                case 15:
                    currProperty = MapOwner;
                    break;
                case 16:
                    currProperty = StaticIdFromExport;
                    break;
                case 17:
                    currProperty = WorldSpaced;
                    break;
                case 18:
                    currProperty = LinksEngine;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1111030367;
    }
}

namespace GeneratedCode.DeltaObjects
{
    public class SpawnedObjectAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnedObjectAlways
    {
        public SpawnedObjectAlways(SharedCode.Entities.GameObjectEntities.ISpawnedObject deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnedObject __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnedObject)__deltaObjectBase__;
            }
        }

        public override int TypeId => -149716878;
    }

    public class SpawnedObjectClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnedObjectClientBroadcast
    {
        public SpawnedObjectClientBroadcast(SharedCode.Entities.GameObjectEntities.ISpawnedObject deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnedObject __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnedObject)__deltaObjectBase__;
            }
        }

        public SharedCode.EntitySystem.OuterRef<SharedCode.Entities.GameObjectEntities.ISpawnDaemon> Spawner => __deltaObject__.Spawner;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Spawner;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 496244327;
    }

    public class SpawnedObjectClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnedObjectClientFullApi
    {
        public SpawnedObjectClientFullApi(SharedCode.Entities.GameObjectEntities.ISpawnedObject deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnedObject __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnedObject)__deltaObjectBase__;
            }
        }

        public override int TypeId => 2050753552;
    }

    public class SpawnedObjectClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnedObjectClientFull
    {
        public SpawnedObjectClientFull(SharedCode.Entities.GameObjectEntities.ISpawnedObject deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnedObject __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnedObject)__deltaObjectBase__;
            }
        }

        public SharedCode.EntitySystem.OuterRef<SharedCode.Entities.GameObjectEntities.ISpawnDaemon> Spawner => __deltaObject__.Spawner;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Spawner;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -309960553;
    }

    public class SpawnedObjectServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnedObjectServerApi
    {
        public SpawnedObjectServerApi(SharedCode.Entities.GameObjectEntities.ISpawnedObject deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnedObject __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnedObject)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1140153003;
    }

    public class SpawnedObjectServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnedObjectServer
    {
        public SpawnedObjectServer(SharedCode.Entities.GameObjectEntities.ISpawnedObject deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISpawnedObject __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISpawnedObject)__deltaObjectBase__;
            }
        }

        public SharedCode.EntitySystem.OuterRef<SharedCode.Entities.GameObjectEntities.ISpawnDaemon> Spawner => __deltaObject__.Spawner;
        public SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef PointType => __deltaObject__.PointType;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Spawner;
                    break;
                case 11:
                    currProperty = PointType;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 608007826;
    }
}