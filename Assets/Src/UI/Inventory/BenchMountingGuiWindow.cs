using System;
using Assets.Src.Aspects.Building;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Input;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using ReactivePropsNs;
using ReactivePropsNs.ThreadSafe;
using Src.Input;
using Src.InputActions;
using Uins.Slots;
using Uins.Sound;
using UnityEngine;
using ThreadSafe = ReactivePropsNs.ThreadSafe;
using NonThreadSafe = ReactivePropsNs;

namespace Uins
{
    public class BenchMountingGuiWindow : DependencyEndNode, IGuiWindow
    {
        [UsedImplicitly, SerializeField]
        private WindowId _windowId;

        [UsedImplicitly, SerializeField]
        private InputBindingsRef _inputBindings;

        [UsedImplicitly, SerializeField]
        private InputActionTriggerRef _mountAction;

        [UsedImplicitly, SerializeField]
        private InputActionTriggerRef _cancelAction;

        [UsedImplicitly, SerializeField]
        private GameObject _soundGameObject;

        private BuildingCreator _buildingCreator;
        private SlotViewModel _currentSlotViewModel;


        //=== Props ===========================================================

        [ColonyDI.Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        private ThreadSafe.DisposableComposite _subscriptions = new ThreadSafe.DisposableComposite();
        
        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public NonThreadSafe.ReactiveProperty<GuiWindowState> State { get; set; } = new NonThreadSafe.ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => _inputBindings.Target;


        //=== Unity =============================================================

        private void Awake()
        {
            _windowId.AssertIfNull(nameof(_windowId));
            _inputBindings.Target.AssertIfNull(nameof(_inputBindings));
            _cancelAction.Target.AssertIfNull(nameof(_cancelAction));
            _mountAction.Target.AssertIfNull(nameof(_mountAction));
            _soundGameObject.AssertIfNull(nameof(_soundGameObject));
            State.Value = GuiWindowState.Closed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ============================================================

        public void OnOpen(object arg)
        {
            if (_buildingCreator.AssertIfNull(nameof(_buildingCreator)) ||
                arg.AssertIfNull(nameof(arg)))
                return;

            _currentSlotViewModel = (SlotViewModel) arg;
            if (_currentSlotViewModel.AssertIfNull(nameof(_currentSlotViewModel)))
                return;

            _buildingCreator.MountingStart(_currentSlotViewModel, OnMountingEnd);
            
            InputManager.Instance.Stream(_mountAction.Target)
                .Where(_subscriptions, a => a.Activated)
                .First(_subscriptions)
                .Action(_subscriptions, a => _buildingCreator.MountingAccept().WrapErrors());
            
            InputManager.Instance.Stream(_cancelAction.Target)
                .Where(_subscriptions, a => a.Activated)
                .First(_subscriptions)
                .Action(_subscriptions, a => _buildingCreator.Cancel());
        }

        private void OnMountingEnd(bool isSuccess)
        {
            if (isSuccess)
            {
                SoundControl.Instance.CraftBuildingPlacementEvent?.Post(_soundGameObject);
            }
            else
            {
                SoundControl.Instance.CraftBuildingPlacementDeniedEvent?.Post(_soundGameObject);
            }

            WindowsManager.Close(this);
        }

        public void OnClose()
        {
            _subscriptions.Clear();
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (!Mathf.Approximately(scroll, 0))
                _buildingCreator.Rotate(50 * scroll);
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            SurvivalGui.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _buildingCreator = null;
            }

            if (newEgo != null)
            {
                _buildingCreator = newEgo.GetComponent<BuildingCreator>();
                _buildingCreator.AssertIfNull(nameof(_buildingCreator), gameObject);
            }
        }
    }
}