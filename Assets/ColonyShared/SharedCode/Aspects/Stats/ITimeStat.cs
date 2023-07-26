using SharedCode.EntitySystem;
using Src.Aspects.Impl.Stats.Proxy;
using System.Threading.Tasks;
using GeneratorAnnotations;

namespace Src.Aspects.Impl.Stats
{
    [GenerateDeltaObjectCode]
    public interface ITimeStat : IStat, IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> ChangeValue(float delta);    
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] TimeStatState State { get; set; }
    }
}
