using System;
using ColonyHelpers;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Logging;
using SharedCode.Serializers;
using Uins.Cursor;
using UnityEngine;
using UnityWeld.Binding;
using ReplicationLevel = SharedCode.EntitySystem.ReplicationLevel;

namespace Uins
{
    [Binding]
    public class StuckWindow : DependencyEndNode, IGuiWindow
    {
        private const int SuicideAfter = 5;//45;

        [UsedImplicitly, SerializeField]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _openHotkeyListener;

        private float _closeWindowTime;

        private CursorControl.Token _token;


        //=== Props ==============================================================

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => UI.BlockedActionsMovementAndCamera;

        [ColonyDI.Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _secondsBefore;

        [Binding]
        public int SecondsBefore
        {
            get => _secondsBefore;
            set
            {
                if (_secondsBefore != value)
                {
                    _secondsBefore = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(SecondsBeforeRatio));
                }
            }
        }

        [Binding]
        public float SecondsBeforeRatio => SecondsBefore / (float) SuicideAfter;


        //=== Unity ===============================================================

        private void Awake()
        {
            _windowId.AssertIfNull(nameof(_windowId));
            _openHotkeyListener.AssertIfNull(nameof(_openHotkeyListener));
            State.Value = GuiWindowState.Closed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ===========================================================

        public void OnOpen(object arg)
        {
            IsVisible = true;
            _token = CursorControl.AddCursorFreeRequest(this);
            _closeWindowTime = Time.time + SuicideAfter;
        }

        public void OnClose()
        {
            IsVisible = false;
            _token.Dispose();
            _token = null;
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            if (!_openHotkeyListener.IsPressed())
                WindowsManager.Close(this);

            SecondsBefore = Mathf.RoundToInt(_closeWindowTime - Time.time);

            if (SecondsBefore <= 0)
            {
                DoSuicide();
                WindowsManager.Close(this);
            }
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
            if (_openHotkeyListener.IsFired())
                WindowsManager.Open(this);
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
        }


        //=== Private =========================================================

        private void DoSuicide()
        {
            var charRef = Helpers.GetCharacterRef();
            if (!charRef.IsValid)
            {
                Log. Logger.IfError()?.Message("Can't get character ref.").Write();;
                return;
            }

            var repo = GameState.Instance.ClientClusterNode;
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(charRef))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(charRef.TypeId, charRef.Guid, ReplicationLevel.ClientFull);
                    if (worldCharacter != null)
                        await worldCharacter.Suicide();
                }
            });
        }
    }
}