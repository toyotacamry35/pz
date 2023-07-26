using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities
{
    public interface IHasPingDiagnostics
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IPingDiagnostics PingDiagnostics { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IPingDiagnostics : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [RemoteMethod(60)]
        Task<bool> PingLocal();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [RemoteMethod(60)]
        Task<bool> PingRead();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [RemoteMethod(60)]
        Task<bool> PingWrite();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [RemoteMethod(60)]
        Task<float> PingReadUnityThread();
    }
}
