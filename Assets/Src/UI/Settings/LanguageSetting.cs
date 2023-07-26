using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;

namespace Uins.Settings
{
    internal static class LanguageSetting
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetLogger("LanguageSetting");

        internal const Languages DefaultLanguage = Languages.English;
        // Registry key:
        private const string LanguageID = "Language";
        private static readonly LocalizationKeysDef LanguageLocalizationKeys = GlobalConstsHolder.GlobalConstsDef.LocalizationKeysDefsHolder.Target?.LanguageLocalizationKeys.Target;

        ///#TODO: пока пересел на пашину новую локализацию быстро (сайд-эффект - осн. д-е в `LocalizationsConfig.jdb` и тут по-хорошему нужно бы подцепиться на прямую к нему попробовать. Но пока приходится синхронизировать:
        /// - его
        /// - с `L10n_Settings_Languages_enum.jdb`
        /// - и с `enum LanguageSetting.Languages`
        /// - и с dic. `LanguageSetting.LangToLocalization`
        /// Это конечно негуд и нужно пересесть нормально.
        internal enum Languages
        {
            ///#IMPORTANT: !!! SHOULD BE MANUALLY INTER-SYNCRONIZED ALL 4!:
            /// 1 - LocalizationKeysDef resource jdb: `L10n_Settings_Languages_enum.jdb` !!!
            /// 2 - `LocalizationsConfig.jdb`
            /// 3 - enum `LanguageSetting.Languages`
            /// 4 - dictionary `LanguageSetting.LangToLocalization`

            English,
            Russian,
            German,
            French,
            Italian,
            SpanishSpain,
            SpanishMexican,
            PortugueseBrazil,


            //#Important!!: Don't forget to update `Max` when add new values to the enum
            Max = PortugueseBrazil
        }

        internal static Dictionary<Languages, string> LangToLocalization = new Dictionary<Languages, string>()
        {
            ///#IMPORTANT: !!! SHOULD BE MANUALLY INTER-SYNCRONIZED ALL 4!:
            /// 1 - LocalizationKeysDef resource jdb: `L10n_Settings_Languages_enum.jdb` !!!
            /// 2 - `LocalizationsConfig.jdb`
            /// 3 - enum `LanguageSetting.Languages`
            /// 4 - dictionary `LanguageSetting.LangToLocalization`

            {Languages.English, "en-US"},
            {Languages.Russian, "ru-RU"},
            {Languages.German,  "de-DE"},
            {Languages.French,  "fr-FR"},
            {Languages.Italian, "it-IT"},
            {Languages.SpanishSpain, "es-ES"},
            {Languages.SpanishMexican, "es-MX"},
            {Languages.PortugueseBrazil, "pt-BR"},
        };

        internal static ReactiveProperty<Languages> LanguageRp { get; } = new ReactiveProperty<Languages>() { Value = DefaultLanguage };
        internal static ReactiveProperty<int> LanguageRp_2WayIntReflection { get; } = new ReactiveProperty<int>();

        internal static DisposableComposite D = new DisposableComposite();


        // --- API: ----------------------------------

        internal static void Init()
        {
            // Bind `LanguageRp` to `_int` & back:
            LanguageRp.Action(D, x =>
            {
                if (!LanguageRp_2WayIntReflection.HasValue || LanguageRp_2WayIntReflection.Value != (int)x)
                    LanguageRp_2WayIntReflection.Value = (int)x;
            });

            LanguageRp_2WayIntReflection.Action(D, x =>
            {
                if (!LanguageRp.HasValue || x != (int)LanguageRp.Value)
                    LanguageRp.Value = (Languages)x;
            });

            //Единственный раз читаем сохраненные значения
            LanguageRp.Value = SavedLanguage;

            //Связываем изменения значений Rp с 1) изменением значения локали в движке, 2) сохранением в UniquePlayerPrefs (реестре)
            LanguageRp.Action(D, val =>
            {
                SavedLanguage = val;
                SetLanguage(val);
            });

            LanguageRp.Action(D, val =>
            {
                SavedLanguage = val;
                SetLanguage(val);
            });
        }

        internal static void ShutDown()
        {
            D.Clear(); //Убирает все связи с ReactiveProp<>
        }

        internal static LocalizedString LanguageToLocalizedString(int indx)
        {
            /*return*/var res = (indx >= 0 && indx <= (int)Languages.Max)
                ? GetLs((Languages)indx)
                : LsExtensions.Empty;
            //#Dbg:
            //if (DbgLog.Enabled) DbgLog.Log($"#LanguageToLocalizedString({indx}) ==> \"{res}\"");
            return res;
        }


        // --- Privates: ----------------------------------------------

        private static Languages SavedLanguage
        {
            get
            {
                var str = UniquePlayerPrefs.GetString(LanguageID, string.Empty);
                if (!Enum.TryParse(str, out Languages result))
                    result = DefaultLanguage;
                return result;
            }
            set => UniquePlayerPrefs.SetString(LanguageID, value.ToString());
        }

        private static void L10nHolderInited()
        {
            L10nHolder.L10nHolderInited -= L10nHolderInited;
            if (_setLanguageLastRequestBeforeInited.HasValue)
                SetLanguageDo(_setLanguageLastRequestBeforeInited.Value);
            else
                Logger.IfError()?.Message($"{nameof(L10nHolderInited)} subscription is called, but `{nameof(_setLanguageLastRequestBeforeInited)}` == null").Write();
        }
        private static Languages? _setLanguageLastRequestBeforeInited;
        private static void SetLanguage(Languages val)
        {
            if (L10nHolder.Instance != null)
                SetLanguageDo(val);

            // if did not subscribe yet:
            if (!_setLanguageLastRequestBeforeInited.HasValue)
                L10nHolder.L10nHolderInited += L10nHolderInited;
         
            // Remember last request
            _setLanguageLastRequestBeforeInited = val;
        }
        private static void SetLanguageDo(Languages val)
        {
            if (L10nHolder.Instance.AssertIfNull(nameof(L10nHolder)))
                return;

            if (!LangToLocalization.TryGetValue(val, out var localization))
            {
                Logger.IfError()?.Message($"{nameof(LangToLocalization)} doesn't contain key '{val}'").Write();
                return;
            }

            // if (!L10nHolder.Instance.AvailableCultureNames.Contains(localization))
            // {
            //     Logger.IfError()?.Message($"{nameof(L10nHolder.Instance.AvailableCultureNames)} doesn't contain '{localization}'").Write();
            //     return;
            // }

            if (L10nHolder.Instance.AvailableCultures.All(x => x.Code != localization))
            {
                Logger.IfError()?.Message($"{nameof(L10nHolder.Instance.AvailableCultures)} doesn't contain '{localization}' code").Write();
                return;
            }

            L10nHolder.Instance.ChangeLocalization(localization);

            Logger.IfInfo()?.Message($"{nameof(SetLanguage)}({val}({localization})).").Write();
            //#Dbg
            //if (DbgLog.Enabled) DbgLog.Log($"{nameof(SetLanguage)}({val}({localization})).");
        }


        private static LocalizedString GetLs(Languages lang)
        {
            if (LanguageLocalizationKeys?.LocalizedStrings.TryGetValue(lang.ToString(), out var nameLs) ?? false)
                return nameLs;
            else
                return LsExtensions.Empty;
        }
    }
}
