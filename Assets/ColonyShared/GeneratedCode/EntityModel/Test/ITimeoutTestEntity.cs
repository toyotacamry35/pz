using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.EntityModel.Test
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ITimeoutTestEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaList<int> TestDeltaListInt { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaList<ITestDeltaObject> TestDeltaListDeltaObject { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<int, int> TestDeltaDictionaryInt { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<int, ITestDeltaObject> TestDeltaDictionaryIntDeltaObject { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int TestProperty { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task LongUsage();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task ShortUsage();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetTestProperty(int value);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task AwaitWriteTimeSec(float seconds);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task AwaitWriteTimeSecAndSetTestProperty(float seconds, int value);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]

        Task AwaitReadTimeSec(float seconds);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task AwaitWriteTimeSecAndCallSubTestEntityRpcWithAwait(float seconds, float subseconds, int value);
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ITestDeltaObject : IDeltaObject
    {
        int Value { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ITimeoutSubTestEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaList<int> TestDeltaListInt { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaList<ITestDeltaObject> TestDeltaListDeltaObject { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<int, int> TestDeltaDictionaryInt { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<int, ITestDeltaObject> TestDeltaDictionaryIntDeltaObject { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int TestProperty { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task LongUsage();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task ShortUsage();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetTestProperty(int value);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task AwaitWriteTimeSec(float seconds);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task AwaitWriteTimeSecAndSetTestProperty(float seconds, int value);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task AwaitReadTimeSec(float seconds);
    }
}
