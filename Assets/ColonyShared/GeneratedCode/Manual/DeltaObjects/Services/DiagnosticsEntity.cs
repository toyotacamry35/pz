using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.Chain;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    public partial class DiagnosticsEntity : IHookOnInit
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const float TimeoutSeconds = 5;

        private const float ErrorDurationSeconds = 0.5f;

        private ChainCancellationToken _token;

        public Task OnInit()
        {
            _token = this.Chain().Delay(5, true).PingUnityThread().Run();
            return Task.CompletedTask;
        }

        public async Task PingUnityThreadImpl()
        {
            var sw = new Stopwatch();
            sw.Start();
            var unityTask = EntitytObjectsUnitySpawnService.SpawnService.RunInUnityThread(() =>
            {
                if (sw.Elapsed.TotalSeconds >= ErrorDurationSeconds)
                    Logger.IfError()?.Message("Unity thread inner ping too long {0} milliseconds", sw.Elapsed.TotalMilliseconds).Write();
                else
                    Logger.IfDebug()?.Message("Unity thread inner ping {0} milliseconds", sw.Elapsed.TotalMilliseconds).Write();
            });
            var delayTask = new SuspendingAwaitable(Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds)));
            await SuspendingAwaitable.WhenAny(new[] { unityTask, delayTask });
            sw.Stop();

            if (!unityTask.IsCompleted)
                Logger.IfError()?.Message("Unity thread ping timeout {0} seconds", TimeoutSeconds).Write();
            else if (sw.Elapsed.TotalSeconds >= ErrorDurationSeconds)
                Logger.IfError()?.Message("Unity thread ping too long {0} milliseconds", sw.Elapsed.TotalMilliseconds).Write();
            else
                Logger.IfDebug()?.Message("Unity thread ping {0} milliseconds", sw.Elapsed.TotalMilliseconds).Write();
        }
    }
}
