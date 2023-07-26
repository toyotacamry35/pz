using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasDestroyable
    {
        [ReplicationLevel(ReplicationLevel.Server)] IDestroyable Destroyable { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> Destroy();

    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IDestroyable : IDeltaObject
    {
        // Can't be implemented in general way cause inner ".Chain()" need for custom implementation
        //  Implement like: 
        //    return Task.FromResult(this.Chain().Delay(delay).Destroy().Run());
        [ReplicationLevel(ReplicationLevel.Server)] Task<ChainCancellationToken> DestroyAfterDelay(float delay);
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> Destroy();
    }
}
