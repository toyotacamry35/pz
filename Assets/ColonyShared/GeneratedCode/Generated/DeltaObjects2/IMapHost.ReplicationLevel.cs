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
    public class MapHostEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMapHostEntityAlways
    {
        public MapHostEntityAlways(SharedCode.MapSystem.IMapHostEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MapSystem.IMapHostEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MapSystem.IMapHostEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 268414715;
    }

    public class MapHostEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMapHostEntityClientBroadcast
    {
        public MapHostEntityClientBroadcast(SharedCode.MapSystem.IMapHostEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MapSystem.IMapHostEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MapSystem.IMapHostEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1540743038;
    }

    public class MapHostEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMapHostEntityClientFullApi
    {
        public MapHostEntityClientFullApi(SharedCode.MapSystem.IMapHostEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MapSystem.IMapHostEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MapSystem.IMapHostEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1661451804;
    }

    public class MapHostEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMapHostEntityClientFull
    {
        public MapHostEntityClientFull(SharedCode.MapSystem.IMapHostEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MapSystem.IMapHostEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MapSystem.IMapHostEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1800154469;
    }

    public class MapHostEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMapHostEntityServerApi
    {
        public MapHostEntityServerApi(SharedCode.MapSystem.IMapHostEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MapSystem.IMapHostEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MapSystem.IMapHostEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 103820825;
    }

    public class MapHostEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMapHostEntityServer
    {
        public MapHostEntityServer(SharedCode.MapSystem.IMapHostEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MapSystem.IMapHostEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MapSystem.IMapHostEntity)__deltaObjectBase__;
            }
        }

        public IDeltaList<GeneratedCode.Custom.Config.MapDef> HostedMaps => __deltaObject__.HostedMaps;
        public System.Threading.Tasks.Task<SharedCode.MapSystem.HostOrInstallMapResult> HostMap(System.Guid id, System.Guid realmId, GeneratedCode.Custom.Config.MapDef map, SharedCode.Aspects.Sessions.RealmRulesDef realmRules)
        {
            return __deltaObject__.HostMap(id, realmId, map, realmRules);
        }

        public System.Threading.Tasks.Task<bool> LogoutUserFromMap(System.Guid userId, System.Guid map, bool terminal)
        {
            return __deltaObject__.LogoutUserFromMap(userId, map, terminal);
        }

        public System.Threading.Tasks.ValueTask<bool> AddUsersDirect(System.Collections.Generic.List<System.Guid> users)
        {
            return __deltaObject__.AddUsersDirect(users);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = HostedMaps;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1730275270;
    }
}