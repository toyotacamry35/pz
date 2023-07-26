using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;

namespace Src.InputActions
{
    public static class ControlsModifiers
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal const float DfltIsMouseSensivity = 1.0f;
        internal const bool DfltIsMouseInverted = false;
        internal const float MinMouseSensivity = 0.1f;
        internal const float MaxMouseSensivity = 2f;

        private static bool _isMouseInverted;
        public static bool IsMouseInverted
        {
            get => _isMouseInverted;
            set
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** CoMo:: A. Set_IsMouseInverted: {_isMouseInverted} --> {value}");

                _isMouseInverted = value;
                Logger.IfInfo()?.Message($"Set_{nameof(IsMouseInverted)}({value}).").Write();

                //if (DbgLog.Enabled) DbgLog.Log($"#*** CoMo:: Z. Set_IsMouseInverted: {_isMouseInverted} --> {value}");
            }
        }

        private static float _mouseSensivity;
        public static float MouseSensivity
        {
            get => _mouseSensivity;
            set
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** CoMo:: A. Set_MouseSensivity: {_isMouseInverted} --> {value}");

                if (!SharedHelpers.InRange(value, MinMouseSensivity, MaxMouseSensivity))
                {
                    Logger.IfError()?.Message($"Passed value({value}) is out of range: min:{MinMouseSensivity}, max:{MaxMouseSensivity}. 'll be clamped").Write();
                    value = SharedHelpers.Clamp(value, MinMouseSensivity, MaxMouseSensivity);
                }
                _mouseSensivity = value;
                Logger.IfInfo()?.Message($"Set_{nameof(MouseSensivity)}({value}).").Write();

                //if (DbgLog.Enabled) DbgLog.Log($"#*** CoMo:: Z. Set_MouseSensivity: {_isMouseInverted} --> {value}");
            }
        }
    }

}
