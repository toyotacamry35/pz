using System;
using ColonyShared.SharedCode.Input;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins;
using UnityEngine;

public class InputTimeWindow : DependencyEndNode, IGuiWindow
{
    [SerializeField, UsedImplicitly]
    private WindowId _windowId;


    //=== Props ===============================================================

    public WindowId Id => _windowId;

    public WindowStackId CurrentWindowStack { get; set; }

    public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

    public bool IsUnclosable => false;

    public InputBindingsDef InputBindings => UI.BlockedActionsMovementAndCamera;


    //=== Public ==============================================================

    public void OnOpen(object arg)
    {
    }

    public void OnClose()
    {
    }

    public void OnFade()
    {
    }

    public void OnUnfade()
    {
    }

    public void OpenUpdate()
    {
    }

    public void NoClosedUpdate()
    {
    }

    public void ClosedHotkeyUpdate(Action additionalAction = null)
    {
    }


    //=== Unity ===============================================================

    private void Awake()
    {
        State.Value = GuiWindowState.Closed;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        State.Dispose();
    }


    //=== Protected ===========================================================

    public override void AfterDependenciesInjected()
    {
        WindowsManager.RegisterWindow(this);
    }
}