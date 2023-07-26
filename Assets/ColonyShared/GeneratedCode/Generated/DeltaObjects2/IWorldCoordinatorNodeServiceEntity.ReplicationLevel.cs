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
    public class WorldCoordinatorNodeServiceEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldCoordinatorNodeServiceEntityAlways
    {
        public WorldCoordinatorNodeServiceEntityAlways(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1241319817;
    }

    public class WorldCoordinatorNodeServiceEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldCoordinatorNodeServiceEntityClientBroadcast
    {
        public WorldCoordinatorNodeServiceEntityClientBroadcast(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -810548650;
    }

    public class WorldCoordinatorNodeServiceEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldCoordinatorNodeServiceEntityClientFullApi
    {
        public WorldCoordinatorNodeServiceEntityClientFullApi(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1546066152;
    }

    public class WorldCoordinatorNodeServiceEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldCoordinatorNodeServiceEntityClientFull
    {
        public WorldCoordinatorNodeServiceEntityClientFull(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -2038792743;
    }

    public class WorldCoordinatorNodeServiceEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldCoordinatorNodeServiceEntityServerApi
    {
        public WorldCoordinatorNodeServiceEntityServerApi(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1303771720;
    }

    public class WorldCoordinatorNodeServiceEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldCoordinatorNodeServiceEntityServer
    {
        public WorldCoordinatorNodeServiceEntityServer(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.Service.CreateOrConfirmMapResult> RequestLoginToMap(SharedCode.Entities.Service.MapLoginMeta loginMeta)
        {
            return __deltaObject__.RequestLoginToMap(loginMeta);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.Service.CreateOrConfirmMapResult> RequestLogoutFromMap(System.Guid userId, bool terminal)
        {
            return __deltaObject__.RequestLogoutFromMap(userId, terminal);
        }

        public System.Threading.Tasks.Task<bool> CancelMapRequest(System.Guid requestId)
        {
            return __deltaObject__.CancelMapRequest(requestId);
        }

        public System.Threading.Tasks.Task<System.Guid> GetMapIdByUserId(System.Guid userId)
        {
            return __deltaObject__.GetMapIdByUserId(userId);
        }

        public System.Threading.Tasks.Task<string> GlobalNotification(string notificationText)
        {
            return __deltaObject__.GlobalNotification(notificationText);
        }

        public override int TypeId => -983234123;
    }
}