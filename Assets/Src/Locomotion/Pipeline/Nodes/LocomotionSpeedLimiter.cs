using System;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public class LocomotionSpeedLimiter : ILocomotionPipelinePassNode
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Guid _entityId;
        private readonly ISettings _settings;
        private bool _horizontalOverLimit;
        private bool _verticalOverLimit;
        private readonly IDumpingLoggerProvider _loggerProvider;
        private readonly Type _thisType = typeof(LocomotionSpeedLimiter);

        public LocomotionSpeedLimiter(ISettings settings, Guid entityId, IDumpingLoggerProvider loggerProvider)
        {
            _entityId = entityId;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _loggerProvider = loggerProvider;
        }

        public bool IsReady => true;

        public LocomotionVariables Pass(LocomotionVariables vars, float dt)
        {
            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagIn, vars);

            if (vars.Flags.Any(LocomotionFlags.CheatMode))
            {
                //#Dbg:
                if (_loggerProvider?.LogBackCounter > 0)
                    _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut1, vars);

                return vars;
            }

            if (vars.Velocity.Horizontal.sqrMagnitude > _settings.HorizontalSpeedLimit * _settings.HorizontalSpeedLimit)
            {
                if (!_horizontalOverLimit)
                {
                    Logger.IfWarn()?.Message(_entityId, $"Horizontal speed limit exceeded: {vars.Velocity.Horizontal.magnitude}").Write();
                    _horizontalOverLimit = true;
                }   
                vars.Velocity = new LocomotionVector(vars.Velocity.Horizontal.Normalized * _settings.HorizontalSpeedLimit, vars.Velocity.Vertical);
            }
            else
                _horizontalOverLimit = false;

            if (Abs(vars.Velocity.Vertical) > _settings.VerticalSpeedLimit)
            {
                if (!_verticalOverLimit)
                {
                    Logger.IfWarn()?.Message(_entityId, $"Vertical speed limit exceeded: {vars.Velocity.Vertical}").Write();
                    _verticalOverLimit = true;
                }   
                vars.Velocity = new LocomotionVector(vars.Velocity.Horizontal, _settings.VerticalSpeedLimit * Sign(vars.Velocity.Vertical));
            }
            else
                _verticalOverLimit = false;

            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut2, vars);

            return vars;
        }

        public interface ISettings
        {
            float HorizontalSpeedLimit { get; }
            float VerticalSpeedLimit { get; }
        }
    }
}