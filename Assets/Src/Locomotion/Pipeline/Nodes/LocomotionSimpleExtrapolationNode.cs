using System;
using Assets.Src.Locomotion.Debug;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public class LocomotionSimpleExtrapolationNode : ILocomotionPipelinePassNode
    {
        private readonly ILocomotionClock _clock;
        private readonly Guid _entityId;
        private readonly ISettings _settings;
        private readonly ICurveLoggerProvider _curveLogProv;
        private readonly IFrameIdNormalizer _frameIdNormalizer;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public LocomotionSimpleExtrapolationNode(
            ISettings settings, 
            ILocomotionClock clock, 
            Guid entityId,
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _entityId = entityId;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _curveLogProv = curveLogProv;
            _frameIdNormalizer = (IFrameIdNormalizer)curveLogProv ?? DefaultFrameIdNormalizer.Instance;
        }

        bool ILocomotionPipelinePassNode.IsReady => true;

        LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float dt)
        {
            if (!vars.Timestamp.Valid) throw new ArgumentException("Timestamp of frame is not valid", "vars.Timestamp");
            if (_clock.Timestamp > vars.Timestamp)
            {
                float delta = (_clock.Timestamp - vars.Timestamp).Seconds;
                var newPosition = vars.Position + vars.Velocity * Min(delta, _settings.MaxExtrapolationTime);
                // _logger.Info($"{_entityId} Extrapolation extrapolated OldPosition={vars.Position} NewPosition={newPosition} delta={delta} timestampWithOffset={timestampWithOffset} _clock.Timestamp={_clock.Timestamp}");
                vars.Position = newPosition;
                vars.Timestamp = _clock.Timestamp;
            }
            // else
            // {
            //     _logger.Info($"{_entityId} Extrapolation doesn't affect _clock.Timestamp={_clock.Timestamp} timestampWithOffset={timestampWithOffset} real vars.Timestamp={vars.Timestamp}");
            // }

#if DEBUG
            _curveLogProv?.CurveLogger?.IfActive?.AddData("3)Cl_Extra-ed.Pos",        SyncTime.Now, vars.Position);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("3)Cl_Extra-ed.Velo",       SyncTime.Now, vars.Velocity);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("3)Cl_Extra-ed.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(ref vars));
            if (false)_curveLogProv?.CurveLogger?.IfActive?.AddData("3)Cl_Extra-ed.FrameId", SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(vars.Timestamp));
#endif
            return vars;
        }
        
        public interface ISettings
        {
            /// <summary>
            /// Максимальное время с момента получения последнего кадра в течении которого интерполируется текущая позиция
            /// </summary>
            float MaxExtrapolationTime { get; }
        }
    }
}