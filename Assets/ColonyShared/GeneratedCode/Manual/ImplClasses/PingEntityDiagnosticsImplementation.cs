using System.Diagnostics;
using System.Threading.Tasks;
using SharedCode.Entities.GameObjectEntities;

namespace GeneratedCode.DeltaObjects
{
    public partial class PingDiagnostics : IPingDiagnosticsImplementRemoteMethods
    {
        public Task<bool> PingLocalImpl()
        {
            return Task.FromResult(true);
        }

        public Task<bool> PingReadImpl()
        {
            return Task.FromResult(true);
        }

        public Task<bool> PingWriteImpl()
        {
            return Task.FromResult(true);
        }

        public async Task<float> PingReadUnityThreadImpl()
        {
            var sw = new Stopwatch();
            sw.Start();
            await EntitytObjectsUnitySpawnService.SpawnService.RunInUnityThread(() => { });
            sw.Stop();
            var elapsedMs = (float)sw.Elapsed.TotalMilliseconds;
            return elapsedMs;
        }
    }
}
