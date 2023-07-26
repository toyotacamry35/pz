using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using SharedCode.Config;
using SharedCode.EntitySystem;
using SharedCode.Utils.HttpServer;

namespace GeneratedCode.DeltaObjects
{
    public partial class RestApiServiceEntity : IHasLoadFromJObject, IHookOnDestroy
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private RestApiServiceEntityConfig _config;

        private CancellationTokenSource _cts = null;
        private Task _serverTask = Task.CompletedTask;

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _config = (RestApiServiceEntityConfig)config;
            _cts = new CancellationTokenSource();

            if (HttpServerFactory.CreateDelegate != null)
            {
                var httpServer = HttpServerFactory.CreateServer(entitiesRepository);
                _serverTask = Log(httpServer.StartAsync(_config, _cts.Token));
            }
            return Task.CompletedTask;
        }

        private static async Task Log(Task task)
        {
            try
            {
                await task;
            }
            catch(OperationCanceledException)
            {
            }
            catch(Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
                throw;
            }
        }

        public Task OnDestroy()
        {
            _cts.Cancel();
            return _serverTask;
        }
    }
}
