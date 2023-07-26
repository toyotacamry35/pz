using System;
using Assets.ReactiveProps;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using UnityWeld.Binding;
using ReactivePropsNs;

namespace Uins.Settings
{
    // #note: Expected lifetime - infinite
    [Binding]
    public class SettingsGuiWindowCtrl : BindingController<SettingsGuiWindowVM>
    {
        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("SettingsGuiWindowCtrl");

        [SerializeField, UsedImplicitly]  private SettingsGroupButtonCtrl _settingsGroupButtonPrefab;
        [SerializeField, UsedImplicitly]  private Transform _settingsGroupButtonsRoot;
        [SerializeField, UsedImplicitly]  private SetOfSettingsCtrl _setOfSettingsListContainerPrefab;
        [SerializeField, UsedImplicitly]  private Transform _settingsListContainersRoot;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local - Is used implicitly by binding via reflection 
        [Binding]  public bool IsActive { get; private set; }

        private BindingControllersPool<SettingsGroupButtonVM> _groupsBtnsPool;
        private Action<ConfirmationDialogParams> _openConfirmationDialogCallback;

        internal void Init(
            Action escHotkeyPressedWindowEvent, 
            Action applyHotkeyPressedWindowEvent, 
            Action defaultsHotkeyPressedWindowEvent, 
            Action closeWindowCallback,
            Action<ConfirmationDialogParams> openConfirmationDialogCallback)
        {
            _settingsGroupCtrl.Init(
                escHotkeyPressedWindowEvent, 
                applyHotkeyPressedWindowEvent, 
                defaultsHotkeyPressedWindowEvent, 
                closeWindowCallback,
                openConfirmationDialogCallback);

            _openConfirmationDialogCallback = openConfirmationDialogCallback;
        }

        SetOfSettingsCtrl _settingsGroupCtrl;
        [UsedImplicitly]
        private void Awake()
        {
            _settingsGroupButtonPrefab       .AssertIfNull(nameof(_settingsGroupButtonPrefab));
            _settingsGroupButtonsRoot        .AssertIfNull(nameof(_settingsGroupButtonsRoot));
            _setOfSettingsListContainerPrefab.AssertIfNull(nameof(_setOfSettingsListContainerPrefab));
            _settingsListContainersRoot      .AssertIfNull(nameof(_settingsListContainersRoot));

            //_settingsGroupButtonsRoot.DestroyAllChildren(); //Clean dbg content   

            _groupsBtnsPool = new BindingControllersPool<SettingsGroupButtonVM>(_settingsGroupButtonsRoot, _settingsGroupButtonPrefab);
            Vmodel.Action(D, OnVmChanged);

            //_settingsListContainersRoot.DestroyAllChildren(); //Clean dbg content   
            _settingsGroupCtrl = Instantiate(_setOfSettingsListContainerPrefab, _settingsListContainersRoot);
            Vmodel.Action(D, vm =>
                {
                    var cg = vm.CurrentGroup;
                    _settingsGroupCtrl.SetVmodel(new SetOfSettingsVM(cg, cg));
                });

            Bind(Vmodel.SubStream(D, vm => vm.IsActive)/*.Log(D, "@+@+@+@+@+@ : ~НЕ длжно происходить вроде~ СЛУЧаЕТСЯ" + IsActive)*/, () => IsActive);
        }


        // --- Publics: ---------------------------------------

        internal void SetActive(bool val)
        {
            if (Vmodel.HasValue)
                Vmodel.Value.IsActive.Value = val;
        }

        private void OnVmChanged(SettingsGuiWindowVM vm)
        {
            _groupsBtnsPool.Disconnect();
            if (!Vmodel.HasValue)
            {
                System.Diagnostics.Debug.Assert(vm == null);
                return;
            }
            //else:
            System.Diagnostics.Debug.Assert(vm == Vmodel.Value);
            // Когда VM.HasVal,  //а это случится только 1 раз, т.к. потом пару Ctrl-VM никогда больше не меняем на протяжении всей игры
            //  так вот, когда VM.HasVal, т.е. когда VM наконец готова, мы превращаем её статич.лист, описывающий всё содержимое окна настроек
            //  в ListStream элементов `SettingsGroupButtonVM` (носителей д-х обо всём содержании одной группы setting'ов).
            // Т.к. source-лист статический, то и pool по сути будет статичным (после наполнения с ним ничего не будет происходить)
            _groupsBtnsPool.Connect(Vmodel.SubListStream(D, vmodel => vmodel.ContentDefinition).Transform(D,
                    tuple =>
                    {
                        System.Diagnostics.Debug.Assert(!tuple.Equals(null));
                        System.Diagnostics.Debug.Assert(Vmodel.HasValue);
                        return new SettingsGroupButtonVM(tuple, 
                            Vmodel.Value.CurrentGroup, 
                            WasCurrShownSettingsChanged,
                            CancelCurrShownSettingsChanged,
                            _openConfirmationDialogCallback);
                    }));
        }
        
        // --- Privates: -------------------------------------

        private bool WasCurrShownSettingsChanged() => _settingsGroupCtrl?.WasChanged ?? false;
        private void CancelCurrShownSettingsChanged() => _settingsGroupCtrl?.Cancel();

    }
}
