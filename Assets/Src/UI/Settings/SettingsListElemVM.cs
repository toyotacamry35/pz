using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;

namespace Uins.Settings
{
    /// <summary>
    /// Эта пара Ctrl и VM классов лишь обёртка-посредник. Нужна только для того, чтобы можно было сложить разнотипные хэндлеры (Int, Bool, Float, ..) в `BindingControllersPool`.
    /// </summary>
    public class SettingsListElemVM : BindingVmodel
    {
        //#Dbg:
        // static int DBG_counter_static = 0;
        // internal int DBG_counter;

        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal ReactiveProperty<SettingHandlerDefinition> Definition = new ReactiveProperty<SettingHandlerDefinition>();
        public IStream<LocalizedString> Name;
        private LocalizationKeysDefRef _settingsNamesLocalizationKeys;

        internal SettingsListElemVM(SettingHandlerDefinition definition, LocalizationKeysDefRef settingsNamesLocalizationKeys)
        {
            // var prev = DBG_counter;
            // DBG_counter = DBG_counter_static++;
            // if (DbgLog.Enabled) Debug.Log($"VM.ctor: {prev} --> <{DBG_counter}>");

            _settingsNamesLocalizationKeys = settingsNamesLocalizationKeys;
            _settingsNamesLocalizationKeys?.Target.AssertIfNull(nameof(_settingsNamesLocalizationKeys));
            Definition.Value = definition;
            Name = Definition
                    //.Log(D, "SeLiEl_VM(0)", def => $"{def} ({def.Name})")
                    .Func(D, def =>
                    {
                       var ls = GetLs(def.Name);
                       // if (DbgLog.Enabled) Debug.Log($"Name=Def.Func: <{DBG_counter}> Name:{Name},  def.Name:{def.Name},  {ls.ToStr()}");
                       return ls;
                    });
                    //.Log(D, "SeLiEl_VM(1)", ls => $"({Name} -> {ls} (k:{ls.Key}, td:{ls.TranslationData}, txt:{ls.GetText()}))");
            //Name.DeepLog("SeLiElVM.Name_DeepLog");
        }

        private LocalizedString GetLs(SettingsNames name)
        {
            if (_settingsNamesLocalizationKeys?.Target?.LocalizedStrings.TryGetValue(name.ToString(), out var nameLs) ?? false)
                return nameLs;
            else
                return LsExtensions.Empty;
        }

        //#Dbg
        // public override void Dispose()
        // {
        //     if (DbgLog.Enabled) DbgLog.Log($"SeLiEl_VM  <{DBG_counter}>  .DISPOSE.");
        //     base.Dispose();
        // }
    }

    //#Dbg:
    // public static class LocalizedStringEstension
    // {
    //     public static string ToStr(this LocalizedString ls)
    //     {
    //         return $"{ls} (k:{ls.Key}, txt:{ls.GetText()}";
    //     }
    // }

}
