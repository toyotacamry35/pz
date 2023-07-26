using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities.Cloud
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface ILoadDataEntity : IEntity
    {
        [RemoteMethod]
        Task<byte[]> LoadEntity(int typeId, Guid entityId);
    }
}
