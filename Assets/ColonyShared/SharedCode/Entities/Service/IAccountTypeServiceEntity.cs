using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server, addedByDefaultToNodeType: CloudNodeType.None)]
    public interface IAccountTypeServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaDictionary<Guid, long> AccountTypes { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task<long> GetAccountType(Guid userId);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetAccountType(Guid userId, long accountType);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task RemoveAccountType(Guid userId);
    }

    [Flags]
    public enum AccountType: ulong
    {
        None = 0,
        User = 1,
        GameMaster = 2,
        TechnicalSupport = 4,
        Developer = 8,
        Everything = User | GameMaster | TechnicalSupport | Developer
    }
}
