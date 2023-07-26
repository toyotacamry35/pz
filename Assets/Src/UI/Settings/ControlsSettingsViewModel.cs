using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs;
using Src.InputActions;
// ReSharper disable CommentTypo

namespace Uins.Settings
{
    public static class ControlsSettingsViewModel
    {
        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly Logger Logger = LogManager.GetLogger("ControlsSettingsViewModel");

        // Registry keys:
        private const string MouseSensivityID = "Mouse_sensivity";
        private const string MouseInversionID = "Mouse_inversion";

        internal static float DfltIsMouseSensivity => ControlsModifiers.DfltIsMouseSensivity;
        internal static bool DfltIsMouseInverted => ControlsModifiers.DfltIsMouseInverted;
        internal static float MinMouseSensivity => ControlsModifiers.MinMouseSensivity;
        internal static float MaxMouseSensivity => ControlsModifiers.MaxMouseSensivity;
        internal static DisposableComposite D = new DisposableComposite();

        internal static ReactiveProperty<float> MouseSensivityRp { get; } = new ReactiveProperty<float>() { Value = DfltIsMouseSensivity };
        internal static ReactiveProperty<bool>  MouseInversionRp { get; } = new ReactiveProperty<bool>()  { Value = DfltIsMouseInverted };

        internal static void Init()
        {
            //Единственный раз читаем сохраненные значения
            MouseSensivityRp.Value = SavedMouseSensivity;
            MouseInversionRp.Value = SavedMouseInversion;

            //Связываем изменения значений Rp с 1) изменением громкости в движке, 2) сохранением в UniquePlayerPrefs (реестре)
            MouseSensivityRp.Action(D, val =>
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** CoSe:: MouseSensivityRp --> Saved:  {SavedMouseSensivity}  -->  {val}");
                SavedMouseSensivity = val;
                SetMouseSensivity(val);
            });
            MouseInversionRp.Action(D, val =>
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** CoSe:: MouseInversionRp --> Saved:  {SavedMouseInversion}  -->  {val}");
                SavedMouseInversion = val;
                SetMouseInversion(val);
            });
        }

        internal static void ShutDown()
        {
            D.Clear(); //Убирает все связи с ReactiveProp<>
        }


        // --- Privates: ----------------------------------------------
        private static float SavedMouseSensivity
        {
            get => UniquePlayerPrefs.GetFloat(MouseSensivityID, DfltIsMouseSensivity);
            set => UniquePlayerPrefs.SetFloat(MouseSensivityID, value);
        }

        private static bool SavedMouseInversion
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            get => UniquePlayerPrefs.GetBool(MouseInversionID, DfltIsMouseInverted);
            set => UniquePlayerPrefs.SetBool(MouseInversionID, value);
        }
        
        private static void SetMouseSensivity(float val)
        {
            //if (DbgLog.Enabled) DbgLog.Log($"#*** CoSe:: A. SetMouseSensivity: {"предыдущее значение"} --> {val}");

            ControlsModifiers.MouseSensivity = val;
            Logger.IfInfo()?.Message($"{nameof(SetMouseSensivity)}({val}).").Write();

            //if (DbgLog.Enabled) DbgLog.Log($"#*** CoSe:: Z. SetMouseSensivity: {"предыдущее значение"} --> {val}");
        }
        private static void SetMouseInversion(bool val)
        {
            //if (DbgLog.Enabled) DbgLog.Log($"#*** CoSe:: A. SetMouseInversion: {"предыдущее значение"} --> {val}");

            ControlsModifiers.IsMouseInverted = val;
            Logger.IfInfo()?.Message($"{nameof(SetMouseInversion)}({val}).").Write();

            //if (DbgLog.Enabled) DbgLog.Log($"#*** CoSe:: Z. SetMouseInversion: {"предыдущее значение"} --> {val}");
        }
        
    }
}
