using System;
using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Locomotion.Debug;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Utils;
using ColonyShared.SharedCode;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using Src.Locomotion;
using TimeUnits = System.Int64;
using SharedCode.Serializers;

namespace Assets.Src.NetworkedMovement
{
    public class CharacterPositionLoggingHandler : ILocoCurveLoggerProvider, ILocomotionUpdateable, ILocomotionDebugAgent
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(CharacterPositionLoggingHandler));

        private OuterRef _entityRef;
        private long _baseFrameId;

        internal CharacterPositionLoggingHandler()
        {
        }

        /// <summary>
        /// Subscribe/Unsubscribe DebugMobPositionLoggingEvent
        /// </summary>
        internal void SubscribeUnsubscribe(OuterRef entityRef, IEntitiesRepository repo, SubscribeUnsubscribe instruction)
        {
            if(!entityRef.IsValid) throw new ArgumentException(nameof(entityRef));
            _entityRef = entityRef;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(_entityRef.TypeId, _entityRef.Guid))
                {
                    var hasLoggable = wrapper?.Get<IHasLogableEntityAlways>(_entityRef, ReplicationLevel.Always);
                    if (hasLoggable == null)
                    {
                        //Logger.IfError()?.Message($"Can't get `{nameof(IDebugPositionLoggerServer)}` by outerRef {outerRef}").Write();
                        return;
                    }

                    switch (instruction)
                    {
                        case Aspects.SubscribeUnsubscribe.Subscribe:
                            hasLoggable.LogableEntity.SubscribePropertyChanged(nameof(hasLoggable.LogableEntity.IsCurveLoggerEnable), OnIsCurveLoggerEnableChanged);
                            break;
                        case Aspects.SubscribeUnsubscribe.Unsubscribe:
                            hasLoggable.LogableEntity.UnsubscribePropertyChanged(nameof(hasLoggable.LogableEntity.IsCurveLoggerEnable), OnIsCurveLoggerEnableChanged);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                    }
                }
            }, repo);
        }

        public long NormalizeFrameId(TimeUnits frameId)
        {
            if (_baseFrameId == 0L)
                _baseFrameId = frameId;
            return frameId - _baseFrameId;
        }

        public CurveLogger CurveLogger { get; private set; }

        private Task OnIsCurveLoggerEnableChanged(EntityEventArgs args)
        {
            var enable = (bool) args.NewValue;
            
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityRef.Guid, $"{nameof(OnIsCurveLoggerEnableChanged)}({enable})").Write();
            
            if (enable && CurveLogger == null)
                CurveLogger = CurveLogger.Get(_entityRef.Guid.ToString()); // is enabled from cheats (see `CheatServiceEntity`.SetCurveLoggerStateDo(_) (switches on cluster) & `UnityCheatServiceEntity` called same method (switches on Unity-node).
            if (!enable)
                _baseFrameId = 0;
            return Task.CompletedTask;
        }

        public void Update(float deltaTime)
        {
            CurveLogger?.IfActive?.AddData("7) LocoUpdNow", SyncTime.NowUnsynced, 0);
        }

        bool ILocomotionDebugAgent.IsActive => CurveLogger != null && CurveLogger.Active;

        void ILocomotionDebugAgent.Add(DebugTag id, Value entry)
        {
            if (CurveLogger != null && CurveLogger.Active)
                CurveLogger.AddData(id.ToString(), SyncTime.NowUnsynced, entry);
        }

        void ILocomotionDebugAgent.BeginOfFrame() {}

        void ILocomotionDebugAgent.EndOfFrame() {}
    }
}