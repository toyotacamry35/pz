using Assets.Src.ResourceSystem.L10n;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Settings
{
    /// <summary>
    /// Возможно не оч.удачное название. По-сути это не столько про кнопку переключения разделов опций, сколько про содержание в VM структуры - описания всех опций этого раздела
    /// </summary>
    // #note: Expected lifetime - infinite
    [Binding]
    public class SettingsGroupButtonCtrl : BindingController<SettingsGroupButtonVM>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("SettingsGroupButtonCtrl");

        // ReSharper disable once UnusedAutoPropertyAccessor.Local - used implicitly by binding via reflection
        [Binding] public bool IsSelected  { get; private set; } //for outline
        // ReSharper disable once UnusedAutoPropertyAccessor.Local - used implicitly by binding via reflection
        [Binding] public LocalizedString GroupName { get; private set; }

        [SerializeField, UsedImplicitly] private LocalizationKeysDefRef _settingsGroupsLocalizationKeys;
        [SerializeField, UsedImplicitly] private LocalizationKeyProp _leaveConfirmationQuestion;

        [UsedImplicitly] //U
        private void Awake()
        {
            _settingsGroupsLocalizationKeys?.Target.AssertIfNull(nameof(_settingsGroupsLocalizationKeys));
            if (!_leaveConfirmationQuestion?.IsValid ?? true)
                Logger.Error($"_leaveConfirmationQuestion?.IsValid: {_leaveConfirmationQuestion?.IsValid}");

            // Listen `CurrGroup` for local purposes: i.e. show outline, ... Listening to fill concrete settings list resides at `SettingsGuiWindowCtrl`
            var isSelectedStream = Vmodel.SubStream(D, vm => vm.CurrentGroup) .Func(D, x => x != null && x == Vmodel.Value);
            var groupNameStream  = Vmodel.SubStream(D, vm => vm.SettingsGroup).Func(D, x =>
            {
                if (_settingsGroupsLocalizationKeys?.Target?.LocalizedStrings.TryGetValue(x.ToString(), out var groupName) ?? false)
                    return groupName;
                else
                    return LsExtensions.Empty;
            });
            
            Bind(isSelectedStream, () => IsSelected);
            Bind(groupNameStream,  () => GroupName);
        }

        [UsedImplicitly] //Should be set to Btn.OnClick() 
        public void ChoseCurrGrop()
        {
            // Set `CurrGroup`
            //Vmodel.Value.CurrentGroup.Value = null; //#tmp attempt crutch. Иначе начинают теряться названия опций и txt-представление curr.val при непосредственном переключении м/у разделами
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(ChoseCurrGrop)}({Vmodel.Value})").Write();

            if (!Vmodel.HasValue || !Vmodel.Value.WasCurrShownSettingsChangedCallback())
                Vmodel.Value.CurrentGroup.Value = Vmodel.Value;
            else
            {
                var dialogParams = new ConfirmationDialogParams()
                {
                    OnConfirmAction = ConfirmationDialogOnConfirm,
                    OnCancelAction = null,
                    Description = _leaveConfirmationQuestion.LocalizedString
                };
                Vmodel.Value.OpenConfirmationDialogCallback(dialogParams);
            }
        }
        private void ConfirmationDialogOnConfirm()
        {
            Vmodel.Value.CancelCurrShownSettingsChangedCallback();
            Vmodel.Value.CurrentGroup.Value = Vmodel.Value;
        }

    }
}
