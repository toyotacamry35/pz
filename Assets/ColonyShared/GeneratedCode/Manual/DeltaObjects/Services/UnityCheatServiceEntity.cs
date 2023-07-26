using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class UnityCheatServiceEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // @param `isOn` - pass false to stop repeated sleeps
        // @param `delayBeforeSleep` - delay before 1st sleep
        // @param `repeatTime` - pass 0 to do not repeat
        public Task MainUnityThreadOnServerSleepImpl(bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"2. MainUnityThreadOnServerSleepImpl({isOn}, {sleepTime}, {delayBeforeSleep}, {repeatTime})").Write();
            EntitytObjectsUnitySpawnService.SpawnService.MainUnityThreadOnServerSleep(isOn, sleepTime, delayBeforeSleep, repeatTime);
            return Task.CompletedTask;
        }

        public Task SetCurveLoggerStateImpl(bool enabledStatus, bool dump, string loggerName, Guid dumpId)
        {
            CheatServiceEntity.SetCurveLoggerStateDo(enabledStatus, dump, loggerName, dumpId, EntitiesRepository);
            return Task.CompletedTask;
        }

        public async Task<Transform> GetClosestPlayerSpawnPointTransformImpl(Vector3 pos)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"GetClosestPlayerSpawnPointTransform({pos})").Write();
            //if (DbgLog.Enabled) DbgLog.Log("4. GetClosestPlayerSpawnPointTransformImpl");
            return await EntitytObjectsUnitySpawnService.SpawnService.GetClosestPlayerSpawnPointTransform(EntitiesRepository, pos);
        }
    }
}
