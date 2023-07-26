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
    public class LifespanAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILifespanAlways
    {
        public LifespanAlways(Assets.ColonyShared.SharedCode.Entities.ILifespan deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.ILifespan __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1952656451;
    }

    public class LifespanClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILifespanClientBroadcast
    {
        public LifespanClientBroadcast(Assets.ColonyShared.SharedCode.Entities.ILifespan deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.ILifespan __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObjectBase__;
            }
        }

        public float LifespanSec => __deltaObject__.LifespanSec;
        public bool IsLifespanExpired => __deltaObject__.IsLifespanExpired;
        public int LifespanCycleNumber => __deltaObject__.LifespanCycleNumber;
        public long BirthTime => __deltaObject__.BirthTime;
        public System.Threading.Tasks.Task<float> GetExpiredLifespanPercent()
        {
            return __deltaObject__.GetExpiredLifespanPercent();
        }

        public System.Threading.Tasks.Task<bool> IsExpiredLifespanPercentInRange(float fromIncluding, float tillExcluding)
        {
            return __deltaObject__.IsExpiredLifespanPercentInRange(fromIncluding, tillExcluding);
        }

        public event System.Func<System.Guid, int, Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired, System.Threading.Tasks.Task> LifespanExpiredEvent
        {
            add
            {
                __deltaObject__.LifespanExpiredEvent += value;
            }

            remove
            {
                __deltaObject__.LifespanExpiredEvent -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 12:
                    currProperty = LifespanSec;
                    break;
                case 13:
                    currProperty = IsLifespanExpired;
                    break;
                case 14:
                    currProperty = LifespanCycleNumber;
                    break;
                case 15:
                    currProperty = BirthTime;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 773329817;
    }

    public class LifespanClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILifespanClientFullApi
    {
        public LifespanClientFullApi(Assets.ColonyShared.SharedCode.Entities.ILifespan deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.ILifespan __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObjectBase__;
            }
        }

        public override int TypeId => 568181144;
    }

    public class LifespanClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILifespanClientFull
    {
        public LifespanClientFull(Assets.ColonyShared.SharedCode.Entities.ILifespan deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.ILifespan __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObjectBase__;
            }
        }

        public float LifespanSec => __deltaObject__.LifespanSec;
        public bool IsLifespanExpired => __deltaObject__.IsLifespanExpired;
        public int LifespanCycleNumber => __deltaObject__.LifespanCycleNumber;
        public long BirthTime => __deltaObject__.BirthTime;
        public System.Threading.Tasks.Task<float> GetExpiredLifespanPercent()
        {
            return __deltaObject__.GetExpiredLifespanPercent();
        }

        public System.Threading.Tasks.Task<bool> IsExpiredLifespanPercentInRange(float fromIncluding, float tillExcluding)
        {
            return __deltaObject__.IsExpiredLifespanPercentInRange(fromIncluding, tillExcluding);
        }

        public event System.Func<System.Guid, int, Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired, System.Threading.Tasks.Task> LifespanExpiredEvent
        {
            add
            {
                __deltaObject__.LifespanExpiredEvent += value;
            }

            remove
            {
                __deltaObject__.LifespanExpiredEvent -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 12:
                    currProperty = LifespanSec;
                    break;
                case 13:
                    currProperty = IsLifespanExpired;
                    break;
                case 14:
                    currProperty = LifespanCycleNumber;
                    break;
                case 15:
                    currProperty = BirthTime;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1555168575;
    }

    public class LifespanServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILifespanServerApi
    {
        public LifespanServerApi(Assets.ColonyShared.SharedCode.Entities.ILifespan deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.ILifespan __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObjectBase__;
            }
        }

        public override int TypeId => -835480554;
    }

    public class LifespanServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILifespanServer
    {
        public LifespanServer(Assets.ColonyShared.SharedCode.Entities.ILifespan deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.ILifespan __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObjectBase__;
            }
        }

        public Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired DoOnExpired => __deltaObject__.DoOnExpired;
        public float LifespanSec => __deltaObject__.LifespanSec;
        public bool IsLifespanExpired => __deltaObject__.IsLifespanExpired;
        public int LifespanCycleNumber => __deltaObject__.LifespanCycleNumber;
        public long BirthTime => __deltaObject__.BirthTime;
        public System.Threading.Tasks.Task<bool> InvokeLifespanExpiredEvent(Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired whatToDo)
        {
            return __deltaObject__.InvokeLifespanExpiredEvent(whatToDo);
        }

        public System.Threading.Tasks.Task<bool> CancelLifespanCountdown()
        {
            return __deltaObject__.CancelLifespanCountdown();
        }

        public System.Threading.Tasks.Task StartLifespanCountdown()
        {
            return __deltaObject__.StartLifespanCountdown();
        }

        public System.Threading.Tasks.Task<float> GetExpiredLifespanPercent()
        {
            return __deltaObject__.GetExpiredLifespanPercent();
        }

        public System.Threading.Tasks.Task<bool> IsExpiredLifespanPercentInRange(float fromIncluding, float tillExcluding)
        {
            return __deltaObject__.IsExpiredLifespanPercentInRange(fromIncluding, tillExcluding);
        }

        public event System.Func<System.Guid, int, Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired, System.Threading.Tasks.Task> LifespanExpiredEvent
        {
            add
            {
                __deltaObject__.LifespanExpiredEvent += value;
            }

            remove
            {
                __deltaObject__.LifespanExpiredEvent -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = DoOnExpired;
                    break;
                case 12:
                    currProperty = LifespanSec;
                    break;
                case 13:
                    currProperty = IsLifespanExpired;
                    break;
                case 14:
                    currProperty = LifespanCycleNumber;
                    break;
                case 15:
                    currProperty = BirthTime;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 378271854;
    }
}