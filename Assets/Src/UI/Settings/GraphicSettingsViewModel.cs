using System;
using System.Collections;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using EnumerableExtensions;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using UnityEngine;
using Utilities;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace Uins.Settings
{
    internal static class GraphicSettingsViewModel
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("GraphicSettings");

        // Нужно будет перевести на новые рельсы, когда при мерже встретится с веткой Влада Г. 
        private static readonly LocalizationKeysDef FullscreenModeLocalizationKeys = GlobalConstsHolder.GlobalConstsDef.LocalizationKeysDefsHolder.Target?.FullscreenModeLocalizationKeys.Target;

        // Registry keys:
        private const string WindowModeID = "Window_mode";
        private const string ResolutionWidthID = "Resolution_W";
        private const string ResolutionHeightID = "Resolution_H";
        private const string VSyncID = "V_Sync";
        //private const string RefreshRateID = "Refresh_rate";

        internal const FullScreenModeShort DfltWindowMode = FullScreenModeShort.ExclusiveFullScreen; 
        internal const int DfltWindowMode_Int = (int)DfltWindowMode;
        internal static readonly int MaxWindowMode_Int = Enum.GetValues(typeof(FullScreenModeShort)).Length -1;
        internal static ResolutionShort DfltResolution = new ResolutionShort(Screen.currentResolution);
        
        internal static int DfltResolution_Int => GetResolutionIndex(DfltResolution);
        internal static readonly ResolutionShort[] Resolutions = Screen.resolutions.Select(x => new ResolutionShort(x)).Distinct().ToArray();
        internal static readonly int MaxResolution_Int = Resolutions.Length -1;
        internal const bool DfltVSync = false;

        internal static DisposableComposite D = new DisposableComposite();

        internal static ReactiveProperty<FullScreenModeShort> WindowModeRp { get; } = new ReactiveProperty<FullScreenModeShort>() { Value = DfltWindowMode };
        //#todo: Think, how move UI-representation specific out from here - to UI VM! (this task looks complicated for now)
        // Or may be it's ok, if think about this class as 2nd lvl VM
        internal static ReactiveProperty<int> WindowModeRp_2WayIntReflection { get; } = new ReactiveProperty<int>();
        
        internal static ReactiveProperty<ResolutionShort> ResolutionRp { get; } = new ReactiveProperty<ResolutionShort>() { Value = DfltResolution };
        internal static ReactiveProperty<int> ResolutionRp_2WayIntReflection { get; } = new ReactiveProperty<int>();
        
        internal static ReactiveProperty<bool> VSyncRp { get; } = new ReactiveProperty<bool>() { Value = DfltVSync };
        private static MonoBehaviour _coroutineOwner; ///##DIFF

        internal static void Init(MonoBehaviour coroutineOwner)
        {
            if (MaxResolution_Int < 1)
                Logger.Error($"#IMPOSSIBLE: MaxResolution_Int < 1 ({MaxResolution_Int}). Resolutions.N: {Resolutions.Length}." +
                             $"\nResolutions: {Resolutions.ItemsToStringByLines()}");
            FullscreenModeLocalizationKeys.AssertIfNull(nameof(FullscreenModeLocalizationKeys));
            coroutineOwner.AssertIfNull(nameof(coroutineOwner));
            _coroutineOwner = coroutineOwner;

            // Bind `WindowModeRp` to `_int` & back:
            WindowModeRp.Action(D, x =>
            {
                if (DbgLog.Enabled) DbgLog.Log(16631, $"#*** GrSe:: WindowModeRp --> _2WayIntReflection:  {(WindowModeRp_2WayIntReflection.HasValue ? WindowModeRp_2WayIntReflection.Value.ToString() : "no_val")}  -->  {x}({(int)x})");
                if (!WindowModeRp_2WayIntReflection.HasValue || WindowModeRp_2WayIntReflection.Value != (int)x) 
                    WindowModeRp_2WayIntReflection.Value = (int)x;
            });

            WindowModeRp_2WayIntReflection.Action(D, x =>
            {
                if (DbgLog.Enabled) DbgLog.Log(16631, $"#*** GrSe:: _2WayIntReflection --> WindowModeRp:  {(WindowModeRp.HasValue ? WindowModeRp.Value.ToString() : "no_val")}  -->  {x}({(FullScreenModeShort)x})");
                if (WindowModeRp.HasValue && x == (int) WindowModeRp.Value)
                    return;

                if (TryIntToFullScreenModeShort(x, out var fsMode))
                    WindowModeRp.Value = fsMode;
                else
                    Logger.IfError()?.Message($"!IsInFullScreenModeRange({x})").Write();
            });

            // Bind `ResolutionRp` to `_int` & back:
            ResolutionRp.Action(D, x =>
            {
                var indx = GetResolutionIndex(x);
                if (!IsResolutionIndexInRange(indx))
                    Logger.Error($"GetResolutionIndex({x}) returned indx {indx} - is out of range. (Resolutions.N=={Resolutions.Length})." +
                                 $"\n Resolutions: {Resolutions.ItemsToStringByLines()}");
                else if (DbgLog.Enabled) DbgLog.Log(16631, $"#*** GrSe:: ResolutionRp --> _2WayIntReflection:  {(ResolutionRp_2WayIntReflection.HasValue ? ResolutionRp_2WayIntReflection.Value.ToString() : "no_val")}  -->  {indx}({Resolutions[indx]} ({x}))");

                if (!ResolutionRp_2WayIntReflection.HasValue || ResolutionRp_2WayIntReflection.Value != indx)
                    ResolutionRp_2WayIntReflection.Value = indx; //#Danger: could  be -1 (is it ok?)
            });

            ResolutionRp_2WayIntReflection.Action(D, x =>
            {
                if (DbgLog.Enabled) DbgLog.Log(16631, $"#*** GrSe:: _2WayIntReflection --> ResolutionRp:  {(ResolutionRp.HasValue ? ResolutionRp.Value.ToString() : "no_val")}  -->  {Resolutions[x]}({x})");
                if (IsResolutionIndexInRange(x) && (!ResolutionRp.HasValue || (x >= 0 && !Resolutions[x].Equals(ResolutionRp.Value))))
                    ResolutionRp.Value = Resolutions[x];
            });

            //Единственный раз читаем сохраненные значения
            WindowModeRp.Value = SavedWindowMode;
            ResolutionRp.Value = SavedResolution;
            VSyncRp     .Value = SavedVSync;

            //Связываем изменения значений Rp с 1) изменением громкости в движке, 2) сохранением в UniquePlayerPrefs (реестре)
            WindowModeRp.Action(D, val =>
            {
                if (DbgLog.Enabled) DbgLog.Log (16631, $"#*** GrSe:: WindowModeRp --> Saved:  {SavedWindowMode}  -->  {val}");
                SavedWindowMode = val;
                AskSetWindowMode(val);
            });

            ResolutionRp.Action(D, val =>
            {
                if (DbgLog.Enabled) DbgLog.Log (16631, $"#*** GrSe:: ResolutionRp --> Saved:  {SavedResolution}  -->  {val}");
                SavedResolution = val;
                AskSetResolution(val);
            });

            VSyncRp.Action(D, val =>
            {
                if (DbgLog.Enabled) DbgLog.Log (16631, $"#*** GrSe:: VSyncRp --> Saved:  {SavedVSync}  -->  {val}");
                SavedVSync = val;
                SetVSync(val);
            });

            // if we got requests before coud start setting-coroutine
            if (_askedResolution.HasValue || _askedWindowMode.HasValue)
                RequestDoSetting();

            StartSettingResolutionAndWindowModeByFactualCoroutine();
        }

        // Coroutine `SetResolutionAndWindowModeByFactual` is working:
        private static bool IsSettingByFactual;
        static void StartSettingResolutionAndWindowModeByFactualCoroutine()
        {
            if (IsSettingByFactual)
                return;
            IsSettingByFactual = true;
            _coroutineOwner.StartCoroutine(SetResolutionAndWindowModeByFactual());
        }

        private static bool spamPreventer;
        static IEnumerator SetResolutionAndWindowModeByFactual()
        {
            if (DbgLog.Enabled) DbgLog.Log(16631, $"х__х  #*** A. GrSe:: SetResolutionAndWindowModeByFactual:  " +
                                                  $"{(WindowModeRp.HasValue ? WindowModeRp.Value.ToString() : "no_val")} --> {Screen.fullScreenMode}, " +
                                                  $"{(ResolutionRp.HasValue ? ResolutionRp.Value.ToString() : "no_val")} --> {Screen.currentResolution}");

            yield return null; //Skip 1 frame;

            var fsm = Screen.fullScreenMode;
            if (fsm == FullScreenMode.MaximizedWindow && !spamPreventer)
            {
                Logger.Warn($"Unexpected {nameof(Screen.fullScreenMode)} == maximizedWindow({fsm}) (on Windows).");
                spamPreventer = true;
            }

            var resol = new ResolutionShort(Screen.currentResolution);
            bool isWindow = fsm == FullScreenMode.Windowed;
            var fsmShort = FullscreenModeToShort(fsm);
            
            //#Dbg:
            if (WindowModeRp.HasValue && WindowModeRp.Value != fsmShort) 
                Logger.Error($"GrSe=( :: WindowModeRp.V != Screen.fullScreenMode : {WindowModeRp.Value} != {fsm}({fsmShort})");
            if (!isWindow && ResolutionRp.HasValue && !ResolutionRp.Value.Equals(resol))
                Logger.Error($"GrSe=( :: ResolutionRp.V != currResol : {ResolutionRp.Value} != {resol}");

            WindowModeRp.Value = fsmShort;
            if (!isWindow) //'cos Screen.currentResolution returns monitor resol., but ResolutionRp is responsible about window resol.in this case. 
                ResolutionRp.Value = resol;

            #region DBG
            if (false)
            {
                yield return null;
                DBG_Check(1);
                yield return new WaitForSeconds(2);
                DBG_Check(2);
            }
            #endregion

            Debug.Assert(IsSettingByFactual);
            IsSettingByFactual = false;

            if (DbgLog.Enabled) DbgLog.Log(16631, $"х__х  #*** Z. GrSe:: SetResolutionAndWindowModeByFactual:  " +
                                                  $"{(WindowModeRp.HasValue ? WindowModeRp.Value.ToString() : "no_val")} ?== {fsm}({fsmShort}), " +
                                                  $"{(ResolutionRp.HasValue ? ResolutionRp.Value.ToString() : "no_val")} ?== {Screen.currentResolution}");
        }

        //#Dbg:
        static void DBG_Check(int N)
        {
            var resol = new ResolutionShort(Screen.currentResolution);
            bool isWindow = Screen.fullScreenMode == FullScreenMode.Windowed;

            if (WindowModeRp.HasValue && WindowModeRp.Value != FullscreenModeToShort(Screen.fullScreenMode))
                Logger.Error($"DBG_Check[{N}] =( :: WindowModeRp.V != Screen.fullScreenMode : {WindowModeRp.Value} != {Screen.fullScreenMode}({FullscreenModeToShort(Screen.fullScreenMode)})");
            if (!isWindow && ResolutionRp.HasValue && !ResolutionRp.Value.Equals(resol))
                Logger.Error($"DBG_Check[{N}] =( :: ResolutionRp.V != currResol : {ResolutionRp.Value} != {resol}");
        }

        internal static void ShutDown()
        {
            D.Clear(); //Убирает все связи с ReactiveProp<>
        }


        // --- Privates: ----------------------------------------------

        private static FullScreenModeShort SavedWindowMode
        {
            get
            {
                var str = UniquePlayerPrefs.GetString(WindowModeID, string.Empty);
                if (!Enum.TryParse(str, out FullScreenModeShort result))
                    result = DfltWindowMode;
                return result;
            }
            set => UniquePlayerPrefs.SetString(WindowModeID, value.ToString());
        }
        private static ResolutionShort SavedResolution
        {
            get
            {
                var w = UniquePlayerPrefs.GetInt(ResolutionWidthID , DfltResolution.Width);
                var h = UniquePlayerPrefs.GetInt(ResolutionHeightID, DfltResolution.Height);
                //var refreshRate = UniquePlayerPrefs.GetInt(RefreshRateID     , DfltResolution.refreshRate);
                return new ResolutionShort() { Width = w, Height = h };
            }
            set
            {
                //#Dbg:
                if (false)
                {
                    var w = UniquePlayerPrefs.GetInt(ResolutionWidthID, DfltResolution.Width);
                    var h = UniquePlayerPrefs.GetInt(ResolutionHeightID, DfltResolution.Height);
                    var was = new ResolutionShort() {Width = w, Height = h};
                    if (DbgLog.Enabled) DbgLog.Log(16631, $"SavedResolution_SET: {was} --> {value}.");
                }
                
                UniquePlayerPrefs.SetInt(ResolutionWidthID,  value.Width);
                UniquePlayerPrefs.SetInt(ResolutionHeightID, value.Height);
                //UniquePlayerPrefs.SetInt(RefreshRateID,      value.refreshRate);
            }
        }
        private static bool SavedVSync
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            get => UniquePlayerPrefs.GetBool(VSyncID, DfltVSync);
            set => UniquePlayerPrefs.SetBool(VSyncID, value);
        }

        private static void SetVSync(bool val)
        {
            if (DbgLog.Enabled) DbgLog.Log(16631, $"#*** GrSe:: A. SetVSync: {"?"} --> {val}");

            QualitySettings.vSyncCount = val ? 1 : 0; //Possible valued 
            Logger.IfInfo()?.Message($"{nameof(SetVSync)}({val}).").Write();
            //#Dbg
            //if (Dbg_ReportResults && DbgLog.Enabled) DbgLog.Log($"{nameof(SetVSync)}({val}).");

            if (DbgLog.Enabled) DbgLog.Log(16631, $"#*** GrSe:: Z. SetVSync: {"?"} --> {val}");
        }

        private static int GetResolutionIndex(ResolutionShort r)
        {
            return Resolutions.IndexOf(r);
        }

        internal static LocalizedString WindowModeToLocalizedString(int indx)
        {
            if (TryIntToFullScreenModeShort(indx, out var fsMode))
                return GetLS(fsMode);
            else
            {
                Logger.IfError()?.Message($"!IsInFullScreenModeRange({indx})").Write();
                return LsExtensions.Empty;
            }
        }
        private static bool IsResolutionIndexInRange(int indx)
        {
            return MaxResolution_Int >= 0 && SharedHelpers.InRange(indx, 0, MaxResolution_Int);
        }
        internal static string ResolutionToString(int indx)
        {
            return (IsResolutionIndexInRange(indx))
                ? ResolutionToString(Resolutions[indx])
                : string.Empty;
        }
        internal static string ResolutionToString(ResolutionShort r)
        {
            return $"{r.Width} x {r.Height}";
        }

        private static LocalizedString GetLS(FullScreenModeShort mode)
        {
            if (FullscreenModeLocalizationKeys?.LocalizedStrings.TryGetValue(mode.ToString(), out var nameLs) ?? false)
                return nameLs;
            else
                return LsExtensions.Empty;
        }

        static bool IsInFullScreenModeRange(int i) => i >= 0 && i <= MaxWindowMode_Int;

        static bool TryIntToFullScreenModeShort(int i, out FullScreenModeShort fsmShort)
        {
            fsmShort = FullScreenModeShort.Windowed; //assign by dflt
            if (!IsInFullScreenModeRange(i))
            {
                Logger.IfError()?.Message($"!IsInFullScreenModeRange({i})").Write();
                return false;
            }
            fsmShort = (FullScreenModeShort)i;
            return true;
        }

    #region Set resolution & window mode requests + requests proceeding coroutine
        private static ResolutionShort? _askedResolution;
        private static FullScreenModeShort? _askedWindowMode;
        // Resolution & windowMode
        private static bool _isSettingResolutionWindMode;
        private static void AskSetResolution(ResolutionShort r)
        {
            _askedResolution = r;
            RequestDoSetting();
        }
        private static void AskSetWindowMode(FullScreenModeShort val)
        {
            _askedWindowMode = val;
            RequestDoSetting();
        }

        static void RequestDoSetting()
        {
            if (!_isSettingResolutionWindMode)
                _coroutineOwner?.StartCoroutine(SettingResolutoinAndWindowModeCoroutine());
            else
            {
                //_newAskResolutionWhenSettingIsInProgress = true;
                // do nothing - coroutine is already works & 'll set requested value.
            }
        }

        static IEnumerator SettingResolutoinAndWindowModeCoroutine()
        {
            // if (DbgLog.Enabled) DbgLog.Log(16631, $"SettingResolutoinAndWindowModeCoroutine: ({Screen.currentResolution} / {Screen.fullScreenMode}) --> " +
            //                                $"{(_askedResolution.HasValue ? _askedResolution.Value.ToString() : "no_val")} / " +
            //                                $"{(_askedWindowMode.HasValue ? _askedWindowMode.Value.ToString() : "no_val")}");

            Debug.Assert(!_isSettingResolutionWindMode);
            _isSettingResolutionWindMode = true;

            yield return null; //wait 1 frame to let all simultanious requests be heared

            Debug.Assert(_askedResolution.HasValue || _askedWindowMode.HasValue);

            if (DbgLog.Enabled) DbgLog.Log(16631, $"SettingResolutoinAndWindowModeCoroutine: ({Screen.currentResolution} / {Screen.fullScreenMode}) --> " +
                                           $"{(_askedResolution.HasValue ? _askedResolution.Value.ToString() : "no_val")} / " +
                                           $"{(_askedWindowMode.HasValue ? _askedWindowMode.Value.ToString() : "no_val")}");

            // Case that need special handling, 'cos of Unity underhood (probably) _bug of simultanious change:
            // `ExclusiveFullscr` to `FullscrWind` && resolution up:
            // We'll set them one-by-one with 1 frame pause.
            bool isSpecialCase = _askedResolution.HasValue
                              && _askedWindowMode.HasValue
                              && _askedWindowMode.Value == FullScreenModeShort.FullScreenWindow
                              && Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen
                              && (GetResolutionIndex(_askedResolution.Value) > GetResolutionIndex(new ResolutionShort(Screen.currentResolution)));

            // if (isSpecialCase)
            //     if (DbgLog.Enabled) DbgLog.Log("#Dbg: isSpecialCase");

            // Consume requests:
            var res = _askedResolution.HasValue
                ? _askedResolution.Value
                : (ResolutionRp.HasValue ? ResolutionRp.Value : new ResolutionShort(Screen.currentResolution));
            _askedResolution = null;
           
            // if a Special case:
            if (isSpecialCase)
            {
                Screen.SetResolution(res.Width, res.Height, Screen.fullScreenMode);
                yield return null; //wait 1 frame
                Debug.Assert(res.Equals(new ResolutionShort(Screen.currentResolution)));
                if (DbgLog.Enabled) DbgLog.Log(16631, $"SettingRes-tion_DO [isSpecialCase Mid]: ({Screen.currentResolution} / {Screen.fullScreenMode})");
            }

            var winMode = _askedWindowMode.HasValue
                ? FullscreenModeFromShort(_askedWindowMode.Value)
                : (WindowModeRp.HasValue ? FullscreenModeFromShort(WindowModeRp.Value) : Screen.fullScreenMode);
            _askedWindowMode = null;

            if (DbgLog.Enabled) DbgLog.Log(16631, $"SettingRes-tion_DO: ({Screen.currentResolution} / {Screen.fullScreenMode}) --> " +
                                           $"{res} / " +
                                           $"{winMode}");

            Screen.SetResolution(res.Width, res.Height, winMode);
            Debug.Assert(_isSettingResolutionWindMode);
            _isSettingResolutionWindMode = false;

            StartSettingResolutionAndWindowModeByFactualCoroutine();
        }
    #endregion Set resolution & window mode requests + requests proceeding coroutine


        // --- Util types: ---------------------------------
        // A subset of Unity.FullScreenMode
        internal enum FullScreenModeShort
        {
            ///#IMPORTANT: !!! SHOULD BE MANUALLY SYNCRONIZED WITH LocalizationKeysDef resource jdb: `L10n_Settings_WindowMode_enum.jdb` !!!

            ExclusiveFullScreen,
            FullScreenWindow,
            //MaximizedWindow, - not work on Windows - `FullScreenWindow` is factually set when setting this one
            Windowed,
        }

        internal static FullScreenModeShort FullscreenModeToShort(FullScreenMode fsm)
        {
            switch (fsm)
            {
                case FullScreenMode.ExclusiveFullScreen: return FullScreenModeShort.ExclusiveFullScreen;
                case FullScreenMode.FullScreenWindow:    return FullScreenModeShort.FullScreenWindow;
                case FullScreenMode.MaximizedWindow:     return FullScreenModeShort.FullScreenWindow; // Ok MW is FSW
                case FullScreenMode.Windowed:            return FullScreenModeShort.Windowed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fsm), fsm, null);
            }
        }
        internal static FullScreenMode FullscreenModeFromShort(FullScreenModeShort fsmShort)
        {
            switch (fsmShort)
            {
                case FullScreenModeShort.ExclusiveFullScreen: return FullScreenMode.ExclusiveFullScreen;
                case FullScreenModeShort.FullScreenWindow:    return FullScreenMode.FullScreenWindow;
                case FullScreenModeShort.Windowed:            return FullScreenMode.Windowed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fsmShort), fsmShort, null);
            }
        }

        internal struct ResolutionShort
        {
            internal int Width;
            internal int Height;

            internal ResolutionShort(Resolution r)
            {
                Width = r.width;
                Height = r.height;
            }

            internal ResolutionShort(int w, int h)
            {
                Width = w;
                Height = h;
            }

            public override string ToString()
            {
                return $"{Width} x {Height}";
            }
        }
    }
}
