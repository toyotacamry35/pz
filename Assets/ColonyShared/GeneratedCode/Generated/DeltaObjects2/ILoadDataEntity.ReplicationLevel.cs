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
    public class LoadDataEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoadDataEntityAlways
    {
        public LoadDataEntityAlways(SharedCode.Entities.Cloud.ILoadDataEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.ILoadDataEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.ILoadDataEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId)
        {
            return __deltaObject__.LoadEntity(typeId, entityId);
        }

        public override int TypeId => 831618039;
    }

    public class LoadDataEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoadDataEntityClientBroadcast
    {
        public LoadDataEntityClientBroadcast(SharedCode.Entities.Cloud.ILoadDataEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.ILoadDataEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.ILoadDataEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId)
        {
            return __deltaObject__.LoadEntity(typeId, entityId);
        }

        public override int TypeId => -343950379;
    }

    public class LoadDataEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoadDataEntityClientFullApi
    {
        public LoadDataEntityClientFullApi(SharedCode.Entities.Cloud.ILoadDataEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.ILoadDataEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.ILoadDataEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1695053720;
    }

    public class LoadDataEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoadDataEntityClientFull
    {
        public LoadDataEntityClientFull(SharedCode.Entities.Cloud.ILoadDataEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.ILoadDataEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.ILoadDataEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId)
        {
            return __deltaObject__.LoadEntity(typeId, entityId);
        }

        public override int TypeId => -1018191854;
    }

    public class LoadDataEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoadDataEntityServerApi
    {
        public LoadDataEntityServerApi(SharedCode.Entities.Cloud.ILoadDataEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.ILoadDataEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.ILoadDataEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -107449017;
    }

    public class LoadDataEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoadDataEntityServer
    {
        public LoadDataEntityServer(SharedCode.Entities.Cloud.ILoadDataEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.ILoadDataEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.ILoadDataEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId)
        {
            return __deltaObject__.LoadEntity(typeId, entityId);
        }

        public override int TypeId => -846299507;
    }
}