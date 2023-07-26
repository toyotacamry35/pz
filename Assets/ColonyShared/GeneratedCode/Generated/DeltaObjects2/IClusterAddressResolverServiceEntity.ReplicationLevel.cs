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
    public class ClusterAddressResolverServiceEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityAlways
    {
        public ClusterAddressResolverServiceEntityAlways(SharedCode.Entities.Service.IClusterAddressResolverServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IClusterAddressResolverServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1562548848;
    }

    public class ClusterAddressResolverServiceEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientBroadcast
    {
        public ClusterAddressResolverServiceEntityClientBroadcast(SharedCode.Entities.Service.IClusterAddressResolverServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IClusterAddressResolverServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 830924878;
    }

    public class ClusterAddressResolverServiceEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientFullApi
    {
        public ClusterAddressResolverServiceEntityClientFullApi(SharedCode.Entities.Service.IClusterAddressResolverServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IClusterAddressResolverServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1944397086;
    }

    public class ClusterAddressResolverServiceEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientFull
    {
        public ClusterAddressResolverServiceEntityClientFull(SharedCode.Entities.Service.IClusterAddressResolverServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IClusterAddressResolverServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1990544470;
    }

    public class ClusterAddressResolverServiceEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityServerApi
    {
        public ClusterAddressResolverServiceEntityServerApi(SharedCode.Entities.Service.IClusterAddressResolverServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IClusterAddressResolverServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -562557664;
    }

    public class ClusterAddressResolverServiceEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityServer
    {
        public ClusterAddressResolverServiceEntityServer(SharedCode.Entities.Service.IClusterAddressResolverServiceEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Service.IClusterAddressResolverServiceEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.ValueTask<System.Guid> GetEntityAddressRepositoryId(int typeId, System.Guid entityId)
        {
            return __deltaObject__.GetEntityAddressRepositoryId(typeId, entityId);
        }

        public System.Threading.Tasks.ValueTask<System.Collections.Generic.IReadOnlyList<(System.Guid entityId, System.Guid repoId)>> GetAllEntitiesByType(int typeId)
        {
            return __deltaObject__.GetAllEntitiesByType(typeId);
        }

        public System.Threading.Tasks.Task SetEntityAddressRepositoryId(int typeId, System.Guid entityId, System.Guid repositoryId)
        {
            return __deltaObject__.SetEntityAddressRepositoryId(typeId, entityId, repositoryId);
        }

        public System.Threading.Tasks.Task SetEntitiesAddressRepositoryId(System.Collections.Generic.Dictionary<int, System.Guid> entities, System.Guid repositoryId)
        {
            return __deltaObject__.SetEntitiesAddressRepositoryId(entities, repositoryId);
        }

        public System.Threading.Tasks.Task RemoveEntityAddressRepositoryId(int typeId, System.Guid entityId)
        {
            return __deltaObject__.RemoveEntityAddressRepositoryId(typeId, entityId);
        }

        public override int TypeId => -2115437475;
    }
}