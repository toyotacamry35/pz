using ColonyShared.SharedCode.Input;
using System;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Uins.Cursor;
using Uins.GuiWindows;
using Uins.Sound;
using JetBrains.Annotations;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using Uins;
using Uins.Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

[Binding]
public class EscMenuGuiWindow : DependencyEndNode, IGuiWindow
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    [SerializeField, UsedImplicitly]
    private WindowId _windowId;

    [SerializeField, UsedImplicitly]
    private Canvas _canvas;

    [SerializeField, UsedImplicitly]
    private HotkeyListener _escHotkey;

    [SerializeField, UsedImplicitly]
    private RespawnGuiWindow _respawnGuiWindow;

    [SerializeField, UsedImplicitly]
    private SettingsGuiWindow _settingsGuiWindow;

    [SerializeField, UsedImplicitly]
    private Button _lobbyButton;

    [SerializeField, UsedImplicitly]
    private WindowStackId[] _observableWindowStacks;

    [SerializeField, UsedImplicitly]
    private TimeLeftCtrl TimeLeftCtrl;

    [SerializeField, UsedImplicitly]
    private Transform _friendsTransform;

    [SerializeField, UsedImplicitly]
    private FriendItemContr _friendItemContrPrefab;


    //=== Props ===============================================================

    public WindowId Id => _windowId;

    public WindowStackId CurrentWindowStack { get; set; }

    public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

    [Binding]
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    public bool IsFadedOrClosed { get; private set; }

    [Binding]
    public bool HasFriends { get; private set; }

    public bool IsUnclosable => false;

    public InputBindingsDef InputBindings => UI.BlockedActionsMovementAndCamera;

    [ColonyDI.Dependency]
    private LobbyGuiNode LobbyGui { get; set; }

    [ColonyDI.Dependency]
    private StartGameGuiNode StartGameNode { get; set; }

    private CursorControl.Token _token;
    private DisposableComposite _timerD;


    //=== Unity ===============================================================

    private void Awake()
    {
        _windowId.AssertIfNull(nameof(_windowId));
        _canvas.AssertIfNull(nameof(_canvas));
        _escHotkey.AssertIfNull(nameof(_escHotkey));
        _respawnGuiWindow.AssertIfNull(nameof(_respawnGuiWindow));
        _settingsGuiWindow.AssertIfNull(nameof(_settingsGuiWindow));
        _observableWindowStacks.IsNullOrEmptyOrHasNullElements(nameof(_observableWindowStacks));
        _friendsTransform.AssertIfNull(nameof(_friendsTransform));
        _friendItemContrPrefab.AssertIfNull(nameof(_friendItemContrPrefab));

        SwitchVisibility(false);

        State.Value = GuiWindowState.Closed;

        var fadedStream = State.Func(D, s => s == GuiWindowState.Faded || s == GuiWindowState.Closed);
        Bind(fadedStream, () => IsFadedOrClosed);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        State.Dispose();
    }


    //=== Public ==============================================================

    public void OnOpen(object arg)
    {
        _token = CursorControl.AddCursorFreeRequest(this);
        SwitchVisibility(true);
        _lobbyButton.interactable = true;
        BlurredBackground.Instance.SwitchCameraFullBlur(this, true);

        _timerD = D.CreateInnerD();
        InitTimer(_timerD);
    }

    private void InitTimer(DisposableComposite localD)
    {
        var accountEntityVM = new EntityVM<IAccountEntityClientFull>(
            GameState.Instance.ClientClusterRepository,
            GameState.Instance.AccountIdStream
        );
        localD.Add(accountEntityVM);
        var charRealmDataTouchable = accountEntityVM.Touchable.Child(localD, account => account.CharRealmData);
        var realmOuterRefStream = charRealmDataTouchable.ToStream(localD, charRealmData => charRealmData.CurrentRealm);
        var realmVM = new RealmVM(GameState.Instance.ClientClusterRepository, realmOuterRefStream);
        localD.Add(realmVM);
        var timeLeftVM = new TimeLeftVM(realmVM.RealmTimeLeftSec);
        localD.Add(timeLeftVM);
        TimeLeftCtrl.SetVmodel(timeLeftVM);
        _timerD.Add(new DisposeAgent(() => TimeLeftCtrl.SetVmodel(null)));
    }

    public void OnClose()
    {
        if (_timerD != null)
        {
            D.DisposeInnerD(_timerD);
            _timerD = null;
        }

        SoundControl.Instance.InventoryOpenEvent?.Post(gameObject);
        _token.Dispose();
        _token = null;
        SwitchVisibility(false);
        BlurredBackground.Instance.SwitchCameraFullBlur(this, false);
    }

    public void OnFade()
    {
    }

    public void OnUnfade()
    {
    }

    public void OpenUpdate()
    {
        if (State.Value != GuiWindowState.Closed && _escHotkey.IsFired())
            WindowsManager.Close(this);
    }

    public void NoClosedUpdate()
    {
    }

    public void ClosedHotkeyUpdate(Action additionalAction = null)
    {
    }

    public void GoToLobbyCommand()
    {
        if (State.Value != GuiWindowState.Closed)
            WindowsManager.Close(this);

        _lobbyButton.interactable = false;
         Logger.IfInfo()?.Message("Exit To Lobby Requested").Write();;
        StartGameNode.ExitToLobby();
        Application.Quit();
    }

    public void GoToStartGameCommand()
    {
        if (State.Value != GuiWindowState.Closed)
            WindowsManager.Close(this);

         Logger.IfInfo()?.Message("Exit To Start Game Requested").Write();;
        StartGameNode.ExitToStartGame();
    }

    private static EscMenuGuiWindow _instance;

    public EscMenuGuiWindow()
    {
        _instance = this;
    }

    [Cheat, UsedImplicitly]
    public static void ExitToStartGame()
    {
        _instance.GoToStartGameCommand();
    }

    public void OpenCheck()
    {
        if (State.Value == GuiWindowState.Closed && _escHotkey.IsFired())
        {
            for (int i = 0, len = _observableWindowStacks.Length; i < len; i++)
            {
                if (WindowsManager.HasOpenedWindowsInStack(_observableWindowStacks[i]))
                    return;
            }

            WindowsManager.Open(this);
        }
    }

    [UsedImplicitly] //OnClick
    public void ShowSettingsWindow()
    {
        WindowsManager.Open(_settingsGuiWindow);
    }


    //=== Protected ===========================================================

    public override void AfterDependenciesInjected()
    {
        WindowsManager.RegisterWindow(this);

        var controllersPool = new BindingControllersPoolWithUsingProp<FriendInfo>(_friendsTransform, _friendItemContrPrefab);
        controllersPool.Connect(SurvivalGuiNode.Instance.FriendList);
        Bind(SurvivalGuiNode.Instance.FriendList.CountStream.Func(D, count => count > 0), () => HasFriends);
    }


    //=== Private =============================================================

    private void SwitchVisibility(bool isVisible)
    {
        _canvas.enabled = isVisible;
    }
}