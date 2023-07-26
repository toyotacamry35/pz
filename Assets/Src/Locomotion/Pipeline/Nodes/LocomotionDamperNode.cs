using System;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;

using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.DebugTag;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;


namespace Src.Locomotion
{
    /*
     * #Notes:
     * Осталось 2 минорных:
     *  1) Есть одиночные точки включения дэмпфера. Причина - клиентский лаг, вследствие чего predicted position расчитана с верной скоростью, но dt, не учитывающим реальное время лага. поэтому. Но решили, что это не проблема, а так (короткое(чаще всего на 1 кадр) включение дэмпфера) возможно даже лучше.
     *  2) Другого рода одиночные точки включения дэмпфера, когда это было не нужно. В этих случаях он включается по условию sqrMgntd расчётной скорости (см. `dPosByDt`) > макс.скорости моба (runningSpeed * 1,1(с запасом)).
     * При этом скорость из `vars` корректная. Причина предположительно в колебаниях этой (расчётной) сорости.
     * Пользователю это не видно. Оперативно найти причину не удалось. Решили не тратить время на дальнейшие разбирательства в причине.
     * (Данные см. в папке `08.01 PZ-9312 Damper2(5) Последний рез-т - Работает, но есть ложные включения`)
     *  3) Есть проблема с временем: например, 9,138 в jdb лога отображено на curve как 9,38 (см. Z==237.623856 в логе клиента в той же папке)
     *  4) Градуировка оси абсцис в clip-asset'е 1,0 1,1 ... 1,5 2,0 (т.е. вместо десятых/сотых - там какие-то 60-тые типа)
     */
    public class LocomotionDamperNode : ILocomotionPipelinePassNode, IResettable
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("LocoDamperNode");
        [NotNull] private static readonly NLog.Logger InternalLoggerToFile = LogManager.GetLogger(nameof(LocomotionDamperNode));

        // Damp only position or position & rotation:
        const bool DampRotationEnabled = false; //true;

        private readonly ISettings _settings;
        private readonly ILocomotionBody _body;
        private readonly float _objMaxSpeedWithGapSqrd;
        private readonly float _objMaxYawSpeedWithGap; // rad/sec
        private readonly float _damperMinDeltaPositionSqrd;
        private readonly float _damperMinDeltaRotationRad;
        private readonly float _damperMaxSpeed;
        private readonly float _damperMaxYawSpeed; // rad/sec
        private readonly ICurveLoggerProvider _curveLogProv;
        private readonly IFrameIdNormalizer _frameIdNormalizer;

        // Should be cleaned every time particular dumping is finished (i.e. when _isDampingOn changes from true to false)
        private LocomotionVector _dampingVelocity;
        //private float _dampingAngSpeed;
        private DamperMode _isDampingOn;
        private Guid _dbgEntId; //Tmp Dbg

        public bool IsReady => true;
        public bool EnableDebugInternalLog;

        public LocomotionDamperNode(
            ISettings settings, 
            ILocomotionBody body, 
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv,
            Guid dbgEntId)
        {
            _settings = settings;
            _body = body;
            _curveLogProv = curveLogProv;
            _frameIdNormalizer = (IFrameIdNormalizer)curveLogProv ?? DefaultFrameIdNormalizer.Instance;
            _objMaxSpeedWithGapSqrd = Sqr(_settings.ObjectMaxSpeed * 1.1f/*Gap*/);
            _objMaxYawSpeedWithGap = _settings.ObjectMaxYawSpeedDeg * Deg2Rad * 1.1f /*Gap*/;
            _damperMinDeltaPositionSqrd = Sqr(_settings.DamperMinDeltaPosition);
            _damperMinDeltaRotationRad = _settings.DamperMinDeltaRotationDeg * Deg2Rad;
            _damperMaxSpeed = Math.Max(_settings.ObjectMaxSpeed * _settings.DamperMaxSpeedFactor, _settings.ObjectMinSpeed);
            _damperMaxYawSpeed = Deg2Rad * Math.Max(_settings.ObjectMaxYawSpeedDeg * _settings.DamperMaxSpeedFactor, _settings.ObjectMinYawSpeedDeg);
            _dbgEntId = dbgEntId;
        }

        bool _dbg_is_settings_printed_once;
        bool _isFirstErrorLogged;
        private LocomotionVector _lastVarsVelo = LocomotionVector.Invalid;
        private float _lastVarsAngVelo = float.NaN;
        private LocomotionVector _lastPos = LocomotionVector.Invalid;
        private float _lastRot = float.NaN;
        private LocomotionTimestamp _lastTimeStamp = LocomotionTimestamp.None;

        public void Reset()
        {
            _dampingVelocity = default;
            _lastVarsVelo = LocomotionVector.Invalid;
            _lastVarsAngVelo = float.NaN;
            _lastPos = LocomotionVector.Invalid;
            _lastRot = float.NaN;
            _lastTimeStamp = LocomotionTimestamp.None;
        }

        public LocomotionVariables Pass(LocomotionVariables vars, float dt)
        {
            /**/ var lastVarsVelo = _lastVarsVelo;
            /**/ _lastVarsVelo = vars.Velocity;
            /**/ var lastVarsAngVelo = _lastVarsAngVelo;
            /**/ _lastVarsAngVelo = vars.AngularVelocity;
            var lastPos = _lastPos;
            var lastRot = vars.Orientation;
            var lastTimeStamp = _lastTimeStamp;
            _lastPos = vars.Position;
            _lastRot = vars.Orientation;
            _lastTimeStamp = vars.Timestamp;

            if (GlobalConstsDef.DebugFlagsGetter.IsDisableLocoClPosDamper(GlobalConstsHolder.GlobalConstsDef)
                /**/ || lastVarsVelo == LocomotionVector.Invalid
                /**/ || float.IsNaN(lastVarsAngVelo)
                || _lastPos == LocomotionVector.Invalid
                || lastPos == LocomotionVector.Invalid
                || float.IsNaN(_lastRot)
                || float.IsNaN(lastRot)
                || !_lastTimeStamp.Valid
                || !lastTimeStamp.Valid
                || vars.Flags.Any(LocomotionFlags.Teleport))
                return vars;

            DebugAgent.Set(DamperTrailBeforeDamp, vars.Position + vars.ExtraPosition);

            // Need it 'cos interpolator passes inconsistent velo & pos.offset: velo from frame whereas position is smoothed (happens after 1 sec server lag)
            var dTimespamps = (vars.Timestamp - lastTimeStamp).Seconds;
            if (dTimespamps < 0) // == 0 is possible, when SyncTime goes back - then LocoSyncedClock don't update Timestamp back
            {
                Logger./*Warn*/Error($"lastTimeStamp > vars.Timestamp (delta seconds:{dTimespamps})");
                return vars;
            }

            // Main work:
            DampPositionIfShould(ref vars, dt, lastPos, lastVarsVelo, dTimespamps);
            if (DampRotationEnabled)
                DampRotationIfShould(ref vars, dt, lastRot, lastVarsAngVelo, dTimespamps);

            DebugAgent.Set(DamperTrailDamped, vars.Position + vars.ExtraPosition);

            return vars;
        }

        private void DampPositionIfShould(ref LocomotionVariables vars, float dt, LocomotionVector lastPos, LocomotionVector lastVarsVelo, float dTimespamps)
        {
            // 2.A. Check, should damp position:

            var dPosByDt = (vars.Position - lastPos) / dTimespamps;
            var offset = /*vars.Velocity*/dPosByDt * dt;// use `last..Velo`, 'cos vars.Pos is reached by that velo.
            var bodyPredictedPos = _body.Position + offset;

            // Log PredictedPos:
            _curveLogProv?.CurveLogger?.IfActive?.AddData("4.0)Cl_Pred-ed.Pos", SyncTime.Now, bodyPredictedPos);

            var delta = vars.Position - bodyPredictedPos;
            if (delta.SqrMagnitude < _damperMinDeltaPositionSqrd 
                && dPosByDt.SqrMagnitude <= _objMaxSpeedWithGapSqrd) //to avoid damper superfluous switching-on
            { // Damper doesn't work
                if ((_isDampingOn & DamperMode.DampPosition)!=0)
                {
                    //_isDampingOn = false;
                    _isDampingOn &= ~DamperMode.DampPosition;
                    _dampingVelocity = LocomotionVector.Zero;
                }
            }
            else
            { // Damper works
                if ((_isDampingOn & DamperMode.DampPosition)==0)
                {
                    //_isDampingOn = true;
                    _isDampingOn |= DamperMode.DampPosition;
                    if (false && EnableDebugInternalLog && InternalLoggerToFile.IsDebugEnabled)
                    {
                        bool cosOfdPos = delta.SqrMagnitude >= _damperMinDeltaPositionSqrd;
                        bool cosOfdPosByDt = dPosByDt.SqrMagnitude > _objMaxSpeedWithGapSqrd;
                        InternalLoggerToFile.IfDebug()?.Message($"LocoDumper On: 'cos of dPos:{cosOfdPos} / dPosDt:{cosOfdPosByDt}. " +
                                                                   $"\n delta.Sqr:{delta.SqrMagnitude}, minDposSqr:{_damperMinDeltaPositionSqrd}, delta:{delta}. " +
                                                                   $"\n dPosByDt.Sqr:{dPosByDt.SqrMagnitude}, _objMaxSpdSqr:{_objMaxSpeedWithGapSqrd}, dPosByDt:{dPosByDt}." +
                                                                   $"\n FrameId: {vars.Timestamp}. (ent: {_dbgEntId})")
                            .Write();
                    }
                }
            }

            // 3.A. The main work (damp Position):

            if ((_isDampingOn & DamperMode.DampPosition) != 0)
            {
                //#Dbg print actual settings
                if (false && !_dbg_is_settings_printed_once)
                {
                    _dbg_is_settings_printed_once = true;
                    Logger.IfDebug()?.Message/*DbgLog.Log*/($"#Dbg(Not Error): =============== SmoothDamp settings: "
                                                               + $"SmoothTime: {_settings.DamperSmoothTime}, " 
                                                               + $"MaxSpeed: {_damperMaxSpeed}" 
                                                               + "\n-----------------Angular:-------------------\n"
                                                               + $"_settings.DamperMinDeltaRotation: {_damperMinDeltaRotationRad}, " 
                                                               + $"_objMaxYawSpeedWithGap: {_objMaxYawSpeedWithGap}, "
                                                               + $"_damperMaxAngSpeed: {_damperMaxYawSpeed}, "
                                                               + $"_settings.ObjectMinSpeed: {_settings.ObjectMinSpeed}.")
                        .Write();
                }

                var bodyPredictedPosByVarsVelo = _body.Position + lastVarsVelo * dt; // Need it to avoid passing to SmoothDamp `bodyPredictedPos` with veeery big offset (at 1st damper frame at 1st new locoFrame at pipeline after server lag)
                var currPosForSmoothDamp = (dPosByDt.SqrMagnitude < lastVarsVelo.SqrMagnitude) ? bodyPredictedPos : bodyPredictedPosByVarsVelo;
                var posWithCorrection = LocomotionVector.SmoothDamp(/*_body.Position*//*bodyPredictedPos*/currPosForSmoothDamp, vars.Position, ref _dampingVelocity, 
                                                                _settings.DamperSmoothTime, _damperMaxSpeed, dt);
                // Check V*dt == pos.correction ?
                if (false && EnableDebugInternalLog && InternalLoggerToFile.IsDebugEnabled)
                {
                    var Vdt = _dampingVelocity * dt;
                    var posCorr = posWithCorrection - currPosForSmoothDamp;
                    var deltaCorr = posCorr - Vdt;
                    InternalLoggerToFile.IfDebug()?.Message($"{deltaCorr.Magnitude} : pc:{posCorr.Magnitude} : Vdt:{Vdt.Magnitude}. (pc:{posCorr}, Vdt:{Vdt})")
                        .Write();
                }

#if DEBUG
                //Log correction:
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4.1)Cl_DampCorr.Pos",        SyncTime.Now, posWithCorrection - currPosForSmoothDamp);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4.1)Cl_DampCorr.Velo",       SyncTime.Now, _dampingVelocity);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4.1)Cl_DampCorr.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(posWithCorrection - currPosForSmoothDamp, _dampingVelocity));
#endif
                vars.Velocity += _dampingVelocity; // Not sure, it's consistent with all done here (and Velocity doesn't used somewhere farther at mob pipeline)
                
                //vars.Velocity.Clamp(_damperMaxSpeed); ///Если клампить, то в 1,0+ * mobReMaxSpeed, чтобы в люб.случ.догнать
                
                //vars.Position = _body.Position + vars.Velocity * dt; ///!use PosCorrection vvv
                // Logger.IfInfo()?.Message($"Damper {_dbgEntId} posWithCorrection={posWithCorrection} Position={vars.Position} Timestamp={vars.Timestamp} dPosByDt<lastVarsVelo={dPosByDt.SqrMagnitude < lastVarsVelo.SqrMagnitude} currPosForSmoothDamp={currPosForSmoothDamp} _settings.DamperSmoothTime={_settings.DamperSmoothTime}").Write();
                vars.Position = posWithCorrection;
#if DEBUG
                //Log damped:
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4)Cl_Damped.Pos",        SyncTime.Now, vars.Position);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4)Cl_Damped.Velo",       SyncTime.Now, vars.Velocity);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4)Cl_Damped.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(ref vars));
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4)Cl_Damped.dPos/dt",    SyncTime.Now, dPosByDt); // calc-ed velo.
                if(false)_curveLogProv?.CurveLogger?.IfActive?.AddData("4)Cl_Damped.FrameId", SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(vars.Timestamp));
#endif
            }

        }

        private void DampRotationIfShould(ref LocomotionVariables vars, float dt, float lastRot, float lastVarsAngVelo, float dTimespamps)
        {
            // 2.B. Check, should damp rotation:

            // in radians ranged to (-pi, +pi]
            var dRotByDt = DeltaAngleRad(lastRot, vars.Orientation) / dTimespamps;
            var rotOffset = /*vars.Velocity*/dRotByDt * dt;// use `last..Velo`, 'cos vars.Pos is reached by that velo.
            var bodyPredictedRot = _body.Orientation + rotOffset;

            var deltaRot = DeltaAngleRad(bodyPredictedRot, vars.Orientation);
            if (Abs(deltaRot) < _damperMinDeltaRotationRad &&  Abs(dRotByDt) <= _objMaxYawSpeedWithGap)
            { // Damper doesn't work
                if ((_isDampingOn & DamperMode.DampRotation) != 0)
                {
                    //_dampingAngSpeed = 0f;
                    RemoveFlag(ref _isDampingOn, DamperMode.DampRotation);
                    _curveLogProv?.CurveLogger?.IfActive?.AddData("4(R).1)Cl_Damp.Rot(On/Off)", SyncTime.Now, -1);
                    if(false)if (DbgLog.Enabled) DbgLog.Log($"Damp.Rot(Off): {_isDampingOn} ({(_isDampingOn & DamperMode.DampRotation)!=0})");
                }
            }
            else
            { // Damper works
                if ((_isDampingOn & DamperMode.DampRotation) == 0)
                {
                    AddFlag(ref _isDampingOn, DamperMode.DampRotation);
                    _curveLogProv?.CurveLogger?.IfActive?.AddData("4(R).1)Cl_Damp.Rot(On/Off)", SyncTime.Now, +1);
                    if(false)if (DbgLog.Enabled) DbgLog.Log($"Damp.Rot(ON): {_isDampingOn} ({(_isDampingOn & DamperMode.DampRotation)!=0})");
                    if (false && EnableDebugInternalLog && InternalLoggerToFile.IsDebugEnabled/*DbgLog.Enabled*/)
                    {
                        bool cosOfdRot = Abs(deltaRot) < _damperMinDeltaRotationRad;
                        bool cosOfdRotByDt = Abs(dRotByDt) <= _objMaxYawSpeedWithGap;
                        InternalLoggerToFile.IfDebug()?.Message/*DbgLog.Log*/($"LocoDumper(ROT) On: 'cos of dRot:{cosOfdRot} / dRotDt:{cosOfdRotByDt}. " +
                                                                                 $"\n deltaRot:{deltaRot}, minDRot:{_damperMinDeltaRotationRad}. " +
                                                                                 $"\n dRotByDt:{dRotByDt}, _objMaxYawSpd:{_objMaxYawSpeedWithGap}." +
                                                                                 $"\n FrameId: {vars.Timestamp}. (ent: {_dbgEntId})")
                            .Write();
                    }
                }
            }

            // 3.B. The main work (damp Rotation):
            if ((_isDampingOn & DamperMode.DampRotation) != 0)
            {
                //#Dbg print actual settings
                if (false && !_dbg_is_settings_printed_once)
                {
                    _dbg_is_settings_printed_once = true;
                    Logger.IfDebug()?.Message/*DbgLog.Log*/($"#Dbg(Not Error): =============== SmoothDamp settings: "
                                                               + $"SmoothTime: {_settings.DamperSmoothTime}, " 
                                                               + $"MaxSpeed: {_damperMaxSpeed}" 
                                                               + "\n-----------------Angular:-------------------\n"
                                                               + $"_settings.DamperMinDeltaRotation: {_damperMinDeltaRotationRad}" 
                                                               + $"_objMaxYawSpeedWithGap: {_objMaxYawSpeedWithGap}"
                                                               + $"_damperMaxAngSpeed: {_damperMaxYawSpeed}")
                        .Write();
                }

                var bodyPredictedRotByVarsAngVelo = _body.Orientation + lastVarsAngVelo * dt; // Need it to avoid passing to SmoothDamp `bodyPredictedPos` with veeery big offset (at 1st damper frame at 1st new locoFrame at pipeline after server lag)
                var currRotForSmoothDamp = (dRotByDt < lastVarsAngVelo) ? bodyPredictedRot : bodyPredictedRotByVarsAngVelo;
                //#todo: сейчас использую векторный SmoothDamp, хотя нужен проще - для float. Заменить облегчённым вариантом.
                // var rotWithCorrection = LocomotionVector.SmoothDamp(/*_body.Position*//*bodyPredictedPos*/new LocomotionVector(currRotForSmoothDamp, 0, 0), vars.Position, ref _dampingVelocity, 
                //                                                 _settings.DamperSmoothTime, _damperMaxSpeed, dt);
                var rotWithCorrection = SimpleDamp( currRotForSmoothDamp, 
                                                    DeltaAngleRad(currRotForSmoothDamp, vars.Orientation), 
                                                     out var dampingAngSpeed,
                                                    _settings.DamperSmoothTime, 
                                                    _damperMaxYawSpeed, 
                                                    dt );

                //Log correction:
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4(R).1)Cl_DampCorr.Rot",      SyncTime.Now, DeltaAngleRad(currRotForSmoothDamp, rotWithCorrection));
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4(R).1)Cl_DampCorr.AngSpeed", SyncTime.Now, dampingAngSpeed);

                vars.AngularVelocity += dampingAngSpeed; // Not sure, it's consistent with all done here (and Velocity doesn't used somewhere farther at mob pipeline)
                
                //vars.Velocity.Clamp(_damperMaxSpeed); ///Если клампить, то в 1,0+ * mobReMaxSpeed, чтобы в люб.случ.догнать
                
                //vars.Position = _body.Position + vars.Velocity * dt; ///!use PosCorrection vvv
                vars.Orientation = rotWithCorrection;

                //Log damped:
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4(R))Cl_Damped.Ori",     SyncTime.Now, vars.Orientation);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4(R))Cl_Damped.AngVelo", SyncTime.Now, vars.AngularVelocity);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("4(R))Cl_Damped.dRot/dt", SyncTime.Now, dRotByDt); // calc-ed velo.
            }
        }

        private void AddFlag(ref DamperMode val, DamperMode flag) => val |= flag;
        private void RemoveFlag(ref DamperMode val, DamperMode flag) => val &= ~flag;

        // --- Util types: -----------------------------

        [Flags]
        enum DamperMode : byte
        {
            Off = 0,
            DampPosition = 1,
            DampRotation = 2,
            // = 4, 8, 16
        }

        public interface ISettings
        {
            /// <summary>
            /// Если текущая погрешность позиции меньше этого значения, дэмпфер "выключается" из pipeline'а (просто выдавая вход на выход без преобразования),
            /// что приводит к фактически мгновенному погашению погрешности (ну с оговоркой про то, как body.MovePosition(..) отработает 
            /// (или чем мы там сейчас двигаем)).
            /// </summary>
            float DamperMinDeltaPosition { get; }

            float DamperMinDeltaRotationDeg { get; }

            /// <summary>
            /// Приблизительное время требующееся для достижения цели. Наименьшее значение достигнет цели быстрее. (не приводит к "черепахе Ахиллеса")
            /// </summary>
            float DamperSmoothTime { get; }

            /// <summary>
            /// (a)`ObjectMaxSpeed`, (b)`ObjectMinSpeed` & (c)`DamperMaxSpeedFactor`:  Damper max speed 'll be := (a) * (c), but Clamped to >= (b)
            /// </summary>
            float ObjectMaxSpeed { get; }
            /// <summary>
            /// (a)`ObjectMaxSpeed`, (b)`ObjectMinSpeed` & (c)`DamperMaxSpeedFactor`:  Damper max speed 'll be := (a) * (c), but Clamped to >= (b)
            /// </summary>
            float ObjectMinSpeed { get; }
            /// <summary>
            /// @Range: expected range: (0,1]
            /// (a)`ObjectMaxSpeed`, (b)`ObjectMinSpeed` & (c)`DamperMaxSpeedFactor`:  Damper max speed 'll be := (a) * (c), but Clamped to >= (b)
            /// &&
            /// (a)`ObjectMaxYawSpeed`, (b)`ObjectMinYawSpeed` & (c)`DamperMaxSpeedFactor`:  Damper max speed 'll be := (a) * (c), but Clamped to >= (b)
            /// </summary>
            float DamperMaxSpeedFactor { get; }

            /// <summary>
            /// (a)`ObjectMaxYawSpeed`, (b)`ObjectMinYawSpeed` & (c)`DamperMaxSpeedFactor`:  Damper max speed 'll be := (a) * (c), but Clamped to >= (b)
            /// </summary>
            float ObjectMaxYawSpeedDeg { get; }
            /// <summary>
            /// (a)`ObjectMaxYawSpeed`, (b)`ObjectMinYawSpeed` & (c)`DamperMaxSpeedFactor`:  Damper max speed 'll be := (a) * (c), but Clamped to >= (b)
            /// </summary>
            float ObjectMinYawSpeedDeg { get; }
        }
    }
}
