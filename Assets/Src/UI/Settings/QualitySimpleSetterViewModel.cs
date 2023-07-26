using System;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.App.QualitySettings;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

namespace Uins.Settings
{
    public static class QualitySimpleSetterViewModel
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // Нужно будет перевести на новые рельсы, когда при мерже встретится с веткой Влада Г. 
        private static readonly LocalizationKeysDef QualityLvlLocalizationKeys = GlobalConstsHolder.GlobalConstsDef.LocalizationKeysDefsHolder.Target?.QualityLvlLocalizationKeys.Target;

        // Registry keys:
        private const string QualityLvlID = "Quality_level";

        internal const int DfltQualityLvl_Int = (int)DfltQualityLvl;
        internal static readonly int MaxQualityLvl_Int = Enum.GetValues(typeof(QualityLevels)).Length - 1;
        internal const QualityLevels DfltQualityLvl = QualityLevels.High;
        internal static DisposableComposite D = new DisposableComposite();

        internal static ReactiveProperty<QualityLevels> QualityLvlRp { get; } = new ReactiveProperty<QualityLevels>() { Value = DfltQualityLvl };
        //#todo: Think, how move UI-representation specific out from here - to UI VM! (this task looks complicated for now)
        // Or may be it's ok, if think about this class as 2nd lvl VM
        internal static ReactiveProperty<int> QualityLvlRp_2WayIntReflection { get; } = new ReactiveProperty<int>();
        
        internal static void Init()
        {
            QualityLvlLocalizationKeys.AssertIfNull(nameof(QualityLvlLocalizationKeys));

            // Bind `QualityLvlRp` to `_int` & back:
            QualityLvlRp.Action(D, x =>
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** GrSe:: QualityLvlRp --> _2WayIntReflection:  {(QualityLvlRp_2WayIntReflection.HasValue ? QualityLvlRp_2WayIntReflection.Value.ToString() : "no_val")}  -->  {x}({(int)x})");
                if (!QualityLvlRp_2WayIntReflection.HasValue || QualityLvlRp_2WayIntReflection.Value != (int)x) 
                    QualityLvlRp_2WayIntReflection.Value = (int)x;
            });

            QualityLvlRp_2WayIntReflection.Action(D, x =>
            {
                //if (DbgLog.Enabled) DbgLog.Log($"#*** GrSe:: _2WayIntReflection --> QualityLvlRp:  {(QualityLvlRp.HasValue ? QualityLvlRp.Value.ToString() : "no_val")}  -->  {x}({(QualityLevels)x})");
                if (QualityLvlRp.HasValue && x == (int) QualityLvlRp.Value)
                    return;

                if (TryIntToQualityLvl(x, out var qLvl))
                    QualityLvlRp.Value = qLvl;
                else
                    Logger.IfError()?.Message($"!IsInQualityLvlRange({x})").Write();
            });

            //Единственный раз читаем сохраненные значения
            QualityLvlRp.Value = SavedQualityLvl;

            //Связываем изменения значений Rp с 1) изменением громкости в движке, 2) сохранением в UniquePlayerPrefs (реестре)
            QualityLvlRp.Action(D, val =>
            {
                //if (DbgLog.Enabled) DbgLog.Log           ($"#*** GrSe:: QualityLvlRp --> Saved:  {SavedQualityLvl}  -->  {val}");
                SavedQualityLvl = val;
                SetQualityLvl(val);
            });

        }
        
        internal static void ShutDown()
        {
            D.Clear(); //Убирает все связи с ReactiveProp<>
        }


        // --- Privates: ----------------------------------------------

        private static QualityLevels SavedQualityLvl
        {
            get
            {
                var str = UniquePlayerPrefs.GetString(QualityLvlID, string.Empty);
                if (!Enum.TryParse(str, out QualityLevels result))
                    result = DfltQualityLvl;
                return result;
            }
            set => UniquePlayerPrefs.SetString(QualityLvlID, value.ToString());
        }

        private static void SetQualityLvl(QualityLevels val)
        {
            //if (DbgLog.Enabled) DbgLog.Log($"#*** GrSe:: A. SetQualityLvl: {QualitySimpleSetter.DBG_LastSetQLvl} --> {val}");

            QualitySimpleSetter.SetQuality(val);
            Logger.IfInfo()?.Message($"{nameof(SetQualityLvl)}({val}).").Write();

            //if (DbgLog.Enabled) DbgLog.Log($"#*** GrSe:: Z. SetQualityLvl: {QualitySimpleSetter.DBG_LastSetQLvl} --> {val}");
        }

        internal static LocalizedString QualityLvlToLocalizedString(int indx)
        {
            LocalizedString ls;
            if (TryIntToQualityLvl(indx, out var qLvl))
                /*return*/ls= GetLS(qLvl);
            else
            {
                Logger.IfError()?.Message($"!IsInQualityLvlRange({indx})").Write();
                /*return*/ls= LsExtensions.Empty;
            }
            //#Dbg:
            //if (DbgLog.Enabled) DbgLog.Log($"#QualityLvlToLocalizedString({indx}) ==> \"{res}\"");
            return ls;
        }

        private static LocalizedString GetLS(QualityLevels qLvl)
        {
            if (QualityLvlLocalizationKeys?.LocalizedStrings.TryGetValue(qLvl.ToString(), out var nameLs) ?? false)
                return nameLs;
            else
                return LsExtensions.Empty;
        }

        static bool IsInQualityLvlRange(int i) => i >= 0 && i <= MaxQualityLvl_Int;

        static bool TryIntToQualityLvl(int i, out QualityLevels qLvl) //v
        {
            qLvl = QualityLevels.High; //assign by dflt
            if (!IsInQualityLvlRange(i))
            {
                Logger.IfError()?.Message($"!IsInFullScreenModeRange({i})").Write();
                return false;
            }
            qLvl = (QualityLevels)i;
            return true;
        }

    }

}
