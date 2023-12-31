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
    public class TestEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ITestEntityAlways
    {
        public TestEntityAlways(Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1793715988;
    }

    public class TestEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ITestEntityClientBroadcast
    {
        public TestEntityClientBroadcast(Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -479947687;
    }

    public class TestEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ITestEntityClientFullApi
    {
        public TestEntityClientFullApi(Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -2107457697;
    }

    public class TestEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ITestEntityClientFull
    {
        public TestEntityClientFull(Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1754249020;
    }

    public class TestEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ITestEntityServerApi
    {
        public TestEntityServerApi(Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1862484834;
    }

    public class TestEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ITestEntityServer
    {
        public TestEntityServer(Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.ITestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1340200965;
    }
}

namespace GeneratedCode.DeltaObjects
{
    public class ChainCallTestEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallTestEntityAlways
    {
        public ChainCallTestEntityAlways(Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1543524335;
    }

    public class ChainCallTestEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallTestEntityClientBroadcast
    {
        public ChainCallTestEntityClientBroadcast(Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity)__deltaObjectBase__;
            }
        }

        public System.Text.StringBuilder StringBuilder => __deltaObject__.StringBuilder;
        public int Value => __deltaObject__.Value;
        public System.Threading.Tasks.Task<int> GetValue()
        {
            return __deltaObject__.GetValue();
        }

        public System.Threading.Tasks.Task<int> SetValue(int value)
        {
            return __deltaObject__.SetValue(value);
        }

        public System.Threading.Tasks.Task<int> AddToValue(int add, bool stop)
        {
            return __deltaObject__.AddToValue(add, stop);
        }

        public System.Threading.Tasks.Task<int> MulValue(int mul, bool stop)
        {
            return __deltaObject__.MulValue(mul, stop);
        }

        public System.Threading.Tasks.Task<bool> CheckValueGreater(int value)
        {
            return __deltaObject__.CheckValueGreater(value);
        }

        public System.Threading.Tasks.Task<bool> SetValueAndWait(int value, float seconds)
        {
            return __deltaObject__.SetValueAndWait(value, seconds);
        }

        public System.Threading.Tasks.Task<bool> SetValueAndWaitAndCallAnotherPeriodic(int value, float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.SetValueAndWaitAndCallAnotherPeriodic(value, seconds, anotherTestEntity);
        }

        public System.Threading.Tasks.Task<bool> MigrationCircleRpcCall(int value, float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.MigrationCircleRpcCall(value, seconds, anotherTestEntity);
        }

        public System.Threading.Tasks.Task<int> MigrationCircleRpcCallBack(float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.MigrationCircleRpcCallBack(seconds, anotherTestEntity);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Value;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -2054918302;
    }

    public class ChainCallTestEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallTestEntityClientFullApi
    {
        public ChainCallTestEntityClientFullApi(Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1481753574;
    }

    public class ChainCallTestEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallTestEntityClientFull
    {
        public ChainCallTestEntityClientFull(Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity)__deltaObjectBase__;
            }
        }

        public System.Text.StringBuilder StringBuilder => __deltaObject__.StringBuilder;
        public int Value => __deltaObject__.Value;
        public System.Threading.Tasks.Task<int> GetValue()
        {
            return __deltaObject__.GetValue();
        }

        public System.Threading.Tasks.Task<int> SetValue(int value)
        {
            return __deltaObject__.SetValue(value);
        }

        public System.Threading.Tasks.Task<int> AddToValue(int add, bool stop)
        {
            return __deltaObject__.AddToValue(add, stop);
        }

        public System.Threading.Tasks.Task<int> MulValue(int mul, bool stop)
        {
            return __deltaObject__.MulValue(mul, stop);
        }

        public System.Threading.Tasks.Task<bool> CheckValueGreater(int value)
        {
            return __deltaObject__.CheckValueGreater(value);
        }

        public System.Threading.Tasks.Task<bool> SetValueAndWait(int value, float seconds)
        {
            return __deltaObject__.SetValueAndWait(value, seconds);
        }

        public System.Threading.Tasks.Task<bool> SetValueAndWaitAndCallAnotherPeriodic(int value, float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.SetValueAndWaitAndCallAnotherPeriodic(value, seconds, anotherTestEntity);
        }

        public System.Threading.Tasks.Task<bool> MigrationCircleRpcCall(int value, float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.MigrationCircleRpcCall(value, seconds, anotherTestEntity);
        }

        public System.Threading.Tasks.Task<int> MigrationCircleRpcCallBack(float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.MigrationCircleRpcCallBack(seconds, anotherTestEntity);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Value;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1213030780;
    }

    public class ChainCallTestEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallTestEntityServerApi
    {
        public ChainCallTestEntityServerApi(Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -585736799;
    }

    public class ChainCallTestEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallTestEntityServer
    {
        public ChainCallTestEntityServer(Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.Test.IChainCallTestEntity)__deltaObjectBase__;
            }
        }

        public System.Text.StringBuilder StringBuilder => __deltaObject__.StringBuilder;
        public int Value => __deltaObject__.Value;
        public System.Threading.Tasks.Task<int> GetValue()
        {
            return __deltaObject__.GetValue();
        }

        public System.Threading.Tasks.Task<int> SetValue(int value)
        {
            return __deltaObject__.SetValue(value);
        }

        public System.Threading.Tasks.Task<int> AddToValue(int add, bool stop)
        {
            return __deltaObject__.AddToValue(add, stop);
        }

        public System.Threading.Tasks.Task<int> MulValue(int mul, bool stop)
        {
            return __deltaObject__.MulValue(mul, stop);
        }

        public System.Threading.Tasks.Task<bool> CheckValueGreater(int value)
        {
            return __deltaObject__.CheckValueGreater(value);
        }

        public System.Threading.Tasks.Task<bool> SetValueAndWait(int value, float seconds)
        {
            return __deltaObject__.SetValueAndWait(value, seconds);
        }

        public System.Threading.Tasks.Task<bool> SetValueAndWaitAndCallAnotherPeriodic(int value, float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.SetValueAndWaitAndCallAnotherPeriodic(value, seconds, anotherTestEntity);
        }

        public System.Threading.Tasks.Task<bool> MigrationCircleRpcCall(int value, float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.MigrationCircleRpcCall(value, seconds, anotherTestEntity);
        }

        public System.Threading.Tasks.Task<int> MigrationCircleRpcCallBack(float seconds, System.Guid anotherTestEntity)
        {
            return __deltaObject__.MigrationCircleRpcCallBack(seconds, anotherTestEntity);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Value;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -637100532;
    }
}