using System;
using Assets.ReactiveProps;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using UnityEngine;
 
namespace Uins.Settings
{
    // #note: Expected lifetime - infinite
    public class SetOfSettingsCtrl : BindingController<SetOfSettingsVM>, IApplyableCancelableDefaultableCanBeChanged
    {
        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly] private SettingsListElemCtrl _listElemPrefab;
        [SerializeField, UsedImplicitly] private Transform _listItemsRoot;
        [SerializeField, UsedImplicitly] private LocalizationKeysDefRef _settingsNamesLocalizationKeys;
        [SerializeField, UsedImplicitly] private LocalizationKeyProp _leaveConfirmationQuestion;


        private event Action _escHotkeyPressedWindowEvent;
        private event Action _applyHotkeyPressedWindowEvent;
        private event Action _defaultsHotkeyPressedWindowEvent;
        private Action _closeWindowCallback;
        private Action<ConfirmationDialogParams> _openConfirmationDialogCallback;

        private BindingControllersPool<SettingsListElemVM> _pool;

        internal void Init(
            Action escHotkeyPressedWindowEvent, 
            Action applyHotkeyPressedWindowEvent, 
            Action defaultsHotkeyPressedWindowEvent,
            [NotNull] Action closeWindowCallback,
            [NotNull] Action<ConfirmationDialogParams> openConfirmationDialogCallback)
        {
            _escHotkeyPressedWindowEvent      = escHotkeyPressedWindowEvent;
            _applyHotkeyPressedWindowEvent    = applyHotkeyPressedWindowEvent;
            _defaultsHotkeyPressedWindowEvent = defaultsHotkeyPressedWindowEvent;
            _closeWindowCallback              = closeWindowCallback;
            _openConfirmationDialogCallback   = openConfirmationDialogCallback;

            _closeWindowCallback.AssertIfNull(nameof(_closeWindowCallback)); // 3 others are null here, cos no subscribers
            _openConfirmationDialogCallback.AssertIfNull(nameof(_openConfirmationDialogCallback));

            _escHotkeyPressedWindowEvent += Back;
            _applyHotkeyPressedWindowEvent += TryApply;
            _defaultsHotkeyPressedWindowEvent += TrySetToDefault;
        }

        protected override void OnDestroy()
        {
            _escHotkeyPressedWindowEvent -= Back;
            _applyHotkeyPressedWindowEvent -= TryApply;
            _defaultsHotkeyPressedWindowEvent -= TrySetToDefault;
            base.OnDestroy();
        }

        [UsedImplicitly] //U
        private void Awake()
        {
            _listElemPrefab.AssertIfNull(nameof(_listElemPrefab));
            _listItemsRoot .AssertIfNull(nameof(_listItemsRoot));
            _settingsNamesLocalizationKeys?.Target.AssertIfNull(nameof(_settingsNamesLocalizationKeys));
            if (!_leaveConfirmationQuestion?.IsValid ?? true)
                Logger.Error($"_leaveConfirmationQuestion?.IsValid: {_leaveConfirmationQuestion?.IsValid}");

            //_pool = new BindingControllersPool<SettingsListElemVM>(_listItemsRoot, _listElemPrefab);
            //_pool = new BindingControllersPoolWithUsingProp<SettingsListElemVM>(_listItemsRoot, _listElemPrefab);
            _pool = new BindingControllersPoolWithGoActivation<SettingsListElemVM>(_listItemsRoot, _listElemPrefab);
            var contentStream = Vmodel.SubStream(D, vm => vm.ContentStream);
            var s = contentStream.SubListStream(D, vm => vm?.SettingHandlerDefinitions);
            _pool.Connect(s.Transform(D, y => new SettingsListElemVM(y, _settingsNamesLocalizationKeys)));
        }

        public bool WasChanged => WasSomeItemChanged();
        private bool WasSomeItemChanged()
        {
            if (_pool == null)
                return false;

            bool result = false;
            foreach (var ctrl in _pool.GetEnumerable)
            {
                result |= ((SettingsListElemCtrl) ctrl).WasChanged;
                if (result)
                    break;
            }
            return result;
        }


        // --- IApplyableCancelable: ---------------------------------
        public bool Apply()
        {
            if (_pool == null)
                return false;

            bool result = false;
            foreach (var ctrl in _pool.GetEnumerable)
                result |= ((SettingsListElemCtrl)ctrl).Apply();

            return result;
        }

        public bool Cancel()
        {
            if (_pool == null)
                return false;

            bool result = false;
            foreach (var ctrl in _pool.GetEnumerable)
                result |= ((SettingsListElemCtrl)ctrl).Cancel();

            return result;
        }

        public bool SetToDefault()
        {
            if (_pool == null)
                return false;

            bool result = false;
            foreach (var ctrl in _pool.GetEnumerable)
                result |= ((SettingsListElemCtrl)ctrl).SetToDefault();

            return result;
        }


        // --- Publics: ---------------------------

        public void TryApply() => Apply();
        /// <summary>
        /// 1st "Back" nulls vm, 2nd should close window
        /// </summary>
        public void Back()
        {
            if (!WasChanged)
                _closeWindowCallback();
            else
            {
                var dialogParams = new ConfirmationDialogParams()
                {
                    OnConfirmAction = ConfirmationDialogOnConfirm,
                    OnCancelAction = null,
                    Description = _leaveConfirmationQuestion.LocalizedString  
                };
                _openConfirmationDialogCallback(dialogParams);
            }
        }

        private void ConfirmationDialogOnConfirm()
        {
            Cancel();
            _closeWindowCallback();
        }

        public void TrySetToDefault() => SetToDefault();

    }
}
