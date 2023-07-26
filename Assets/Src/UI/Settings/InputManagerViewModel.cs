using System;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs;
using Src.Input;
// ReSharper disable InconsistentNaming

namespace Uins.Settings
{
    internal class InputManagerViewModel
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        // Registry keys:
        private const string KeyboardLayoutID = "Keyboard_layout";

        internal const int DfltKeyboardLayout_Int = (int)DfltKeyboardLayout;
        internal static readonly int MaxKeyboardLayout_Int = Enum.GetValues(typeof(KeyboardLayout)).Length - 1;
        internal const KeyboardLayout DfltKeyboardLayout = KeyboardLayout.Qwerty;
        internal static DisposableComposite D = new DisposableComposite();
        internal static ReactiveProperty<KeyboardLayout> KeyboardLayoutRp { get; } = new ReactiveProperty<KeyboardLayout>() { Value = DfltKeyboardLayout };
        internal static ReactiveProperty<int> KeyboardLayoutRp_2WayIntReflection { get; } = new ReactiveProperty<int>();

        private static KeyboardLayout SavedKeyboardLayout
        {
            get
            {
                var str = UniquePlayerPrefs.GetString(KeyboardLayoutID, string.Empty);
                if (!Enum.TryParse(str, out KeyboardLayout result))
                    result = DfltKeyboardLayout;
                return result;
            }
            set => UniquePlayerPrefs.SetString(KeyboardLayoutID, value.ToString());
        }

        internal static void Init()
        {
            // Bind `KeyboardLayoutRp` to `_int` & back:
            KeyboardLayoutRp.Action(D, x =>
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** GrSe:: KeyboardLayoutRp --> _2WayIntReflection:  {(KeyboardLayoutRp_2WayIntReflection.HasValue ? KeyboardLayoutRp_2WayIntReflection.Value.ToString() : "no_val")}  -->  {x}({(int)x})");
                if (!KeyboardLayoutRp_2WayIntReflection.HasValue || KeyboardLayoutRp_2WayIntReflection.Value != (int)x)
                    KeyboardLayoutRp_2WayIntReflection.Value = (int)x;
            });

            KeyboardLayoutRp_2WayIntReflection.Action(D, x =>
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** GrSe:: _2WayIntReflection --> KeyboardLayoutRp:  {(KeyboardLayoutRp.HasValue ? KeyboardLayoutRp.Value.ToString() : "no_val")}  -->  {x}({(KeyboardLayout)x})");
                if (KeyboardLayoutRp.HasValue && x == (int)KeyboardLayoutRp.Value)
                    return;

                if (TryIntToKeyboardLayout(x, out var qLvl))
                    KeyboardLayoutRp.Value = qLvl;
                else
                    Logger.IfError()?.Message($"!IsInKeyboardLayoutRange({x})").Write();
            });

            //Единственный раз читаем сохраненные значения
            KeyboardLayoutRp.Value = SavedKeyboardLayout;

            //Связываем изменения значений Rp с 1) изменением громкости в движке, 2) сохранением в UniquePlayerPrefs (реестре)
            KeyboardLayoutRp.Action(D, val =>
            {
                //if (DbgLog.Enabled) DbgLog.Log           ($"#*** GrSe:: KeyboardLayoutRp --> Saved:  {SavedKeyboardLayout}  -->  {val}");
                SavedKeyboardLayout = val;
                SetKeyboardLayout(val);
            });
        }

        internal static void ShutDown()
        {
            D.Clear(); //Убирает все связи с ReactiveProp<>
        }

        internal static void SetKeyboardLayout(KeyboardLayout layout) => InputManager.Instance.SetKeyboardLayout(layout);

        internal static string KeyboardLayoutToString(int indx)
        {
            return (TryIntToKeyboardLayout(indx, out var layout))
                ? layout.ToString()
                : string.Empty;
        }

        static bool IsInKeyboardLayoutRange(int i) => i >= 0 && i <= MaxKeyboardLayout_Int;

        static bool TryIntToKeyboardLayout(int i, out KeyboardLayout layout) //v
        {
            layout = KeyboardLayout.Qwerty; //assign by dflt
            if (!IsInKeyboardLayoutRange(i))
            {
                Logger.IfError()?.Message($"!IsInFullScreenModeRange({i})").Write();
                return false;
            }
            layout = (KeyboardLayout)i;
            return true;
        }

    }
}
