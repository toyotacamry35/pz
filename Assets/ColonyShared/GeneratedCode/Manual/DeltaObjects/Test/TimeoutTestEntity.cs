using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratedCode.EntityModel.Test;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutTestEntity
    {
        public async Task LongUsageImpl()
        {
            TestProperty = 10;
            await Task.Delay(TimeSpan.FromSeconds(15));
            TestProperty = 20;
        }

        public Task ShortUsageImpl()
        {
            return Task.CompletedTask;
        }

        public Task SetTestPropertyImpl(int value)
        {
            TestProperty = value;
            return Task.CompletedTask;
        }

        public async Task AwaitWriteTimeSecImpl(float seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        public async Task AwaitWriteTimeSecAndSetTestPropertyImpl(float seconds, int value)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            TestProperty = value;
        }

        public async Task AwaitReadTimeSecImpl(float seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        public async Task AwaitWriteTimeSecAndCallSubTestEntityRpcWithAwaitImpl(float seconds, float subseconds, int value)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            using (var wrapper = await EntitiesRepository.Get<ITimeoutSubTestEntity>(Id))
            {
                var entity = wrapper.Get<ITimeoutSubTestEntity>(Id);
                await entity.AwaitWriteTimeSecAndSetTestProperty(subseconds, value);
            }
        }
    }


    public partial class TimeoutSubTestEntity
    {
        public async Task LongUsageImpl()
        {
            TestProperty = 10;
            await Task.Delay(TimeSpan.FromSeconds(15));
            TestProperty = 20;
        }

        public Task ShortUsageImpl()
        {
            return Task.CompletedTask;
        }

        public Task SetTestPropertyImpl(int value)
        {
            TestProperty = value;
            return Task.CompletedTask;
        }

        public async Task AwaitWriteTimeSecImpl(float seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        public async Task AwaitWriteTimeSecAndSetTestPropertyImpl(float seconds, int value)
        {
            var dateTimeFinish = DateTime.UtcNow.AddSeconds(seconds);
            while (DateTime.UtcNow < dateTimeFinish)
                using (var wrapper = await EntitiesRepository.Get(TypeId, Id))
                {
                    var entity = wrapper.Get<IEntity>(TypeId, Id);
                    var z = entity;
                    await Task.Delay(10);
                }

            TestProperty = value;
        }

        public async Task AwaitReadTimeSecImpl(float seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }
    }
}
