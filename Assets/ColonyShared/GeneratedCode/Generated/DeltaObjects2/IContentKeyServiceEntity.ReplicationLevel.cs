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
    public class ContentKeyServiceEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContentKeyServiceEntityAlways
    {
        public ContentKeyServiceEntityAlways(GeneratedCode.ContentKeys.IContentKeyServiceEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.ContentKeys.IContentKeyServiceEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObjectBase__;
            }
        }

        public IDeltaList<ResourceSystem.ContentKeys.ContentKeyDef> Keys => __deltaObject__.Keys;
        public System.Threading.Tasks.Task<string> EnableKey(string key)
        {
            return __deltaObject__.EnableKey(key);
        }

        public System.Threading.Tasks.Task<string> DisableKey(string key)
        {
            return __deltaObject__.DisableKey(key);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Keys;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1909395998;
    }

    public class ContentKeyServiceEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContentKeyServiceEntityClientBroadcast
    {
        public ContentKeyServiceEntityClientBroadcast(GeneratedCode.ContentKeys.IContentKeyServiceEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.ContentKeys.IContentKeyServiceEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObjectBase__;
            }
        }

        public IDeltaList<ResourceSystem.ContentKeys.ContentKeyDef> Keys => __deltaObject__.Keys;
        public System.Threading.Tasks.Task<string> EnableKey(string key)
        {
            return __deltaObject__.EnableKey(key);
        }

        public System.Threading.Tasks.Task<string> DisableKey(string key)
        {
            return __deltaObject__.DisableKey(key);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Keys;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1190986145;
    }

    public class ContentKeyServiceEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContentKeyServiceEntityClientFullApi
    {
        public ContentKeyServiceEntityClientFullApi(GeneratedCode.ContentKeys.IContentKeyServiceEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.ContentKeys.IContentKeyServiceEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -746297428;
    }

    public class ContentKeyServiceEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContentKeyServiceEntityClientFull
    {
        public ContentKeyServiceEntityClientFull(GeneratedCode.ContentKeys.IContentKeyServiceEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.ContentKeys.IContentKeyServiceEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObjectBase__;
            }
        }

        public IDeltaList<ResourceSystem.ContentKeys.ContentKeyDef> Keys => __deltaObject__.Keys;
        public System.Threading.Tasks.Task<string> EnableKey(string key)
        {
            return __deltaObject__.EnableKey(key);
        }

        public System.Threading.Tasks.Task<string> DisableKey(string key)
        {
            return __deltaObject__.DisableKey(key);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Keys;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1151587464;
    }

    public class ContentKeyServiceEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContentKeyServiceEntityServerApi
    {
        public ContentKeyServiceEntityServerApi(GeneratedCode.ContentKeys.IContentKeyServiceEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.ContentKeys.IContentKeyServiceEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1362360115;
    }

    public class ContentKeyServiceEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContentKeyServiceEntityServer
    {
        public ContentKeyServiceEntityServer(GeneratedCode.ContentKeys.IContentKeyServiceEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.ContentKeys.IContentKeyServiceEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObjectBase__;
            }
        }

        public IDeltaList<ResourceSystem.ContentKeys.ContentKeyDef> Keys => __deltaObject__.Keys;
        public System.Threading.Tasks.Task<string> EnableKey(string key)
        {
            return __deltaObject__.EnableKey(key);
        }

        public System.Threading.Tasks.Task<string> DisableKey(string key)
        {
            return __deltaObject__.DisableKey(key);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Keys;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1281925873;
    }
}