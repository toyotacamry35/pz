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
    public class SimpleMovementSyncAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncAlways
    {
        public SimpleMovementSyncAlways(SharedCode.MovementSync.ISimpleMovementSync deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.ISimpleMovementSync __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.ISimpleMovementSync)__deltaObjectBase__;
            }
        }

        public System.Type GridSyncType => __deltaObject__.GridSyncType;
        public SharedCode.MovementSync.SimpleMovementStateEvent OnMovementStateChanged => __deltaObject__.OnMovementStateChanged;
        public bool VisibilityOff => __deltaObject__.VisibilityOff;
        public SharedCode.Entities.Transform __SyncTransform => __deltaObject__.__SyncTransform;
        public SharedCode.Entities.Transform Transform => __deltaObject__.Transform;
        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public SharedCode.Utils.Quaternion Rotation => __deltaObject__.Rotation;
        public SharedCode.Utils.Vector3 Scale => __deltaObject__.Scale;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = __SyncTransform;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -2016995409;
    }

    public class SimpleMovementSyncClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientBroadcast
    {
        public SimpleMovementSyncClientBroadcast(SharedCode.MovementSync.ISimpleMovementSync deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.ISimpleMovementSync __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.ISimpleMovementSync)__deltaObjectBase__;
            }
        }

        public System.Type GridSyncType => __deltaObject__.GridSyncType;
        public SharedCode.MovementSync.SimpleMovementStateEvent OnMovementStateChanged => __deltaObject__.OnMovementStateChanged;
        public bool VisibilityOff => __deltaObject__.VisibilityOff;
        public SharedCode.Entities.Transform __SyncTransform => __deltaObject__.__SyncTransform;
        public SharedCode.Entities.Transform Transform => __deltaObject__.Transform;
        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public SharedCode.Utils.Quaternion Rotation => __deltaObject__.Rotation;
        public SharedCode.Utils.Vector3 Scale => __deltaObject__.Scale;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = __SyncTransform;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 841511014;
    }

    public class SimpleMovementSyncClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFullApi
    {
        public SimpleMovementSyncClientFullApi(SharedCode.MovementSync.ISimpleMovementSync deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.ISimpleMovementSync __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.ISimpleMovementSync)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1660791482;
    }

    public class SimpleMovementSyncClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncClientFull
    {
        public SimpleMovementSyncClientFull(SharedCode.MovementSync.ISimpleMovementSync deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.ISimpleMovementSync __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.ISimpleMovementSync)__deltaObjectBase__;
            }
        }

        public System.Type GridSyncType => __deltaObject__.GridSyncType;
        public SharedCode.MovementSync.SimpleMovementStateEvent OnMovementStateChanged => __deltaObject__.OnMovementStateChanged;
        public bool VisibilityOff => __deltaObject__.VisibilityOff;
        public SharedCode.Entities.Transform __SyncTransform => __deltaObject__.__SyncTransform;
        public SharedCode.Entities.Transform Transform => __deltaObject__.Transform;
        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public SharedCode.Utils.Quaternion Rotation => __deltaObject__.Rotation;
        public SharedCode.Utils.Vector3 Scale => __deltaObject__.Scale;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = __SyncTransform;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -2029274143;
    }

    public class SimpleMovementSyncServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServerApi
    {
        public SimpleMovementSyncServerApi(SharedCode.MovementSync.ISimpleMovementSync deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.ISimpleMovementSync __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.ISimpleMovementSync)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1052392907;
    }

    public class SimpleMovementSyncServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISimpleMovementSyncServer
    {
        public SimpleMovementSyncServer(SharedCode.MovementSync.ISimpleMovementSync deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.ISimpleMovementSync __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.ISimpleMovementSync)__deltaObjectBase__;
            }
        }

        public System.Type GridSyncType => __deltaObject__.GridSyncType;
        public SharedCode.MovementSync.SimpleMovementStateEvent OnMovementStateChanged => __deltaObject__.OnMovementStateChanged;
        public bool VisibilityOff => __deltaObject__.VisibilityOff;
        public SharedCode.Entities.Transform __SyncTransform => __deltaObject__.__SyncTransform;
        public SharedCode.Entities.Transform Transform => __deltaObject__.Transform;
        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public SharedCode.Utils.Quaternion Rotation => __deltaObject__.Rotation;
        public SharedCode.Utils.Vector3 Scale => __deltaObject__.Scale;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = __SyncTransform;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1499085302;
    }
}