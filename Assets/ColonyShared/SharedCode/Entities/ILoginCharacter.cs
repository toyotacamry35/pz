using System;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    //[CloudNode(CloudNodeType.Login)]
    public interface ILoginCharacter : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<bool> TestMethod1(int a, Guid itemId);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task TestMethodSimple();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task TestMethodSimpleParam(string param1);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<bool> TestMethod2222(int a, Guid itemId);
    }
}
