using System;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.GeneratedCode.Time;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    public partial class InGameTimeServiceEntity : IHookOnInit, IHookOnReplicationLevelChanged
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public Task OnInit()
        {
            ServerStartTime = DateTime.UtcNow;
            ServerStartupIngameTime = DateTime.Today + InGameTime.StartupHours;
            IngameTimeDayDuration = InGameTime.DayDuration;
            return Task.CompletedTask;
        }
        public async Task<bool> SetTimeFromRealmImpl(DateTime serverStartTime)
        {
            ServerStartTime = serverStartTime;
            return true;
        }
        public Task<DateTime> GetCurrentIngameTimeImpl()
        {
            var timeIngame = InGameTime.GetCurrentIngameTime(ServerStartTime, ServerStartupIngameTime, IngameTimeDayDuration);

            return Task.FromResult(timeIngame);
        }
        public Task<bool> SetCurrentIngameTimeImpl(DateTime time)
        {
            var elapsed = ServerStartTime - new DateTime(SyncTime.Now * 10000);
            var elapsedIngame = time - ServerStartupIngameTime;
            var elapsedRealtime = (float)elapsedIngame.Ticks / (float)IngameTimeDayDuration.Ticks;
            var elapsedDelta = (long)elapsedRealtime - elapsed.Ticks;
            var serverStartShouldBe = new DateTime(ServerStartTime.Ticks + elapsedDelta);
            ServerStartTime = serverStartShouldBe;
            return Task.FromResult(true);
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            AsyncUtils.RunAsyncTask(() => OnReplicationLevelChangedAsync(oldReplicationMask, newReplicationMask), EntitiesRepository);
        }

        private async Task OnReplicationLevelChangedAsync(long oldReplicationMask, long newReplicationMask)
        {
            if (IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast))
            {
                using (var wrapper = await EntitiesRepository.GetFirstService<IInGameTimeServiceEntityClientBroadcast>())
                {
                    var inGameTimeEntity = wrapper.GetFirstService<IInGameTimeServiceEntityClientBroadcast>();

                    if (inGameTimeEntity == null)
                    {
                        Logger.IfError()?.Message("!inGameTimeEntity (repo.Id == " + EntitiesRepository.Id).Write();
                        return;
                    }

                    RegionTime.TimeParams = new RegionTime.TimeParamsData(inGameTimeEntity.ServerStartTime, inGameTimeEntity.ServerStartupIngameTime, inGameTimeEntity.IngameTimeDayDuration);
                }
            }
            if (IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast))
            {
                RegionTime.TimeParams = RegionTime.TimeParamsData.Empty;
            }
        }
    }
}
