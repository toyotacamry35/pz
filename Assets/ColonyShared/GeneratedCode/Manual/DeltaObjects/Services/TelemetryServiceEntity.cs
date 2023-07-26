using NLog;
using SharedCode.Entities.Telemetry;
using SharedCode.Serializers;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class TelemetryServiceEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<TelemetryEventResult> IndexEventImpl(TelemetryEvent telemetryEvent)
        {
            Logger.IfError()?.Message($"TelemetryServiceEntity: type: {telemetryEvent.GetType()}").Write();
            AsyncUtils.RunAsyncTask(async () =>
            {
                telemetryEvent.Index();
            }, EntitiesRepository);
            return new TelemetryEventResult() { Type = telemetryEvent.Type, Result = TelemetryEventErrorCode.Success }; ;
        }
    }
}
