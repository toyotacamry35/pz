using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    [EntityService]
    public interface  IDiagnosticsEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task PingUnityThread();
    }
}
