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
    public class AccountStatsAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountStatsAlways
    {
        public AccountStatsAlways(SharedCode.Entities.IAccountStats deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountStats __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountStats)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.ValueTask<bool> SetAccountStats(SharedCode.Entities.AccountStatsData value)
        {
            return __deltaObject__.SetAccountStats(value);
        }

        public override int TypeId => 1753561026;
    }

    public class AccountStatsClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountStatsClientBroadcast
    {
        public AccountStatsClientBroadcast(SharedCode.Entities.IAccountStats deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountStats __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountStats)__deltaObjectBase__;
            }
        }

        public int AccountExperience => __deltaObject__.AccountExperience;
        public System.Threading.Tasks.ValueTask<bool> SetAccountStats(SharedCode.Entities.AccountStatsData value)
        {
            return __deltaObject__.SetAccountStats(value);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = AccountExperience;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 426445716;
    }

    public class AccountStatsClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountStatsClientFullApi
    {
        public AccountStatsClientFullApi(SharedCode.Entities.IAccountStats deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountStats __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountStats)__deltaObjectBase__;
            }
        }

        public override int TypeId => -976111048;
    }

    public class AccountStatsClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountStatsClientFull
    {
        public AccountStatsClientFull(SharedCode.Entities.IAccountStats deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountStats __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountStats)__deltaObjectBase__;
            }
        }

        public int AccountExperience => __deltaObject__.AccountExperience;
        public ResourceSystem.Aspects.Misc.GenderDef Gender => __deltaObject__.Gender;
        public System.Threading.Tasks.ValueTask<bool> SetAccountStats(SharedCode.Entities.AccountStatsData value)
        {
            return __deltaObject__.SetAccountStats(value);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = AccountExperience;
                    break;
                case 11:
                    currProperty = Gender;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 2070719798;
    }

    public class AccountStatsServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountStatsServerApi
    {
        public AccountStatsServerApi(SharedCode.Entities.IAccountStats deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountStats __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountStats)__deltaObjectBase__;
            }
        }

        public override int TypeId => 120024463;
    }

    public class AccountStatsServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountStatsServer
    {
        public AccountStatsServer(SharedCode.Entities.IAccountStats deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountStats __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountStats)__deltaObjectBase__;
            }
        }

        public int AccountExperience => __deltaObject__.AccountExperience;
        public ResourceSystem.Aspects.Misc.GenderDef Gender => __deltaObject__.Gender;
        public System.Threading.Tasks.ValueTask<bool> SetAccountStats(SharedCode.Entities.AccountStatsData value)
        {
            return __deltaObject__.SetAccountStats(value);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = AccountExperience;
                    break;
                case 11:
                    currProperty = Gender;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1261422762;
    }
}