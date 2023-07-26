using System;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class TestEntity
    {
        public async Task TestUnlimitedTimeUsageImpl()
        {
            await Task.Delay(TimeSpan.FromSeconds(20));
        }

        public Task TestEmptyCallImpl() => Task.CompletedTask;

        public async Task VeryLongCallImpl()
        {
            await Task.Delay(TimeSpan.FromSeconds(15));
        }
    }
}