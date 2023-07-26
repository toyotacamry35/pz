using System;
using ColonyDI;
using ColonyShared.SharedCode.Input;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins.Cursor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityWeld.Binding;

namespace Uins
{
    public abstract class BaseGuiWindow : DependencyEndNode, IGuiWindow
    {
        [SerializeField, UsedImplicitly, FormerlySerializedAs("WindowId")]
        private WindowId _windowId;

        public WindowId Id => _windowId;

        private CursorControl.Token _token;


        //=== Props ===========================================================

        public WindowStackId CurrentWindowStack { get; set; }

        public virtual InputBindingsDef InputBindings => CurrentWindowStack.InputBindings ?? UI.BlockedActionsAndCamera;

        /// <summary>
        /// Должно ли окно раз появившись первым в стеке, сидеть в нем безвылазно
        /// </summary>
        public virtual bool IsUnclosable => false;

        /// <summary>
        /// Нужен ли окну курсор
        /// </summary>
        public virtual bool IsCursorUsed => true;

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        [Binding, UsedImplicitly]
        public bool IsOpen { get; protected set; }

        [Dependency]
        protected GameState GameState { get; set; }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            State.Value = GuiWindowState.Closed;
            Bind(State.Func(D, state => state == GuiWindowState.Opened), () => IsOpen);
        }

        protected virtual void Start()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ==========================================================

        public virtual void OnOpen(object arg)
        {
            if (IsCursorUsed)
                SetFreeCursor(true);
        }

        public virtual void OnClose()
        {
            if (IsCursorUsed)
                SetFreeCursor(false);
        }

        public virtual void OnFade()
        {
            if (IsCursorUsed)
                SetFreeCursor(false);
        }

        public virtual void OnUnfade()
        {
            if (IsCursorUsed)
                SetFreeCursor(true);
        }

        public virtual void OpenUpdate()
        {
        }

        public virtual void NoClosedUpdate()
        {
        }

        public virtual void ClosedHotkeyUpdate(Action additionalAction = null)
        {
        }

        public override void AfterDependenciesInjected()
        {
            base.AfterDependenciesInjected();
            WindowsManager.RegisterWindow(this);
        }


        //=== Private =========================================================

        private void SetFreeCursor(bool isCallNorRevoke)
        {
            if (isCallNorRevoke)
            {
                _token = CursorControl.AddCursorFreeRequest(this);
            }
            else
            {
                _token.Dispose();
                _token = null;
            }
        }
    }
}