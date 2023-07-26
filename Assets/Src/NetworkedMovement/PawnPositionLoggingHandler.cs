using System;
using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Locomotion.Debug;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils;
using TimeUnits = System.Int64;

namespace Assets.Src.NetworkedMovement
{
    /// <summary>
    /// Just encapsulating out from `Pawn` class all pos.logging stuff
    /// </summary>
    internal class PawnPositionLoggingHandler : ILocoCurveLoggerProvider
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(PawnPositionLoggingHandler));

        private Guid EntityId => _pawn.EntityId;

        // To be able normalize frame ids by this one. Should be reseted to default every start logging locomotion data.
        private long _baseFrameId;
        private Pawn _pawn;

        // --- Ctor: -------------------------------------------------------------

        internal PawnPositionLoggingHandler(Pawn pawn)
        {
            _pawn = pawn;
        }

        // --- API: -------------------------------------------------------------------

        /// <summary>
        /// Subscribe/Unsubscribe DebugMobPositionLoggingEvent
        /// </summary>
        internal void SubscribeUnsubscribe(OuterRef<IEntity> outerRef, IEntitiesRepository repo, SubscribeUnsubscribe instruction)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(outerRef))
                {
                    if (wrapper == null)
                    {
                        if(instruction == Aspects.SubscribeUnsubscribe.Subscribe)
                            Logger.IfError()?.Message($"<{GetType().NiceName()}> Subscribe on {outerRef}: {nameof(wrapper)} is null").Write();
                        return;
                    }

                    var movementSync = wrapper.Get<IHasMobMovementSyncAlways>(outerRef, ReplicationLevel.Always);
                    var hasLoggable = wrapper.Get<IHasLogableEntityAlways>(outerRef, ReplicationLevel.Always);

                    switch (instruction)
                    {
                        case Aspects.SubscribeUnsubscribe.Subscribe:
                            if (!movementSync.AssertIfNull(nameof(movementSync)))
                                movementSync.MovementSync.SetDebugMobPositionLoggingEvent += OnSetDebugMobPositionLogging;
                            if (!hasLoggable.AssertIfNull(nameof(hasLoggable)))
                                hasLoggable.LogableEntity.SubscribePropertyChanged(
                                    nameof(hasLoggable.LogableEntity.IsCurveLoggerEnable),
                                    OnIsCurveLoggerEnableChanged);
                            break;

                        case Aspects.SubscribeUnsubscribe.Unsubscribe:
                            if (movementSync != null)
                                movementSync.MovementSync.SetDebugMobPositionLoggingEvent -= OnSetDebugMobPositionLogging;
                            if (hasLoggable != null)
                                hasLoggable.LogableEntity.UnsubscribePropertyChanged(
                                    nameof(hasLoggable.LogableEntity.IsCurveLoggerEnable),
                                    OnIsCurveLoggerEnableChanged);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                    }
                }
            });
        }

        public long NormalizeFrameId(TimeUnits frameId)
        {
            if (_baseFrameId == 0L)
                _baseFrameId = frameId;
            return frameId - _baseFrameId;
        }

        // --- ICurveLoggerProvider: ----------------------------------------------

        public CurveLogger CurveLogger { get; private set; }

        // --- Private methods -----------------------------------------------------------------

        private Task OnIsCurveLoggerEnableChanged(EntityEventArgs args)
        {
            //if ((bool)args.NewValue == true) if (DbgLog.Enabled) DbgLog.Log($"#P3-11221: OnIsCurveLoggerEnableChanged (newval==TRUE) ({EntityId})");

            if ((bool) args.NewValue == true && CurveLogger == null)
                CurveLogger = CurveLogger.Get(EntityId
                    .ToString()); // is enabled from cheats (see `CheatServiceEntity`.SetCurveLoggerStateDo(_) (switches on cluster) & `UnityCheatServiceEntity` called same method (switches on Unity-node).

            return Task.CompletedTask;
        }

        /// <summary>
        /// Сейчас тут только управление специфич-ким включателем логирования для Damper-ноды //Навскидку не вспомнил, зачем.
        /// Тригерится спец.читом `SetDebugMobPositionLogging` (2ой из 3х вызываемых в чите `SetCurveMobLoggingAll`)
        /// </summary>
        private Task OnSetDebugMobPositionLogging(bool enabledStatus, bool dump)
        {
            //if (DbgLog.Enabled) DbgLog.Log($"#P3-11221: {nameof(PawnPositionLoggingHandler)}.OnSetDebugMobPositionLogging {enabledStatus} {dump}");

            if (enabledStatus)
            {
                if (dump)
                    Logger.Warn(
                        $"Unexpected combination of arguments: enabledStatus:{enabledStatus}, dump:{dump}. Only enabledStatus 'll be taken into account.");

                StartPositionLogging();
            }
            else
                StopPositionLogging(dump);

            return Task.CompletedTask;
        }

        private void StartPositionLogging()
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("4. Pawn.StartPositionLogging").Write();;

            if (_pawn._damperNode != null) //null on Server
                _pawn._damperNode.EnableDebugInternalLog = true;
        }

        private void StopPositionLogging(bool dumpToFile)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"6.(1) Pawn. StopPositionLogging({dumpToFile}). EnttyId: " + EntityId).Write();

            _baseFrameId = 0L;

            //UnityQueueHelper.RunInUnityThreadNoWait(() =>
            //{
            //    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"6.(2) Pawn. StopPositionLogging({dumpToFile}). EnttyId: " + _entityId).Write();

            if (_pawn._damperNode != null) //null on Server
                _pawn._damperNode.EnableDebugInternalLog = false;
            //});
        }
    }
}