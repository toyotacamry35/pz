using System;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.Src.ResourcesSystem.Base;
using GeneratorAnnotations;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Entities.Test
{
    [GenerateDeltaObjectCode]
    public interface ITestEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Master)] Task TestUnlimitedTimeUsage();

        [ReplicationLevel(ReplicationLevel.Master)] Task TestEmptyCall();

        [ReplicationLevel(ReplicationLevel.Master)] Task VeryLongCall();
    }

    [GenerateDeltaObjectCode]
    public interface IChainCallTestEntity : IEntity
    {
        [RuntimeData]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        StringBuilder StringBuilder { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int Value { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<int> GetValue();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<int> SetValue(int value);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<int> AddToValue(int add, bool stop);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<int> MulValue(int mul, bool stop);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<bool> CheckValueGreater(int value);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task AppendValue(bool stop);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<bool> SetValueAndWait(int value, float seconds);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<bool> SetValueAndWaitAndCallAnotherPeriodic(int value, float seconds, Guid anotherTestEntity);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<bool> MigrationCircleRpcCall(int value, float seconds, Guid anotherTestEntity);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<int> MigrationCircleRpcCallBack(float seconds, Guid anotherTestEntity);
    }
}
