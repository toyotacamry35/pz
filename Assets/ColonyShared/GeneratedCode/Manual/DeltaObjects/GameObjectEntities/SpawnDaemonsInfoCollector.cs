using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.Chain;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class SpawnDaemonsInfoCollector : IHookOnInit, IHookOnDestroy
    {
        public Task<bool> UpdateImpl()
        {
            SpawnDaemonsInfo.OnUpdate();
            return Task.FromResult(true);
        }

        public Task OnInit()
        {
            SpawnDaemonsInfo.AcceptLogs = true;
            this.Chain().Delay(10, true).Update().Run();
            return Task.CompletedTask;
        }

        public Task OnDestroy()
        {
            SpawnDaemonsInfo.AcceptLogs = false;
            return Task.CompletedTask;
        }
    }
}
