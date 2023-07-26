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
    public class LogableEntityAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILogableEntityAlways
    {
        public LogableEntityAlways(SharedCode.Aspects.Utils.ILogableEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Aspects.Utils.ILogableEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Aspects.Utils.ILogableEntity)__deltaObjectBase__;
            }
        }

        public bool IsCurveLoggerEnable => __deltaObject__.IsCurveLoggerEnable;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = IsCurveLoggerEnable;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 424571219;
    }

    public class LogableEntityClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILogableEntityClientBroadcast
    {
        public LogableEntityClientBroadcast(SharedCode.Aspects.Utils.ILogableEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Aspects.Utils.ILogableEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Aspects.Utils.ILogableEntity)__deltaObjectBase__;
            }
        }

        public bool IsCurveLoggerEnable => __deltaObject__.IsCurveLoggerEnable;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = IsCurveLoggerEnable;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1365068646;
    }

    public class LogableEntityClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILogableEntityClientFullApi
    {
        public LogableEntityClientFullApi(SharedCode.Aspects.Utils.ILogableEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Aspects.Utils.ILogableEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Aspects.Utils.ILogableEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1680171454;
    }

    public class LogableEntityClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILogableEntityClientFull
    {
        public LogableEntityClientFull(SharedCode.Aspects.Utils.ILogableEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Aspects.Utils.ILogableEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Aspects.Utils.ILogableEntity)__deltaObjectBase__;
            }
        }

        public bool IsCurveLoggerEnable => __deltaObject__.IsCurveLoggerEnable;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = IsCurveLoggerEnable;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1914182013;
    }

    public class LogableEntityServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILogableEntityServerApi
    {
        public LogableEntityServerApi(SharedCode.Aspects.Utils.ILogableEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Aspects.Utils.ILogableEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Aspects.Utils.ILogableEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -585253586;
    }

    public class LogableEntityServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ILogableEntityServer
    {
        public LogableEntityServer(SharedCode.Aspects.Utils.ILogableEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Aspects.Utils.ILogableEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Aspects.Utils.ILogableEntity)__deltaObjectBase__;
            }
        }

        public bool IsCurveLoggerEnable => __deltaObject__.IsCurveLoggerEnable;
        public System.Threading.Tasks.Task SetCurveLoggerEnable(bool val)
        {
            return __deltaObject__.SetCurveLoggerEnable(val);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = IsCurveLoggerEnable;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 2021143679;
    }
}