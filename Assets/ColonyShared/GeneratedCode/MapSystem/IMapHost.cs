using GeneratedCode.Custom.Config;
using GeneratorAnnotations;
using SharedCode.Config;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedCode.Aspects.Sessions;

namespace SharedCode.MapSystem
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType:Cloud.CloudNodeType.Server | Cloud.CloudNodeType.Client)]
    public interface IMapHostEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        [RemoteMethod(30)]
        Task<HostOrInstallMapResult> HostMap(Guid id, Guid realmId, MapDef map, RealmRulesDef realmRules);

        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaList<MapDef> HostedMaps { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> LogoutUserFromMap(Guid userId, Guid map, bool terminal);


        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        ValueTask<bool> AddUsersDirect(List<Guid> users);
    }

    public enum HostOrInstallMapResult
    {
        None,
        Success,
        Error
    }

    public enum MapHostLoginResult
    {
        None,
        Success,
        Error
    }
}
