using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities.Test;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Manual.Repositories;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class ChainCallTestEntity
    {
        public Task<int> GetValueImpl()
        {
            return Task.FromResult(Value);
        }

        public Task<int> SetValueImpl(int value)
        {
            Value = value;
            return Task.FromResult(Value);
        }

        public Task<int> AddToValueImpl(int add, bool stop)
        {
            if (!stop)
                Value += add;
            return Task.FromResult(Value);
        }

        public Task<int> MulValueImpl(int mul, bool stop)
        {
            if (!stop)
                Value *= mul;
            return Task.FromResult(Value);
        }

        public Task<bool> CheckValueGreaterImpl(int value)
        {
            return Task.FromResult(Value > value);
        }

        public Task AppendValueImpl(bool stop)
        {
            if (!stop && StringBuilder != null)
                StringBuilder.Append(Value).Append(",");
            return Task.CompletedTask;
        }

        public async Task<bool> SetValueAndWaitImpl(int value, float seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            Value = value;
            return true;
        }

        public async Task<bool> SetValueAndWaitAndCallAnotherPeriodicImpl(int value, float seconds, Guid anotherTestEntity)
        {
            var endTimeTime = DateTime.UtcNow.AddSeconds(seconds);
            int i = 0;
            while (DateTime.UtcNow <= endTimeTime)
            {
                using (var wrapper = await EntitiesRepository.Get<IChainCallTestEntityServer>(anotherTestEntity))
                {
                    var entity = wrapper.Get<IChainCallTestEntityServer>(anotherTestEntity);
                    await entity.SetValue(value + i++);
                }

                await Task.Delay(100);
            }
            Value = value;
            return true;
        }
        public async Task<bool> MigrationCircleRpcCallImpl(int value, float seconds, Guid anotherTestEntity)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            using (var wrapper = await EntitiesRepository.Get<IChainCallTestEntity>(anotherTestEntity))
            {
                var entity = wrapper.Get<IChainCallTestEntityClientFull>(anotherTestEntity);
                var remoteValue = await entity.MigrationCircleRpcCallBack(seconds, this.Id);
                Value = value + remoteValue;
            }
            return true;
        }

        public async Task<int> MigrationCircleRpcCallBackImpl(float seconds, Guid anotherTestEntity)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            using (var wrapper = await EntitiesRepository.Get<IChainCallTestEntity>(anotherTestEntity))
            {
                var entity = wrapper.Get<IChainCallTestEntityClientFull>(anotherTestEntity);
                var remoteValue = await entity.GetValue();
                return Value + remoteValue;
            }
        }
    }
}
