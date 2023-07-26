using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using L10n;
using ReactivePropsNs;
using Src.Input;
using Uins.Sound;

namespace Uins.Settings
{
    // #note: Expected lifetime - infinite
    public class SettingsGuiWindowVM : BindingVmodel
    {
        // возможно уже не используется - проверить по onewaypropbinding компонентам всего дерева в иерархии SettingsGuiWindow
        public ReactiveProperty<bool> IsActive = new ReactiveProperty<bool>();
        // Rp текущего выбранного раздела setting'ов.
        //   Оригинал лежит в `SettingsGuiWindowVM`,
        //   передаётся паре (Ctrl-VM) классов SettingsGroupButton, чтобы оттуда вещёлось в этот стрим (по нажатию кнопки в Rp := Vmodel.Value).    ///assing 1of2
        //   Также передаётся паре `SetOfSettings` - там:
        //    a) слушается и трансформируется в ListStream `SettingHandlerDefinitions`ов,
        //      который подключен к pool'у (тупому без реюза) параметризованному парой `SettingsListElem`.
        //    б) вещается в него `null`, когда нажата кнопка "Back" (так очищается список setting'ов)                                               ///assign 2of2
        public ReactiveProperty<SettingsGroupButtonVM> CurrentGroup = new ReactiveProperty<SettingsGroupButtonVM>();

        private static readonly LocalizationKeyPairsDef OnOffLocalizationKeyPairsDef = GlobalConstsHolder.GlobalConstsDef.LocalizationKeysDefsHolder.Target?.OnOffLocalizationKeyPairsDef.Target;

        // All specific logic lies here

        internal ListStream<(SettingsGroup SettingsGroup, List<SettingHandlerDefinition> SettingHandlerDefinitions)> ContentDefinition//;
                = new ListStream<(SettingsGroup, List<SettingHandlerDefinition>)>()
                {
                    // GENERAL: 
                    (SettingsGroup.General, new List<SettingHandlerDefinition>()
                    {
                        //#Order is important!: #1of2 //Т.к. apply'ится в этом же порядке в foreach, Unity глючит, если в обратном порядке и Ок, если так.  //Если перестанет работать, см. "//#todo:" в `SettingsGuiWindowVM`
                        new SettingHandlerDefinition(SettingsNames.Resolution,      SettingHandlerType.Int, new SettingSwitcherIntVM.Definition()
                        {
                            DfltVal = GraphicSettingsViewModel.DfltResolution_Int,
                            MaxVal  = GraphicSettingsViewModel.MaxResolution_Int,
                            InStream = GraphicSettingsViewModel.ResolutionRp_2WayIntReflection,
                            OutRp    = GraphicSettingsViewModel.ResolutionRp_2WayIntReflection,
                            AppliedInstantly = false,
                            ValToStringConverter = GraphicSettingsViewModel.ResolutionToString,
                        }),
                        //#Order is important!: #2of2 //Т.к. apply'ится в этом же порядке в foreach, Unity глючит, если в обратном порядке и Ок, если так.  //Если перестанет работать, см. "//#todo:" в `SettingsGuiWindowVM`
                        new SettingHandlerDefinition(SettingsNames.WindowMode,      SettingHandlerType.Int, new SettingSwitcherIntVM.Definition()
                        {
                            DfltVal = GraphicSettingsViewModel.DfltWindowMode_Int,
                            MaxVal  = GraphicSettingsViewModel.MaxWindowMode_Int,
                            InStream = GraphicSettingsViewModel.WindowModeRp_2WayIntReflection,
                            OutRp    = GraphicSettingsViewModel.WindowModeRp_2WayIntReflection,
                            AppliedInstantly = false,
                            ValToLocalizedStringConverter = GraphicSettingsViewModel.WindowModeToLocalizedString,
                        }),
                        new SettingHandlerDefinition(SettingsNames.Quality,      SettingHandlerType.Int, new SettingSwitcherIntVM.Definition()
                        {
                            DfltVal = QualitySimpleSetterViewModel.DfltQualityLvl_Int,
                            MaxVal  = QualitySimpleSetterViewModel.MaxQualityLvl_Int,
                            InStream = QualitySimpleSetterViewModel.QualityLvlRp_2WayIntReflection,
                            OutRp    = QualitySimpleSetterViewModel.QualityLvlRp_2WayIntReflection,
                            AppliedInstantly = false,
                            ValToLocalizedStringConverter = QualitySimpleSetterViewModel.QualityLvlToLocalizedString,
                        }),
                        new SettingHandlerDefinition(SettingsNames.VSync,         SettingHandlerType.Bool, new SettingSwitcherBoolVM.Definition()
                        {
                            DfltVal = GraphicSettingsViewModel.DfltVSync,
                            InStream = GraphicSettingsViewModel.VSyncRp,
                            OutRp    = GraphicSettingsViewModel.VSyncRp,
                            AppliedInstantly = false,
                            ValToLocalizedStringConverter = BoolToOnOff,
                        }),
                        new SettingHandlerDefinition(SettingsNames.Language,        SettingHandlerType.Int, new SettingSwitcherIntVM.Definition()
                        {
                            DfltVal = (int)LanguageSetting.DefaultLanguage,
                            MaxVal  = (int)LanguageSetting.Languages.Max,
                            InStream = LanguageSetting.LanguageRp_2WayIntReflection,
                            OutRp    = LanguageSetting.LanguageRp_2WayIntReflection,
                            AppliedInstantly = false,
                            ValToLocalizedStringConverter = LanguageSetting.LanguageToLocalizedString,
                        }),
                        new SettingHandlerDefinition(SettingsNames.KeyboardLayout,  SettingHandlerType.Int, new SettingSwitcherIntVM.Definition()
                        {
                            DfltVal = (int)InputManagerViewModel.DfltKeyboardLayout,
                            MaxVal  = (int)KeyboardLayout.Max,
                            InStream = InputManagerViewModel.KeyboardLayoutRp_2WayIntReflection,
                            OutRp    = InputManagerViewModel.KeyboardLayoutRp_2WayIntReflection,
                            AppliedInstantly = false,
                            ValToStringConverter = InputManagerViewModel.KeyboardLayoutToString,
                        }),
                        new SettingHandlerDefinition(SettingsNames.MouseSensivity,    SettingHandlerType.Float, new SettingSwitcherFloatVM.Definition()
                        {
                            DfltVal = ControlsSettingsViewModel.DfltIsMouseSensivity,
                            MaxVal = ControlsSettingsViewModel.MaxMouseSensivity,
                            InStream = ControlsSettingsViewModel.MouseSensivityRp,
                            OutRp    = ControlsSettingsViewModel.MouseSensivityRp,
                            StreamToValTransformer = val => ToRatio2Borders(val, ControlsSettingsViewModel.MinMouseSensivity, ControlsSettingsViewModel.MaxMouseSensivity),
                            ValToRpTransformer = ratio => OnSomeSliderChanged2Borders(ratio, ControlsSettingsViewModel.MouseSensivityRp, 
                                                                                             ControlsSettingsViewModel.MinMouseSensivity, 
                                                                                             ControlsSettingsViewModel.MaxMouseSensivity),
                            AppliedInstantly = false,
                            ValToStringConverter = FloatToString1Fractional,
                        }),
                        new SettingHandlerDefinition(SettingsNames.MouseInversion,  SettingHandlerType.Bool, new SettingSwitcherBoolVM.Definition()
                        {
                            DfltVal = ControlsSettingsViewModel.DfltIsMouseInverted,
                            InStream = ControlsSettingsViewModel.MouseInversionRp,
                            OutRp    = ControlsSettingsViewModel.MouseInversionRp,
                            AppliedInstantly = false,
                            ValToLocalizedStringConverter = BoolToOnOff,
                        }),

                    }),

                    // AUDIO: 
                    (SettingsGroup.Audio,   new List<SettingHandlerDefinition>()
                    {
                        new SettingHandlerDefinition(SettingsNames.MuteAll,         SettingHandlerType.Bool, new SettingSwitcherBoolVM.Definition()
                        {
                            DfltVal = false,
                            InStream = SoundControl.Instance.IsMutedRp,
                            OutRp    = SoundControl.Instance.IsMutedRp,
                            AppliedInstantly = true,
                            ValToLocalizedStringConverter = BoolToOnOffInversed,
                        }),
                        new SettingHandlerDefinition(SettingsNames.MasterVolume,    SettingHandlerType.Float, new SettingSwitcherFloatVM.Definition()
                        {
                            DfltVal  = SoundControl.DefaultVolume, 
                            MaxVal   = SoundControl.MaxVolume, 
                            InStream = SoundControl.Instance.MasterVolumeRp,
                            OutRp    = SoundControl.Instance.MasterVolumeRp,
                            StreamToValTransformer = val => ToRatio(val, SoundControl.MaxVolume),
                            ValToRpTransformer = ratio => OnSomeSliderChanged(ratio, SoundControl.Instance.MasterVolumeRp, SoundControl.MaxVolume),
                            AppliedInstantly = true,
                            ValToStringConverter = FloatToString_asInt,
                        }),
                        new SettingHandlerDefinition(SettingsNames.MusicVolume,     SettingHandlerType.Float, new SettingSwitcherFloatVM.Definition()
                        {
                            DfltVal  = SoundControl.DefaultVolume,
                            MaxVal   = SoundControl.MaxVolume,
                            InStream = SoundControl.Instance.MusicVolumeRp,
                            OutRp    = SoundControl.Instance.MusicVolumeRp,
                            StreamToValTransformer = val => ToRatio(val, SoundControl.MaxVolume),
                            ValToRpTransformer = ratio => OnSomeSliderChanged(ratio, SoundControl.Instance.MusicVolumeRp, SoundControl.MaxVolume),
                            AppliedInstantly = true,
                            ValToStringConverter = FloatToString_asInt,
                        }),
                        new SettingHandlerDefinition(SettingsNames.EffectsVolume,   SettingHandlerType.Float, new SettingSwitcherFloatVM.Definition()
                        {
                            DfltVal  = SoundControl.DefaultVolume,
                            MaxVal   = SoundControl.MaxVolume,
                            InStream = SoundControl.Instance.SfxVolumeRp,
                            OutRp    = SoundControl.Instance.SfxVolumeRp,
                            StreamToValTransformer = val => ToRatio(val, SoundControl.MaxVolume),
                            ValToRpTransformer = ratio => OnSomeSliderChanged(ratio, SoundControl.Instance.SfxVolumeRp, SoundControl.MaxVolume),
                            AppliedInstantly = true,
                            ValToStringConverter = FloatToString_asInt,
                        }),
                    }),
                };

        private static (bool, float) OnSomeSliderChanged2Borders(float ratio, ReactiveProperty<float> reactiveProperty, float minVal, float maxVal)
        {
            var value = minVal + ratio * (maxVal - minVal);
            return (Math.Abs(reactiveProperty.Value - value) > float.Epsilon)
                ? (true, value)
                : (false, default);
        }

        private static (bool, float) OnSomeSliderChanged(float ratio, ReactiveProperty<float> reactiveProperty, float maxVal) =>
            OnSomeSliderChanged2Borders(ratio, reactiveProperty, 0, maxVal);

        private static float ToRatio2Borders(float val, float minVal, float maxVal)
        {
            return (maxVal > 0 && minVal < maxVal)
                ? (val - minVal) / (maxVal - minVal)
                : default;
        }
        private static float ToRatio(float val, float maxVal)
        {
            return (maxVal > 0)
                ? val / maxVal
                : default;
        }

        private static string FloatToString_asInt(float f) => f.ToString("##");
        private static string FloatToString1Fractional(float f) => f.ToString("0.0");

        private static LocalizedString BoolToOnOff(bool b) => OnOffLocalizationKeyPairsDef?.GetLS(b) ?? LsExtensions.Empty;
        private static LocalizedString BoolToOnOffInversed(bool b) => BoolToOnOff(!b);

    }


    // --- Util types: ------------------------

    ///#todo: Прикрутить сверку ресурсов `L10n_Settings_..._enum.jdb` с соотв-щими enum'ами. Коль подвязались на соответствие, нужно чётко контролировать, что не разошлось. Пока таких 2 тут, 1 `LanguageSetting.Languages` (//есть уже ещё вроде можно найти по комменту внутри jdb и enum'а) и 1 юнитёвский FullscreenMode 

    public enum SettingsGroup
    {
        ///#IMPORTANT: !!! SHOULD BE MANUALLY SYNCRONIZED WITH LocalizationKeysDef resource jdb: `L10n_Settings_Groups_enum.jdb` !!!

        None,
        General,
        Audio,
    }

    public enum SettingsNames
    {
        ///#IMPORTANT: !!! SHOULD BE MANUALLY SYNCRONIZED WITH LocalizationKeysDef resource jdb: `L10n_Settings_Names_enum.jdb` !!!

        None,
        WindowMode,
        Resolution,
        Quality,
        VSync,
        Language,
        KeyboardLayout,
        MouseSensivity,
        MouseInversion,
        MuteAll,
        MasterVolume,
        MusicVolume,
        EffectsVolume,
    }

    internal struct SettingHandlerDefinition
    {
        internal SettingsNames Name;
        internal SettingHandlerType HandlerType;
        internal ISettingSwitcherDefinition Definition;
        internal bool IsValid => Definition != null && HandlerType != SettingHandlerType.None && Name != SettingsNames.None;

        internal SettingHandlerDefinition(SettingsNames name, SettingHandlerType handlerType, ISettingSwitcherDefinition definition)
        {
            Name = name;
            HandlerType = handlerType;
            Definition = definition;
        }

        public override string ToString()
        {
            return $"(valid?:{IsValid}) Name:{Name}, HType:{HandlerType}, def:{Definition}";
        }
    }

    internal enum SettingHandlerType
    {
        None, // Invalid
        Bool,
        Int,
        Float,
    }

    internal static class SettingsHelpers
    {
        internal static Dictionary<Type, SettingHandlerType> TypeToSettingHandlerTypeMap = new Dictionary<Type, SettingHandlerType>()
        {
            {typeof(bool),  SettingHandlerType.Bool  },
            {typeof(int),   SettingHandlerType.Int   },
            {typeof(float), SettingHandlerType.Float },
        };
    }
}
