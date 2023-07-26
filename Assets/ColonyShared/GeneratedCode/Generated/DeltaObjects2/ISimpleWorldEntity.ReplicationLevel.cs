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
    public class SimpleWorldEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleWorldEntityAlways
    {
        public SimpleWorldEntityAlways(SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity)__deltaObjectBase__;
            }
        }

        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedAlways WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedAlways)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncAlways MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncAlways)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public System.Threading.Tasks.Task<bool> NameSet(string value)
        {
            return __deltaObject__.NameSet(value);
        }

        public System.Threading.Tasks.Task<bool> PrefabSet(string value)
        {
            return __deltaObject__.PrefabSet(value);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Name;
                    break;
                case 11:
                    currProperty = Prefab;
                    break;
                case 12:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 13:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 15:
                    currProperty = WorldSpaced;
                    break;
                case 16:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1860107453;
    }

    public class SimpleWorldEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleWorldEntityClientBroadcast
    {
        public SimpleWorldEntityClientBroadcast(SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity)__deltaObjectBase__;
            }
        }

        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientBroadcast WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientBroadcast)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientBroadcast MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientBroadcast)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public System.Threading.Tasks.Task<bool> NameSet(string value)
        {
            return __deltaObject__.NameSet(value);
        }

        public System.Threading.Tasks.Task<bool> PrefabSet(string value)
        {
            return __deltaObject__.PrefabSet(value);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Name;
                    break;
                case 11:
                    currProperty = Prefab;
                    break;
                case 12:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 13:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 15:
                    currProperty = WorldSpaced;
                    break;
                case 16:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -136589805;
    }

    public class SimpleWorldEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleWorldEntityClientFullApi
    {
        public SimpleWorldEntityClientFullApi(SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1491414663;
    }

    public class SimpleWorldEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleWorldEntityClientFull
    {
        public SimpleWorldEntityClientFull(SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity)__deltaObjectBase__;
            }
        }

        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientFull WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientFull)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFull MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFull)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public System.Threading.Tasks.Task<bool> NameSet(string value)
        {
            return __deltaObject__.NameSet(value);
        }

        public System.Threading.Tasks.Task<bool> PrefabSet(string value)
        {
            return __deltaObject__.PrefabSet(value);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Name;
                    break;
                case 11:
                    currProperty = Prefab;
                    break;
                case 12:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 13:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 15:
                    currProperty = WorldSpaced;
                    break;
                case 16:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1315251300;
    }

    public class SimpleWorldEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleWorldEntityServerApi
    {
        public SimpleWorldEntityServerApi(SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1391114112;
    }

    public class SimpleWorldEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleWorldEntityServer
    {
        public SimpleWorldEntityServer(SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.GameObjectEntities.ISimpleWorldEntity)__deltaObjectBase__;
            }
        }

        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedServer WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedServer)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServer MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServer)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public System.Threading.Tasks.Task<bool> NameSet(string value)
        {
            return __deltaObject__.NameSet(value);
        }

        public System.Threading.Tasks.Task<bool> PrefabSet(string value)
        {
            return __deltaObject__.PrefabSet(value);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Name;
                    break;
                case 11:
                    currProperty = Prefab;
                    break;
                case 12:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 13:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 15:
                    currProperty = WorldSpaced;
                    break;
                case 16:
                    currProperty = MovementSync;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1402006521;
    }
}