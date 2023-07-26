using System;
using System.Collections.Generic;
using EnumerableExtensions;
using ReactivePropsNs;
 
namespace Uins.Settings
{
    /// <summary>
    /// Возможно не оч.удачное название. По-сути это не столько про кнопку переключения разделов опций, сколько про содержание в VM структуры - описания всех опций этого раздела
    /// </summary>
    // #note: Expected lifetime - infinite
    public class SettingsGroupButtonVM : BindingVmodel
    {
        internal ReactiveProperty<SettingsGroup> SettingsGroup = new ReactiveProperty<SettingsGroup>();
        internal ListStream<SettingHandlerDefinition> SettingHandlerDefinitions;
        // Rp текущего выбранного раздела setting'ов.
        //   Оригинал лежит в `SettingsGuiWindowVM`,
        //   передаётся паре (Ctrl-VM) классов SettingsGroupButton, чтобы оттуда вещёлось в этот стрим.
        //   Также передаётся паре `SetOfSettings` - там:
        //    a) слушается и трансформируется в ListStream `SettingHandlerDefinitions`ов,
        //      который подключен к pool'у (тупому без реюза) параметризованному парой `SettingsListElem`.
        //    б) вещается в него `null`, когда нажата кнопка "Back" (так очищается список setting'ов)
        internal ReactiveProperty<SettingsGroupButtonVM> CurrentGroup;
        internal ReactiveProperty<bool> IsSelected = new ReactiveProperty<bool>();
        internal Func<bool> WasCurrShownSettingsChangedCallback;
        internal Action CancelCurrShownSettingsChangedCallback;
        internal Action<ConfirmationDialogParams> OpenConfirmationDialogCallback;

        internal SettingsGroupButtonVM(
            (SettingsGroup SettingsGroup, List<SettingHandlerDefinition> SettingHandlerDefinitions) tuple, 
            ReactiveProperty<SettingsGroupButtonVM> currGroup,
            Func<bool> wasCurrShownSettingsChangedCallback,
            Action cancelCurrShownSettingsChangedCallback,
            Action<ConfirmationDialogParams> openConfirmationDialogCallback)
        {
            wasCurrShownSettingsChangedCallback.AssertIfNull(nameof(wasCurrShownSettingsChangedCallback));
            cancelCurrShownSettingsChangedCallback.AssertIfNull(nameof(cancelCurrShownSettingsChangedCallback));
            openConfirmationDialogCallback.AssertIfNull(nameof(openConfirmationDialogCallback));

            SettingsGroup.Value = tuple.SettingsGroup;
            WasCurrShownSettingsChangedCallback = wasCurrShownSettingsChangedCallback;
            CancelCurrShownSettingsChangedCallback = cancelCurrShownSettingsChangedCallback;
            OpenConfirmationDialogCallback = openConfirmationDialogCallback;
            SettingHandlerDefinitions = new ListStream<SettingHandlerDefinition>(tuple.SettingHandlerDefinitions);
            CurrentGroup = currGroup;
            // Set 1st group as selected by default:
            if (!CurrentGroup.HasValue || CurrentGroup.Value == null)
                CurrentGroup.Value = this;
        }

        public override string ToString()
        {
            return $"Group:{(SettingsGroup.HasValue ? SettingsGroup.Value.ToString() : "!_Has_Value")};  SHandlerDefs:{SettingHandlerDefinitions.ItemsToStringByLines()}";
        }
    }
}
