using System;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;
using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.DebugTag;

using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;


namespace Src.Locomotion
{
    public class LocomotionNetworkSender : ILocomotionPipelineCommitNode, IDisposable, IStopAndRestartable
    {
        internal static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(LocomotionNetworkSender));

        private readonly ILocomotionClock _clock;
        private readonly ILocomotionRelevancyLevelProvider _relevancy;
        private readonly ILocomotionNetworkSend _transport;
        private readonly ISettings _settings;
        private readonly ICurveLoggerProvider _curveLogProv;
        private readonly IFrameIdNormalizer _frameIdNormalizer;
        private LocomotionVariables _lastSentVars;
        private float _nextSentTime;
        private readonly IDumpingLoggerProvider _loggerProvider;
        private readonly Guid _entityId;
        private readonly Type _thisType = typeof(LocomotionNetworkSender);

        public LocomotionNetworkSender(
            ILocomotionNetworkSend transport,
            ILocomotionClock clock,
            ILocomotionRelevancyLevelProvider relevancy,
            ISettings settings,
            IDumpingLoggerProvider loggerProvider,
            Guid entityId,
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv = null)
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));
            if (settings  == null) throw new ArgumentNullException(nameof(settings));
            if (clock     == null) throw new ArgumentNullException(nameof(clock));
            if (relevancy == null) throw new ArgumentNullException(nameof(relevancy));
            if (settings  == null) throw new ArgumentNullException(nameof(settings));
            _transport = transport;
            _clock = clock;
            _relevancy = relevancy;
            _settings = settings;
            Restart();
            _curveLogProv = curveLogProv;
            _frameIdNormalizer = (IFrameIdNormalizer)curveLogProv ?? DefaultFrameIdNormalizer.Instance;
            _loggerProvider = loggerProvider;
            _entityId = entityId;
        }

        // (Unfreeze) Call when should restart working again:
        public void Restart()
        {
            _transport.Acquire();
        }

        // (Freeze) Call when should stop working:
        public void Stop()
        {
            _transport.Release();
        }

        void IDisposable.Dispose()
        {
            Stop();
        }

        bool ILocomotionPipelineCommitNode.IsReady => true;

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: e)LocoN/wSender");

            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagIn, inVars);

            if (!inVars.Timestamp.Valid) throw new ArgumentException("Timestamp of frame is not valid", "vars.Timestamp");
            var relevanceLevel = _relevancy.RelevancyLevelForNetwork;
            DebugAgent.Set(RelevanceLevel, relevanceLevel);
            bool shouldResetVelocityToZero;

            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: hasReason");
            var reason = HasReasonToSend(ref inVars, _clock.Time, _lastSentVars, _nextSentTime, relevanceLevel, _settings, out shouldResetVelocityToZero);
            LocomotionProfiler.EndSample();

            if (reason != ReasonForSend.None)
            {
                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: reason != None lastSend");
                _lastSentVars = inVars;
                if (shouldResetVelocityToZero)
                    _lastSentVars.Velocity = LocomotionVector.Zero;
                LocomotionProfiler.EndSample();

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: reason != None SendInterval");
                var sendInterval = _settings.SendInterval(relevanceLevel);
                _nextSentTime = _clock.Time + sendInterval;
                LocomotionProfiler.EndSample();

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: reason != None Send");

                //#Dbg:
                if (_loggerProvider?.LogBackCounter > 0)
                    _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut, _lastSentVars);

                // Logger.IfInfo()?.Message($"{_entityId} sendframe Velocity={_lastSentVars.Velocity} inVars.Velocity={inVars.Velocity} Orientation={_lastSentVars.Orientation} shouldResetVelocityToZero={shouldResetVelocityToZero} reason={reason}").Write();
                Send(_lastSentVars, reason);
                LocomotionProfiler.EndSample();
            }

            _curveLogProv?.CurveLogger?.IfActive?.AddData("0.-) Send.inVelo", SyncTime.Now, inVars.Velocity);        

            LocomotionProfiler.EndSample();
        }

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        //@param `resetVelocityToZero` - to avoid the need to made copy of `inVars` outside
        private static ReasonForSend HasReasonToSend(
            ref LocomotionVariables inVars,
            float time,
            LocomotionVariables lastSentVars,
            float nextSentTime,
            float relevancyLevel,
            ISettings settings,
            out bool resetVelocityToZero)
        {
            resetVelocityToZero = false;

            if (nextSentTime == 0)
                return ReasonForSend.Initial;

            if (relevancyLevel >= 0)
            {
                // изменение скорости гораздо важнее изменения позиции для интерполятора на удалённой стороне,
                // поэтому (резкие) изменения скорости отсылаем без задержек 
                if (inVars.Velocity.Longer(settings.ZeroVelocityThreshold))
                {
                    if (!inVars.Velocity.ApproximatelyEqual(lastSentVars.Velocity, settings.SendVelocityDiffThreshold(relevancyLevel)))
                        return ReasonForSend.Velocity;
                }
                else
                {
                    if (lastSentVars.Velocity.Longer(settings.ZeroVelocityThreshold))
                    {
                        //vars.Velocity = LocomotionVector.Zero;
                        resetVelocityToZero = true;
                        return ReasonForSend.ZeroVelocityThreshold;
                    }
                }
            }
            else
            {
                //vars.Velocity = LocomotionVector.Zero;
                resetVelocityToZero = true;
                /*
                 * #Note: Не занулять нельзя!:
                 * До аниматора на Cl долетает уже 0-velo. Как выяснилось, она зануляется в LocoNwSender. Незанулять плохо -сайд - эффект - экстраполятор
                 * прёт квара с последней скоростью до получения след.пакета(а это часто через 2 сек).Результат - моба каждый раз оттягивает назад после 
                 * каждой конечной фазы движения. Решили использовать вычисленную скорость в ноде аниматора. 
                 */
            }

            if (settings.SendOnlyImportantFlags(relevancyLevel))
            {
                if ((inVars.Flags & ~NotImportantFlags) != (lastSentVars.Flags & ~NotImportantFlags))
                    return ReasonForSend.Flags;

                if (inVars.Flags != 0 && lastSentVars.Flags == 0)
                    return ReasonForSend.Flags;
            }
            else
            {
                if (inVars.Flags != lastSentVars.Flags)
                    return ReasonForSend.Flags;
            }

            if (time >= nextSentTime)
            {
                if (!inVars.Position.ApproximatelyEqual(lastSentVars.Position, settings.SendPositionDiffThreshold(relevancyLevel)))
                    return ReasonForSend.Position;

                if (relevancyLevel >= 0)
                {
                    if (inVars.Flags != lastSentVars.Flags)
                        return ReasonForSend.Flags;

                    if (Mathf.Abs(Mathf.DeltaAngleRad(inVars.Orientation, lastSentVars.Orientation)) >
                        settings.SendRotationDiffThreshold(relevancyLevel))
                        return ReasonForSend.Orientation;
                }
            }


            return ReasonForSend.None;
        }

        private void Send(LocomotionVariables frame, ReasonForSend reason)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("Loco Commit: e)LocoN/wSender: .Send");
            var important = reason == ReasonForSend.Flags
                         || reason == ReasonForSend.ZeroVelocityThreshold
                         || reason == ReasonForSend.Initial;
            _transport.SendFrame(frame, important, reason);
            LocomotionProfiler.EndSample();

            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("!!!DBG: ## Loco Commit: e)LocoN/wSender .Send DebugAgent.Set");
            if (DebugAgent.IsNotNullAndActive()) 
            { 
                DebugAgent.Set(NetworkSentPosition, frame.Position);
                DebugAgent.Set(NetworkSentVelocity, frame.Velocity);
                DebugAgent.Set(NetworkSentOrientation, frame.Orientation);
                DebugAgent.Set(NetworkSentReason, (int)reason);
                DebugAgent.Set(NetworkSentFrameId, frame.Timestamp);
            }
            _curveLogProv?.CurveLogger?.IfActive?.AddData("0)S_Sent.Pos",        SyncTime.Now, frame.Position);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("0)S_Sent.Velo",       SyncTime.Now, frame.Velocity);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("0)S_Sent.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(ref frame));
            _curveLogProv?.CurveLogger?.IfActive?.AddData("0)S_Sent.FrameId",    SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(frame.Timestamp));
            LocomotionProfiler.EndSample();
        }

        private const LocomotionFlags NotImportantFlags =
            LocomotionFlags.Dodge |
            LocomotionFlags.Landing |
            LocomotionFlags.Direct |
            LocomotionFlags.Slipping |
            LocomotionFlags.Turning | 
            LocomotionFlags.NoCollideWithActors 
        ;

        public interface ISettings
        {
            /// <summary>
            /// Порог разницы позиций между кадрами достаточный для отправки нового кадра
            /// </summary>
            float SendPositionDiffThreshold(float relevanceLevel);

            /// <summary>
            /// Порог разницы скоростей между кадрами достаточный для отправки нового кадра
            /// </summary>
            float SendVelocityDiffThreshold(float relevanceLevel);

            /// <summary>
            /// Порог разницы ориентации между кадрами достаточный для отправки нового кадра (радианы)
            /// </summary>
            float SendRotationDiffThreshold(float relevanceLevel);

            /// <summary>
            /// Минимальный период отсылки кадров
            /// </summary>
            float SendInterval(float relevancyLevel);

            /// <summary>
            /// Порог значения скорости ниже которого скорость считается нулевой  
            /// </summary>
            float ZeroVelocityThreshold { get; }

            bool SendOnlyImportantFlags(float relevanceLevel);
        }
    }
}
