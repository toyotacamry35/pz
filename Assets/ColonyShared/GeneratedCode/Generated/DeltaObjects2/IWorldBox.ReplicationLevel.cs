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
    public class WorldBoxAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldBoxAlways
    {
        public WorldBoxAlways(SharedCode.Entities.IWorldBox deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldBox __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldBox)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedAlways WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedAlways)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncAlways MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncAlways)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationAlways OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationAlways)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiAlways ContainerApi => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiAlways)__deltaObject__.ContainerApi?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
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
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = MapOwner;
                    break;
                case 12:
                    currProperty = StaticIdFromExport;
                    break;
                case 13:
                    currProperty = Name;
                    break;
                case 14:
                    currProperty = Prefab;
                    break;
                case 15:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 16:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 18:
                    currProperty = WorldSpaced;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                case 22:
                    currProperty = OwnerInformation;
                    break;
                case 23:
                    currProperty = ContainerApi;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1909247762;
    }

    public class WorldBoxClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldBoxClientBroadcast
    {
        public WorldBoxClientBroadcast(SharedCode.Entities.IWorldBox deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldBox __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldBox)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientBroadcast WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientBroadcast)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientBroadcast MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientBroadcast)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOpenMechanicsClientBroadcast OpenMechanics => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOpenMechanicsClientBroadcast)__deltaObject__.OpenMechanics?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientBroadcast OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientBroadcast)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiClientBroadcast ContainerApi => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiClientBroadcast)__deltaObject__.ContainerApi?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
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
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = MapOwner;
                    break;
                case 12:
                    currProperty = StaticIdFromExport;
                    break;
                case 13:
                    currProperty = Name;
                    break;
                case 14:
                    currProperty = Prefab;
                    break;
                case 15:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 16:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 18:
                    currProperty = WorldSpaced;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                case 21:
                    currProperty = OpenMechanics;
                    break;
                case 22:
                    currProperty = OwnerInformation;
                    break;
                case 23:
                    currProperty = ContainerApi;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -819732216;
    }

    public class WorldBoxClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldBoxClientFullApi
    {
        public WorldBoxClientFullApi(SharedCode.Entities.IWorldBox deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldBox __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldBox)__deltaObjectBase__;
            }
        }

        public override int TypeId => -463751413;
    }

    public class WorldBoxClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldBoxClientFull
    {
        public WorldBoxClientFull(SharedCode.Entities.IWorldBox deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldBox __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldBox)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientFull WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedClientFull)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFull MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFull)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public Assets.ColonyShared.SharedCode.Aspects.Damage.Templates.ItemSpecificStats SpecificStats => __deltaObject__.SpecificStats;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerClientFull Inventory => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerClientFull)__deltaObject__.Inventory?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOpenMechanicsClientFull OpenMechanics => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOpenMechanicsClientFull)__deltaObject__.OpenMechanics?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientFull OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientFull)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiClientFull ContainerApi => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiClientFull)__deltaObject__.ContainerApi?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
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
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = MapOwner;
                    break;
                case 12:
                    currProperty = StaticIdFromExport;
                    break;
                case 13:
                    currProperty = Name;
                    break;
                case 14:
                    currProperty = Prefab;
                    break;
                case 15:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 16:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 18:
                    currProperty = WorldSpaced;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                case 20:
                    currProperty = Inventory;
                    break;
                case 21:
                    currProperty = OpenMechanics;
                    break;
                case 22:
                    currProperty = OwnerInformation;
                    break;
                case 23:
                    currProperty = ContainerApi;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1598664345;
    }

    public class WorldBoxServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldBoxServerApi
    {
        public WorldBoxServerApi(SharedCode.Entities.IWorldBox deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldBox __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldBox)__deltaObjectBase__;
            }
        }

        public override int TypeId => 331788907;
    }

    public class WorldBoxServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldBoxServer
    {
        public WorldBoxServer(SharedCode.Entities.IWorldBox deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldBox __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldBox)__deltaObjectBase__;
            }
        }

        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def => __deltaObject__.Def;
        public GeneratedCode.MapSystem.MapOwner MapOwner => __deltaObject__.MapOwner;
        public System.Guid StaticIdFromExport => __deltaObject__.StaticIdFromExport;
        public string Name => __deltaObject__.Name;
        public string Prefab => __deltaObject__.Prefab;
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful => __deltaObject__.SomeUnknownResourceThatMayBeUseful;
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId => __deltaObject__.OnSceneObjectNetId;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedServer WorldSpaced => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldSpacedServer)__deltaObject__.WorldSpaced?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServer MovementSync => (GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServer)__deltaObject__.MovementSync?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public Assets.ColonyShared.SharedCode.Aspects.Damage.Templates.ItemSpecificStats SpecificStats => __deltaObject__.SpecificStats;
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServer Inventory => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServer)__deltaObject__.Inventory?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOpenMechanicsServer OpenMechanics => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOpenMechanicsServer)__deltaObject__.OpenMechanics?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationServer OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationServer)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiServer ContainerApi => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiServer)__deltaObject__.ContainerApi?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IDestroyableServer Destroyable => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IDestroyableServer)__deltaObject__.Destroyable?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public System.Threading.Tasks.Task<bool> NameSet(string value)
        {
            return __deltaObject__.NameSet(value);
        }

        public System.Threading.Tasks.Task<bool> PrefabSet(string value)
        {
            return __deltaObject__.PrefabSet(value);
        }

        public System.Threading.Tasks.Task<ResourceSystem.Utils.OuterRef> GetOpenOuterRef(ResourceSystem.Utils.OuterRef oref)
        {
            return __deltaObject__.GetOpenOuterRef(oref);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = MapOwner;
                    break;
                case 12:
                    currProperty = StaticIdFromExport;
                    break;
                case 13:
                    currProperty = Name;
                    break;
                case 14:
                    currProperty = Prefab;
                    break;
                case 15:
                    currProperty = SomeUnknownResourceThatMayBeUseful;
                    break;
                case 16:
                    currProperty = OnSceneObjectNetId;
                    break;
                case 18:
                    currProperty = WorldSpaced;
                    break;
                case 19:
                    currProperty = MovementSync;
                    break;
                case 20:
                    currProperty = Inventory;
                    break;
                case 21:
                    currProperty = OpenMechanics;
                    break;
                case 22:
                    currProperty = OwnerInformation;
                    break;
                case 23:
                    currProperty = ContainerApi;
                    break;
                case 25:
                    currProperty = Destroyable;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1097216715;
    }
}