using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace GeneratedCode.EntityModel.Test
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IToucherTestDeltaObject : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int    IntProperty { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetIntProperty(int i);
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class ToucherTestDeltaObject
    {
        public Task SetIntPropertyImpl(int i)
        {
            this.IntProperty = i;
            return Task.CompletedTask;
        }
    }
}