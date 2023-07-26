using System;
using Assets.Src.Effects.UIEffects;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Uins.Cursor;
using Uins.GuiWindows;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.ContainerWindow
{
    [Binding]
    public class ContainerGuiWindow : DependencyEndNode, IGuiWindow
    {
        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _escHotkey;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _interactHotkey;

        [SerializeField, UsedImplicitly]
        private ContainerViewModel _containerViewModel;

        [SerializeField, UsedImplicitly]
        private InventoryNode _inventoryGuiWindow;

        [SerializeField, UsedImplicitly]
        private RectTransform _backgroundImageRectTransform;

        [SerializeField, UsedImplicitly]
        private InventoryNode _inventoryNode;

        private SpellWordCastData _lastCast;
        private IEntitiesRepository _lastCastRepository;
        private bool _isSubscribed;

        private CursorControl.Token _token;
        private readonly string _usageHudTag = nameof(ContainerGuiWindow);


        //=== Props ===========================================================

        [ColonyDI.Dependency]
        private HudGuiNode HudGui { get; set; }

        [ColonyDI.Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        [Binding]
        public bool IsVisible { get; private set; }

        [Binding]
        public bool IsBackgroundVisible { get; private set; }

        public InputBindingsDef InputBindings => CurrentWindowStack.InputBindings ?? UI.BlockedActionsAndCamera;

        private bool _isInteractHotkeyWasPressedDown;


        //=== Unity ===========================================================

        private void Awake()
        {
            _windowId.AssertIfNull(nameof(_windowId));
            _escHotkey.AssertIfNull(nameof(_escHotkey));
            _interactHotkey.AssertIfNull(nameof(_interactHotkey));
            _containerViewModel.AssertIfNull(nameof(_containerViewModel));
            _backgroundImageRectTransform.AssertIfNull(nameof(_backgroundImageRectTransform));
            _inventoryNode.AssertIfNull(nameof(_inventoryNode));

            State.Value = GuiWindowState.Closed;
            Bind(State.Func(D, state => state != GuiWindowState.Closed), () => IsVisible);
            var backgroundVisibleStream = _inventoryNode.State.Func(D, state => state == GuiWindowState.Closed);
            Bind(backgroundVisibleStream, () => IsBackgroundVisible);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ==========================================================

        public void OnOpen(object arg)
        {
            SoundControl.Instance.ContainerOpenEvent?.Post(gameObject);
            _token = CursorControl.AddCursorFreeRequest(this);
            WindowsManager.HudRightPartUsageNotifier.UsageRequest(_usageHudTag, this);
            var bgBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform.root, _backgroundImageRectTransform);
            BlurredBackground.Instance.SwitchCameraPartialBlur(this, true,
                new Rect(new Vector2(bgBounds.min.x / Screen.width, bgBounds.min.y / Screen.height),
                    new Vector2(bgBounds.size.x / Screen.width, bgBounds.size.y / Screen.height)));
        }

        public void OnClose()
        {
            SoundControl.Instance.ContainerCloseEvent?.Post(gameObject);
            WindowsManager.HudRightPartUsageNotifier.UsageRevoke(_usageHudTag, this);
            UnsubscribeIfNeed(); //Если пользователь закрыл окно сам, то в этом методе отписываемся и уведомляем спелл
            _token.Dispose();
            _token = null;
            if (_inventoryGuiWindow.State.Value != GuiWindowState.Closed)
                WindowsManager.Close(_inventoryGuiWindow);
            BlurredBackground.Instance.SwitchCameraPartialBlur(this, false, new Rect());
            _isInteractHotkeyWasPressedDown = false;
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            if (_interactHotkey.IsFired())
            {
                _isInteractHotkeyWasPressedDown = true;
                return;
            }

            if (_escHotkey.IsFired() || (_isInteractHotkeyWasPressedDown && _interactHotkey.IsFired(false)))
            {
                WindowsManager.Close(this);
            }
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
        }

        /// <summary>
        /// Уведомление со спелла об открытии
        /// </summary>
        public void OnOpenContainerFromSpell(BaseEffectOpenUi.EffectData effectData, bool isAuthoredContainer, ContainerMode containerType)
        {
            try
            {
                if (_containerViewModel.TargetOuterRef.IsValid)
                {
                    UnsubscribeIfNeed();
                }

                _lastCast = effectData.Cast;
                _lastCastRepository = effectData.Repo;

                _isSubscribed = true;
                _containerViewModel.SubscribeSlots(
                    containerType == ContainerMode.Bank ? effectData.StartTargetOuterRef : effectData.FinalTargetOuterRef,
                    containerType);

                if (State.Value == GuiWindowState.Closed)
                    WindowsManager.Open(this);

                if (isAuthoredContainer && _inventoryGuiWindow.State.Value == GuiWindowState.Closed)
                    WindowsManager.Open(_inventoryGuiWindow, null, new InventoryNode.OpenParams(InventoryNode.WindowMode.Container));
            }
            catch (Exception e)
            {
                UI.CallerLog($"Exception {e}");
            }
        }

        /// <summary>
        /// Уведомление со спелла о закрытии
        /// </summary>
        public void OnCloseContainerFromSpell(SpellWordCastData cast)
        {
            if (_lastCast != null && !cast.SpellId.Equals(_lastCast.SpellId))
            {
                UI.Logger.IfError()?.Message($"{nameof(OnCloseContainerFromSpell)}() spellIds mismatch! 1) last {_lastCast.SpellId} 2) current {cast.SpellId}").Write();
                //не будем сбрасывать прежний спелл, т.е. ниже попробуем его остановить
            }
            else
            {
                _lastCast = null; //не будем останавливать спелл
                _lastCastRepository = null;
            }

            UnsubscribeIfNeed();

            if (State.Value != GuiWindowState.Closed)
                WindowsManager.Close(this);
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
            SurvivalGui.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            _containerViewModel.Init(HudGui.PlayerMainStatsViewModel);
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                UnsubscribeIfNeed();

                if (State.Value != GuiWindowState.Closed)
                    WindowsManager.Close(this);
            }
        }

        private void UnsubscribeIfNeed()
        {
            if (_isSubscribed)
            {
                _isSubscribed = false;
                if (_containerViewModel.TargetOuterRef.IsValid)
                    _containerViewModel.UnsubscribeSlots();
            }

            if (_lastCast != null)
            {
                var lastCast = _lastCast;
                var lastCastRepository = _lastCastRepository;
                _lastCast = null;
                _lastCastRepository = null;
                AsyncUtils.RunAsyncTask(() => BaseEffectOpenUi.StopSpellAsync(lastCast, lastCastRepository));
            }
        }
    }
}