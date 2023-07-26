using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using SharedCode.Utils;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public class LocomotionInterpolationBufferNode : ILocomotionPipelinePassNode
    {
        private static readonly LocomotionLogger Logger = LocomotionLogger.GetLogger(nameof(LocomotionInterpolationBufferNode));
        
        private readonly Queue<LocomotionVariables> _buffer;
        private readonly ILocomotionClock _clock;
        private /*readonly*/ ISettings _settings;
        // ReSharper disable once NotAccessedField.Local
        private readonly Guid _entityId;
        private readonly ICurveLoggerProvider _curveLogProv;
        private readonly IFrameIdNormalizer _frameIdNormalizer;
        // "earliest" from known(not forgotten yet) frames. Is used as prev.frame to interpolate b/w it & next one.
        private LocomotionVariables _earliestFrame = LocomotionVariables.None;
        private LocomotionTimestamp _latestTimestamp = LocomotionTimestamp.None;
        private LocomotionVariables _lastInterpolatedFrame=LocomotionVariables.None;
        private float _prefetchFactor;

        private ClientServerModeSwitcher _switcherClOrS;

        public LocomotionInterpolationBufferNode(
            ILocomotionClock clock,
            Guid entityId,
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv,
            Func<bool> shouldSaveLocoVars = null,
            Action<LocomotionVariables, Type> saveLocoVarsCallback = null)
        : this(clock, EmptySettings, entityId, curveLogProv, shouldSaveLocoVars, saveLocoVarsCallback)
        {}

        public LocomotionInterpolationBufferNode(
            ILocomotionClock clock,
            ISettings settings,
            Guid entityId,
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv
            , Func<bool> shouldSaveLocoVars = null, Action<LocomotionVariables, Type> saveLocoVarsCallback = null)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _entityId = entityId;
            _buffer = new Queue<LocomotionVariables>();
            _curveLogProv = curveLogProv;
            _frameIdNormalizer = (IFrameIdNormalizer)curveLogProv ?? DefaultFrameIdNormalizer.Instance;
            
            ShouldSaveLocoVars = shouldSaveLocoVars;
            SaveLocoVarsCallback = saveLocoVarsCallback;
        }

        public LocomotionInterpolationBufferNode(
            ILocomotionClock clock,
            ISettings settingsCl,
            ISettings settingsS,
            bool initialStateIsServer,
            Guid entityId,
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv,
            Func<bool> shouldSaveLocoVarS = null,
            Action<LocomotionVariables, Type> saveLocoVarsCallbackS = null)
        : this (clock, settingsCl, entityId, curveLogProv, shouldSaveLocoVarS, saveLocoVarsCallbackS)
        {
            _switcherClOrS = new ClientServerModeSwitcher(
                this,
                settingsCl,
                settingsS,
                shouldSaveLocoVarS,
                saveLocoVarsCallbackS);
            _switcherClOrS.SwitchState(initialStateIsServer, true);
        }
        //#note: Вообще сейчас это напрасные пляски, т.к. факт-ки эти settings мобов одинаковые для S & Cl (PrefetchTime & PrefetchChangingTime), а колбэки дебажные - с ними, когда и если снова понядобятся можно разобраться отдельно
        //       Но чтоб по фен-шую, пусть будет так - аккуратно и устойчиво (условно - если настройки эти бу отличаться, то будет возможно заметно резкое изменение значения при переключении Cl <--> S (хотя оно вроде и правильно. и пусть DamperNode делает свою работу и сглаживает)
        public void SwitchState(bool isServer)
        {
            _switcherClOrS.SwitchState(isServer);
        }
        private void ReinitDo(ISettings settings, Func<bool> shouldSaveLocoVars, Action<LocomotionVariables, Type> saveLocoVarsCallback)
        {
            _settings = settings;
            ShouldSaveLocoVars = shouldSaveLocoVars;
            SaveLocoVarsCallback = saveLocoVarsCallback;
        }

        ///#PZ-13568: #Dbg:
        protected Func<bool> ShouldSaveLocoVars;
        protected Action<LocomotionVariables, Type> SaveLocoVarsCallback;

        bool ILocomotionPipelinePassNode.IsReady => true;

        LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float dt)
        {
            if (!vars.Timestamp.Valid) throw new ArgumentException("Timestamp of frame is not valid", "vars.Timestamp");

            if (vars.Timestamp > _latestTimestamp)
            {
                _buffer.Enqueue(vars);
                _latestTimestamp = vars.Timestamp;
            }
            else if (vars.Timestamp < _latestTimestamp)
                LocomotionLogger.Default.IfWarn()?.Message("New frame timestamp is less than previous frame timestamp: {0} < {1}", vars.Timestamp, _latestTimestamp).Write();

            if (!_earliestFrame.Timestamp.Valid)
            {
                _lastInterpolatedFrame = vars;
                _earliestFrame = vars;
            }

            if (_buffer.Count == 0)
            {
#if DEBUG
                _curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.Pos",        SyncTime.Now, _earliestFrame.Position);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.Velo",       SyncTime.Now, _earliestFrame.Velocity);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(ref _earliestFrame));
                if (false)_curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.FrameId", SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(_earliestFrame.Timestamp));

                if (ShouldSaveLocoVars?.Invoke() ?? false)
                    SaveLocoVarsCallback(_earliestFrame, this.GetType());
#endif

                return _lastInterpolatedFrame;
            }

            if (vars.Flags.Any(LocomotionFlags.Direct) &&_prefetchFactor > 0)
                _prefetchFactor = _settings.PrefetchChangingTime > Epsilon ? Max(_prefetchFactor - dt / _settings.PrefetchChangingTime, 0) : 0;  
            else
            if (!vars.Flags.Any(LocomotionFlags.Direct) &&_prefetchFactor < 1)
                _prefetchFactor = _settings.PrefetchChangingTime > Epsilon ? Min(_prefetchFactor + dt / _settings.PrefetchChangingTime, 1) : 1;  
            
            var prefetchTime = (LocomotionTimestamp)(_settings.PrefetchTime * _prefetchFactor);
            var fetchTime = _clock.Timestamp - prefetchTime;
            
            LocomotionVariables frame;
            do
            {
                frame = _buffer.Peek();
                //#no_need here (if simplified, lerp moved inside if): ///#PZ-9312: ???
                // Logger.IfInfo()?.Message($"Interpolation {_entityId} fetchTime={fetchTime} _clock.Timestamp={_clock.Timestamp} prefetchTime={prefetchTime} frame.Timestamp={frame.Timestamp} _prefetchFactor={_prefetchFactor} vars.Flags.Any(LocomotionFlags.Direct)={vars.Flags.Any(LocomotionFlags.Direct)}").Write();
                var t = LocomotionTimestamp.InverseLerp(_earliestFrame.Timestamp, frame.Timestamp, fetchTime);
                if (t < 1) // i.e.: now < frame.Timestamp. i.e. we found 1st frame from "future"
                { 
                    // if (_clock.Timestamp < frame.Timestamp) // i.e.: now < frame.Timestamp. i.e. we found 1st frame from "future"
                    // {
                    //     var t = LocomotionTimestamp.InverseLerp(_earliestFrame.Timestamp, frame.Timestamp, _clock.Timestamp);
                    Logger.IfTrace()?.Message("EarliestTimestamp:{0} Frame.Timestamp:{1} Clock:{2} T:{3} Count:{4}", _earliestFrame.Timestamp, frame.Timestamp, _clock.Timestamp, t, _buffer.Count)
                        .Write();
                    frame = t <= 0 ? _earliestFrame : Lerp(_earliestFrame, frame, t); // frame interpolation
                    break;
                }
                else
                {
                    _buffer.Dequeue();
                    _earliestFrame = frame;
                }
            } while (_buffer.Count > 0);

            if (_buffer.Count == 0)
                Logger.IfDebug()?.Message("Interpolation buffer underflow. EarliestTimestamp:{0} FetchTime:{1} Clock:{2} Prefetch:{3}", _earliestFrame.Timestamp, fetchTime, _clock.Timestamp, prefetchTime)
                    .Write(); 
            
            frame.Timestamp = _clock.Timestamp;
            _lastInterpolatedFrame = frame;

#if DEBUG
            _curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.Pos",        SyncTime.Now, frame.Position);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.Velo",       SyncTime.Now, frame.Velocity);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(ref frame));
            _curveLogProv?.CurveLogger?.IfActive?.AddData("2)Cl_Inter-ed.FrameId",    SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(frame.Timestamp));

            if (ShouldSaveLocoVars?.Invoke() ?? false)
                SaveLocoVarsCallback(frame, this.GetType());
#endif

            // Logger.IfInfo()?.Message($"Interpolation {_entityId} Velocity={frame.Velocity} Orientation={frame.Orientation} Position={frame.Position}").Write();
            return frame;
        }

        /// <summary>
        /// #Note: Timestamp should be stored outside manually.
        /// </summary>
        private static LocomotionVariables Lerp(LocomotionVariables a, LocomotionVariables b, float t)
        {
            return b.Flags.Any(LocomotionFlags.Teleport)
                ? a
                : new LocomotionVariables(
                    flags: a.Flags,
                    position: LocomotionVector.Lerp(a.Position, b.Position, t),
                    velocity: LocomotionVector.Lerp(a.Velocity, b.Velocity, t),
                    orientation: LerpAngleRad(a.Orientation, b.Orientation, t),
                    angularVelocity: SharedHelpers.Lerp(a.AngularVelocity, b.AngularVelocity, t)
                );
        }

        public interface ISettings
        {
            Int64 PrefetchTime { get; }
            float PrefetchChangingTime { get; }
        }

        public class NullSettings : ISettings
        {
            public Int64 PrefetchTime => 0;
            public float PrefetchChangingTime => 0;
        }
     
        // Вынесенная в отдельный класс логика изменения мода: Server или Client налету.
        // Для возможности использования одного инстанса буфера в обоих loco-пайплайнах server'ном и client'ском (из которых в каждый апдейт работает только какой-то один (т.е. не оба одновременно))
        private class ClientServerModeSwitcher
        {
            private bool _isServerState;
            private LocomotionInterpolationBufferNode _controlledNode;
            private readonly ISettings _settingsCl;
            private readonly ISettings _settingsS;
            private readonly Func<bool> _shouldSaveLocoVarS;
            private readonly Action<LocomotionVariables, Type> _saveLocoVarsCallbackS;
            internal ClientServerModeSwitcher(
                LocomotionInterpolationBufferNode controlledNode,
                ISettings settingsCl,
                ISettings settingsS,
                Func<bool> shouldSaveLocoVarS = null,
                Action<LocomotionVariables, Type> saveLocoVarsCallbackS = null)
            {
                _controlledNode = controlledNode;
                _settingsCl = settingsCl;
                _settingsS = settingsS;
                _shouldSaveLocoVarS = shouldSaveLocoVarS;
                _saveLocoVarsCallbackS = saveLocoVarsCallbackS;
            }

            internal void SwitchState(bool isServer, bool forced = false)
            {
                if (!forced && _isServerState == isServer)
                    return;

                if (isServer)
                    _controlledNode.ReinitDo(_settingsS, _shouldSaveLocoVarS, _saveLocoVarsCallbackS);
                else
                    _controlledNode.ReinitDo(_settingsCl, null, null);
                _isServerState = isServer;
            }
        }

        public static readonly NullSettings EmptySettings = new NullSettings();  

    }
}