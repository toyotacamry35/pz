using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.Input;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Uins.Cursor;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TeleportWindow: DependencyEndNode, IGuiWindow
    {
        [UsedImplicitly, SerializeField]
        private WindowId _windowId;

        private float _duration;
        
        private float _endTime;

        private readonly List<object> _causers = new List<object>();

        //=== Props ==============================================================
        public static TeleportWindow Instance { get; private set; }

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => null;

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
        public float SecondsBeforeRatio => _duration > 0 ? SecondsBefore / (float) _duration : 0;


        //=== Unity ===============================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
            _windowId.AssertIfNull(nameof(_windowId));
            State.Value = GuiWindowState.Closed;
        }

        protected override void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ===========================================================

        public void Open(object causer, float duration)
        {
            _duration = duration;
            _endTime = Time.time + _duration;
            if (!_causers.Contains(causer))
            {
                _causers.Add(causer);
                if (_causers.Count == 1)
                    WindowsManager.Open(this);
            }
        }

        public void Close(object causer)
        {
            if (_causers.Remove(causer) && _causers.Count == 0)
                WindowsManager.Close(this);
        }

        public void OnOpen(object arg)
        {
            IsVisible = true;
        }

        public void OnClose()
        {
            IsVisible = false;
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            SecondsBefore = Mathf.RoundToInt(_endTime - Time.time);
            if (SecondsBefore <= 0)
                WindowsManager.Close(this);
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {}

        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
        }
    }
}