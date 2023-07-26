using JetBrains.Annotations;
using System;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Aspects;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using ReactivePropsNs;
using Uins;
using Uins.Cursor;
using Uins.Sound;
using UnityEngine;
using UnityEngine.UI;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using GeneratedCode.Repositories;
using SharedCode.Serializers;

public class RespawnGuiWindow : DependencyEndNode, IGuiWindow
{
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    [UsedImplicitly, SerializeField]
    private WindowId _windowId;

    [UsedImplicitly, SerializeField]
    private Canvas _canvas;

    [UsedImplicitly, SerializeField]
    private Button _defaultRespawnButton;

    [UsedImplicitly, SerializeField]
    private Button _randomRespawnButton;

    [UsedImplicitly, SerializeField]
    private Button _bakenRespawnButton;

    [UsedImplicitly, SerializeField]
    private Button _lobbyButton;

    [UsedImplicitly, SerializeField]
    private StuckWindow _stuckWindow;

    [UsedImplicitly, SerializeField]
    private TeleportWindow _teleportWindow;

    private bool _isWorking;

    private CursorControl.Token _token;


    //=== Props ===============================================================

    public WindowId Id => _windowId;

    public WindowStackId CurrentWindowStack { get; set; }

    public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>() {Value = GuiWindowState.Closed};

    public bool IsUnclosable => false;

    public InputBindingsDef InputBindings => CurrentWindowStack.InputBindings ?? UI.BlockedActionsAndCamera;

    [ColonyDI.Dependency]
    private SurvivalGuiNode SurvivalGui { get; set; }

    [ColonyDI.Dependency]
    private LobbyGuiNode LobbyGui { get; set; }
    
    [ColonyDI.Dependency]
    private StartGameGuiNode StartGameNode { get; set; }


    //=== Unity ===============================================================

    private void Awake()
    {
        _windowId.AssertIfNull(nameof(_windowId));
        _canvas.AssertIfNull(nameof(_canvas));
        _lobbyButton.AssertIfNull(nameof(_lobbyButton));
        _stuckWindow.AssertIfNull(nameof(_stuckWindow));
        _teleportWindow.AssertIfNull(nameof(_teleportWindow));
        SwitchVisibility(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        State.Dispose();
    }


    //=== Public ==============================================================

    public async void OnOpen(object arg)
    {
        _token = CursorControl.AddCursorFreeRequest(this);
        SwitchVisibility(true);

        // Наследие времён без Weld, стримов и биндингов:
        _lobbyButton.interactable = true;
        _defaultRespawnButton.interactable = true;
        _randomRespawnButton.interactable = false;
        _bakenRespawnButton.interactable = false;
        //BlurredBackground.Instance.SwitchCameraFullBlur(this, true); //пока не нужно, т.к. блюрить нечего - камеры нет

        try
        {
            bool hasBaken = false;
            var repo = NodeAccessor.Repository;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IWorldCharacter>(GameState.Instance.CharacterRuntimeData.CharacterId))
                {
                    var character = wrapper?.Get<IWorldCharacterClientFull>(GameState.Instance.CharacterRuntimeData.CharacterId);
                    if (character == null)
                    {
                        Logger.IfError()?.Message($"Can't respawn. Can't get {nameof(IWorldCharacterClientFull)}").Write();
                        return;
                    }

                    hasBaken = await character.HasBaken();
                }
            });

            _bakenRespawnButton.interactable = hasBaken;
        }
        catch (Exception e)
        {
            UI.Logger.IfError()?.Message($"HasBaken exception: {e}").Write();
            _bakenRespawnButton.interactable = false;
        }
    }

    public void OnClose()
    {
        try
        {
            SoundControl.Instance.InventoryOpenEvent?.Post(gameObject);
            _token.Dispose();
            _token = null;
            SwitchVisibility(false);
            //BlurredBackground.Instance.SwitchCameraFullBlur(this, false);
        }
        catch (Exception e)
        {
            UI.CallerLog($"OnClose() {e}");
        }
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

    [UsedImplicitly]
    public void GoToLobbyCommand()
    {
        if (State.Value != GuiWindowState.Closed)
            WindowsManager.Close(this);

        _lobbyButton.interactable = false;
        StartGameNode.ExitToLobby();
    }

    public void RespawnCommand(Button button)
    {
        var repo = NodeAccessor.Repository;
        var defaultRespawn = button == _defaultRespawnButton;
        var randomRespawn = button == _randomRespawnButton;
        var bakenRespawn = button == _bakenRespawnButton;
        AsyncUtils.RunAsyncTask(async () =>
        {
            using (var wrapper = await repo.Get<IWorldCharacter>(GameState.Instance.CharacterRuntimeData.CharacterId))
            {
                var character = wrapper?.GetFirstService<IWorldCharacterClientFull>();
                if (character == null)
                {
                    Logger.IfError()?.Message($"Can't respawn. Can't get {nameof(IWorldCharacterClientFull)}").Write();
                    return;
                }

                if (defaultRespawn)
                {
                    await character.Respawn(false, false, default);
                }
                else if (randomRespawn)
                {
                    await character.Respawn(false, false, default);
                }
                else if (bakenRespawn)
                {
                    await character.Respawn(true, true, default);
                }
            }
        });
        _defaultRespawnButton.interactable = false;
        _randomRespawnButton.interactable = false;
        _bakenRespawnButton.interactable = false;

        if (State.Value != GuiWindowState.Closed)
            WindowsManager.Close(this);
    }

    public void SwitchWorking(bool isOn)
    {
        _isWorking = isOn;
    }


    //=== Protected ===========================================================

    public override void AfterDependenciesInjected()
    {
        WindowsManager.RegisterWindow(this);
        SurvivalGui.PawnChangesStream.Action(D, OnOurPawnChanged);
    }


    //=== Private =============================================================

    private void SwitchVisibility(bool isVisible)
    {
        _canvas.enabled = isVisible;
    }

    private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
    {
        if (prevEgo != null)
        {
            var subjectPawn = prevEgo.GetComponent<ISubjectPawn>();
            if (!subjectPawn.AssertIfNull(nameof(subjectPawn), gameObject)
                && NodeAccessor.Repository != null 
                && !(NodeAccessor.Repository.TryGetLockfree<IHasMortalClientBroadcast>
                (prevEgo.OuterRefEntity, ReplicationLevel.ClientBroadcast)?.Mortal.IsAlive ?? true))
            {
                if (_isWorking && State.Value == GuiWindowState.Closed)
                {
                    if (_stuckWindow.State.Value != GuiWindowState.Closed)
                        WindowsManager.Close(_stuckWindow);
                    if (_teleportWindow.State.Value != GuiWindowState.Closed)
                        WindowsManager.Close(_teleportWindow);
                    WindowsManager.Open(this);
                }
            }
        }

        if (newEgo != null)
        {
            if (State.Value != GuiWindowState.Closed)
                WindowsManager.Close(this);
        }
    }
}