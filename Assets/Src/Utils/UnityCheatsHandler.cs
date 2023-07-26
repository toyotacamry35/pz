using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;
using Assets.Src.Lib.Extensions;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntityModel.Bots;
using SharedCode.EntitySystem;

using ShrdVector3 = SharedCode.Utils.Vector3;
using Transform = SharedCode.Entities.Transform;

namespace Assets.Src.Utils
{
    /// <summary>
    /// Type to encapsulate cheats logic out from `UnitySpawnService`.
    /// </summary>
    internal class UnityCheatsHandler : IUnityCheatsHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // --- API: ---------------------------------------------------

        public async void MainUnityThreadOnServerSleep(bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime)
        {
            if (repeatTime > 0 && repeatTime <= sleepTime)
            {
                Logger.IfError()?.Message($"`{nameof(repeatTime)}` should be > `{nameof(sleepTime)}`").Write();
                return;
            }

            if (!isOn)
            {
                if (_stopTokenSource != null)
                {
                    _stopTokenSource.Cancel();
                    Logger.IfInfo()?.Message/*DbgLog.Log*/($"Cancellation of {nameof(UnityThredSleepRoutine)} requested.").Write();
                }
                else
                    Logger.IfError()?.Message($"There is no active {nameof(UnityThredSleepRoutine)}").Write();

                return;
            }

            try
            {
                if (delayBeforeSleep > 0)
                    await Task.Delay(SecToMSec(delayBeforeSleep));

                if (repeatTime > 0)
                    await UnityThredSleepRoutine(sleepTime, repeatTime);
                else
                    UnityThredSleepDo(sleepTime);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message("##Got exception: " + e).Write();
                throw;
            }
        }

        private static SpawnPointTypeDef _botSpawnPointTypeDef;
        public async Task<Transform> GetClosestPlayerSpawnPointTransform(IEntitiesRepository repo, ShrdVector3 pos)
        {
            Logger.IfInfo()?.Message($"GetClosestPlayerSpawnPointTransform({pos})").Write();

            if (_botSpawnPointTypeDef == null)
            { 
                // AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
                // {
                    using (var wrapperBots = await repo.GetFirstService<IBotCoordinator>())
                    {
                        var botCoordReplica = wrapperBots?.GetFirstService<IBotCoordinatorServer>();
                        if (botCoordReplica != null)
                            _botSpawnPointTypeDef = botCoordReplica.BotSpawnPointTypeDef;
                        else
                        {
                            Logger.IfError()?.Message($"Can't get  {nameof(IBotCoordinatorServer)} at {repo.Id}").Write();
                            return default;
                        }
                    }
                // },
                // repo).WrapErrors();
            }

            return await UnityQueueHelper.RunInUnityThread(() =>
            {
                if(false)
                {///#Dbg:
                    var dbg = UnityEngine.Object.FindObjectsOfType<PlayerSpawnPoint>();
                    var msg = new StringBuilder($"_botSpawnPointTypeDef: {_botSpawnPointTypeDef?.ToString() ?? "null"}, ");
                    foreach (var e in dbg)
                        msg.Append($"{e.Name ?? "null"}: {e.SpawnPointType?.ToString() ?? "null"}({e.SpawnPointType != _botSpawnPointTypeDef}) / {e.PointTypeMetaData.Get<SpawnPointTypeDef>()?.ToString() ?? "null"}({e.PointTypeMetaData.Get<SpawnPointTypeDef>() != _botSpawnPointTypeDef})");
                    if (DbgLog.Enabled) DbgLog.Log(msg.ToString());
                }

                //#TODO: opt.by k-d tree (or smthng like) 
                var point = UnityEngine.Object.FindObjectsOfType<PlayerSpawnPoint>()
                    ?.Where(p => p.PointTypeMetaData.Get<SpawnPointTypeDef>() != _botSpawnPointTypeDef)
                    ?.OrderBy(p => Vector3.Distance(p.transform.position,  pos.ToUnityVector3())).FirstOrDefault();
                if (point == null)
                {
                     Logger.IfError()?.Message("Can't find appropriate spawn point.").Write();;
                    return default;
                }

                // ReSharper disable once PossibleNullReferenceException
                return new Transform(point.transform.position.ToShared(), point.transform.rotation.ToSharedQuaternion());
            });
        }

        // --- Privates: ---------------------------------------------------------------------

        private static CancellationTokenSource _stopTokenSource;
        static async Task UnityThredSleepRoutine(float sleepTime, float repeatTime)
        {
            if (_stopTokenSource != null)
            {
                Logger.IfError()?.Message($"Previous {nameof(UnityThredSleepRoutine)} should be stopped before start new one.").Write();
                return;
            }
            _stopTokenSource = new CancellationTokenSource();

            Logger.IfInfo()?.Message/*DbgLog.Log*/("UnityThredSleepRoutine started.").Write();

            while (!_stopTokenSource.IsCancellationRequested)
            {
                UnityThredSleepDo(sleepTime);
                await Task.Delay(SecToMSec(repeatTime));
            }
            Logger.IfInfo()?.Message/*DbgLog.Log*/("UnityThredSleepRoutine stopped.").Write();

            _stopTokenSource.Dispose();
            _stopTokenSource = null;
        }
        static void UnityThredSleepDo(float sleepTime)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() => Thread.Sleep(SecToMSec(sleepTime)));
        }
        static int SecToMSec(float sec) => (int)(sec * 1000);

    }
}
