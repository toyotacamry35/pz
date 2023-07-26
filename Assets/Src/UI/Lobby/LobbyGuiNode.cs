using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uins;
using ColonyDI;
using GeneratedCode.Custom.Config;
using JetBrains.Annotations;
using UnityEngine;
using AwesomeTechnologies.VegetationStudio;
using Infrastructure.Config;
using Assets.Src.ResourceSystem;
using Assets.Src.Client.Impl;
using System.Threading;
using UnityWeld.Binding;
using Assets.Src.Aspects.Doings;
using Assets.Src.Camera;
using ColonyShared.SharedCode.Utils;
using L10n;
using SharedCode.Utils.Threads;
using NLog.Fluent;
using Utilities;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using SharedCode.Serializers;

[Binding]
public class LobbyGuiNode : DependencyEndNode
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    [Serializable]
    public class MapRootDefRef : JdbRef<MapRootDef>
    {
    }

    [SerializeField, UsedImplicitly]
    private AkAudioListener _uiAkAudioListener;

    [SerializeField, UsedImplicitly]
    private MapRootDefRef _mapRoot;

    [SerializeField, UsedImplicitly]
    private SceneRef[] _additionalLobbyScenes;

    [SerializeField, UsedImplicitly]
    private LocalizationKeysHolder _connectionLkh;

    [SerializeField, UsedImplicitly]
    private WindowId _settingsWindowId;

    public CreditsContr CreditsContr;

    public const string DefaultDedicAddress = "127.0.0.1";
    public const int DefaultDedicPort = 7800;
    public const string DefaultDevUserName = "";

    private const string Version = "Best version ever";

    private const string UseOptionalScenesInEditorModeKey = "UseOptionalScenesInEditorMode";

    private Camera _guiCamera;

    //=== Props ===============================================================

    [Dependency]
    private GameState GameState { get; set; }

    [Dependency]
    public StartGameGuiNode StartGameGuiNode { get; private set; }

    [Dependency]
    public LoadingScreenNode LoadingScreenUtility { get; private set; }

    public IEnumerable<string> LobbyScenes => UseOptionalScenes
        ? _additionalLobbyScenes.Select(v => v.ScenePath)
        : Enumerable.Empty<string>();

    public string CurrentDevUserName { get; set; }

    public string CurrentDedicAddress { get; set; }

    public int CurrentDedicPort { get; set; }

    public MapDef CurrentMapDef { get; private set; }

    private bool _isVisible;

    public bool BotControl { get; set; } = false;

    public ReactiveProperty<bool> IsVisibleRp { get; } = new ReactiveProperty<bool>() {Value = false};

    [Binding]
    public bool IsVisible { get; private set; }

    private bool UseOptionalScenes { get; set; }

    public static bool UseOptionalScenesInEditorMode
    {
        get => UniquePlayerPrefs.GetBool(UseOptionalScenesInEditorModeKey, true);
        set => UniquePlayerPrefs.SetBool(UseOptionalScenesInEditorModeKey, value);
    }

    public bool UseCustomSettings { get; set; }



    private static string ArgNameOverridenLongTermToken = "-gt";

    private string OverridenLongTermToken => Environment.GetCommandLineArgs().Any(x => x == ArgNameOverridenLongTermToken)
        ? Environment.GetCommandLineArgs()[Environment.GetCommandLineArgs().IndexOf(ArgNameOverridenLongTermToken) + 1]
        : null;

    private static string ArgNameLongTermStream = "-gs";

    private string ProvidedLongTermStreamArg => Environment.GetCommandLineArgs().Any(x => x == ArgNameLongTermStream)
        ? Environment.GetCommandLineArgs()[Environment.GetCommandLineArgs().IndexOf(ArgNameLongTermStream) + 1]
        : null;

    private LoginInterop.VerifyUserLoginTokenResponse SavedResponseWithServersData { get; set; }



    private string _lastServerErrorMessage;

    [Binding]
    public string LastServerErrorMessage
    {
        get { return _lastServerErrorMessage; }
        private set
        {
            if (_lastServerErrorMessage != value)
            {
                _lastServerErrorMessage = value;
                NotifyPropertyChanged();
            }
        }
    }

    private bool _isValidMapPortIp;

    [Binding]
    public bool IsValidMapPortIp
    {
        get { return _isValidMapPortIp; }
        private set
        {
            if (_isValidMapPortIp != value)
            {
                _isValidMapPortIp = value;
                NotifyPropertyChanged();
            }
        }
    }

    private bool _isValidTokenMapPortIp;

    [Binding]
    public bool IsValidTokenMapPortIp
    {
        get { return _isValidTokenMapPortIp; }
        private set
        {
            if (_isValidTokenMapPortIp != value)
            {
                _isValidTokenMapPortIp = value;
                NotifyPropertyChanged();
            }
        }
    }

    private LocalizedString _loginOutButtonLs;

    [Binding]
    public LocalizedString LoginOutButtonLs
    {
        get => _loginOutButtonLs;
        private set
        {
            if (!_loginOutButtonLs.Equals(value))
            {
                _loginOutButtonLs = value;
                NotifyPropertyChanged();
            }
        }
    }

    private LocalizedString _connectButtonLs;

    [Binding]
    public LocalizedString ConnectButtonLs
    {
        get => _connectButtonLs;
        private set
        {
            if (!_connectButtonLs.Equals(value))
            {
                _connectButtonLs = value;
                NotifyPropertyChanged();
            }
        }
    }


    private const string BotDefaultConfigPath = "/Bots/SimpleBot";
    private BotActionDef _botDefaultConfig;

    private CancellationTokenSource _loginCts;
    private Task _loginTask = Task.CompletedTask;

    private CancellationTokenSource _validateCts;
    private Task _validateTask = Task.CompletedTask;

    private string _customIp;
    private string _customPort;


    //=== Unity ===============================================================

    private void Awake()
    {
        UseOptionalScenes = !Application.isEditor || UseOptionalScenesInEditorMode;
    }

    private void Start()
    {
        _uiAkAudioListener.AssertIfNull(nameof(_uiAkAudioListener));
        _connectionLkh.AssertIfNull(nameof(_connectionLkh));
        CreditsContr.AssertIfNull(nameof(CreditsContr));

        LoginOutButtonLs = GetLoginOutButtonLs();
        ConnectButtonLs = GetConnectButtonLs();
        CurrentDedicAddress = DefaultDedicAddress;
        CurrentDedicPort = DefaultDedicPort;

        Bind(IsVisibleRp, () => IsVisible);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameCamera.OnCameraCreated -= GameCameraNotifier_OnCameraCreated;
        GameCamera.OnCameraDestroyed -= GameCameraNotifier_OnCameraDestroyed;
    }

    //=== Public ==============================================================
    [Cheat]
    public static void use_optional_scenes_in_editor(bool isOn)
    {
        UseOptionalScenesInEditorMode = isOn;
    }

    public void SetStartup(StartupParams startupParams)
    {
        _startupParams = startupParams;

        if (_startupParams != null && _startupParams.AutoConnect)
            AutoConnect();
    }

    public void OnRestartGameButton()
    {
        AppStart.Restart();
    }

    [UsedImplicitly]
    public void OnQuitGameButton()
    {
        GameState.ExitGame();
        AppStart.Quit();
    }

    [UsedImplicitly]
    public void OnCreditsButton()
    {
        //TODO
    }

    /// <summary>
    /// Не используется с 22.04
    /// </summary>
    public void OnLoginLogoutButton()
    {
        if (_loginCts != null && !_loginCts.IsCancellationRequested)
            _loginCts.Cancel();

        if (_validateCts != null && !_validateCts.IsCancellationRequested)
            _validateCts.Cancel();

        //if (WatermarkScreen.Instance != null) WatermarkScreen.Instance.SetDefaultParams();

        _customPort = null;
        _customIp = null;
    }

    //Button "Connect"
    public void OnFromLobbyToClusterButton()
    {
        SetServerError(ConnectToClusterImpl());
    }

    //Button "Host"
    public void OnFromLobbyToHostClusterButton() //Button "Start host"
    {
        SetServerError(HostClusterImpl());
    }

    //Button "Connect without token"
    public void OnFromLobbyToClusterWithoutTokenButton() //Button "Connect without token"
    {
        SetServerError(ConnectToClusterWithoutTokenImpl());
    }

    public void InteractableButtonsCheck()
    {
        IsValidMapPortIp = GetIsValidMapPortIp();
        IsValidTokenMapPortIp = GetIsValidTokenMapPortIp();
    }

    public void DebugMakeError() //DEBUG 2del После починки показа логов
    {
        try
        {
            UI.Logger.IfDebug()?.Message($"[{Time.frameCount}] >>>> MoveItem_ClusterCommand(1)").Write();
            //Debug.LogAssertion($"[{Time.frameCount}] >>>> MoveItem_ClusterCommand(2)");
            UI.Logger.IfInfo()?.Message($"[{Time.frameCount}] >>>> MoveItem_ClusterCommand(3)").Write();
            throw new Exception("DebugMakeError Exception");
        }
        catch (Exception e)
        {
            UI.Logger.IfError()?.Message($"{e}").Write();
        }
    }

    public void ShowSettingsWindow()
    {
        WindowsManager.Open(_settingsWindowId);
    }


    //=== Protected ===========================================================

    public override void AfterDependenciesInjectedOnAllProviders()
    {
        StartGameGuiNode.AssertIfNull(nameof(StartGameGuiWindow));

        _guiCamera = GameObject.Find("GuiCamera").GetComponent<Camera>();
        _guiCamera.AssertIfNull(nameof(_guiCamera));

        GameCamera.OnCameraCreated += GameCameraNotifier_OnCameraCreated;
        GameCamera.OnCameraDestroyed += GameCameraNotifier_OnCameraDestroyed;
        _guiCamera.gameObject.SetActive(GameCamera.Camera == null);

        InteractableButtonsCheck();
    }


    //=== Private =============================================================
    private void AutoConnect()
    {
        var autoConnectLoaderToken = LoadingScreenUtility.Show(nameof(AutoConnect));
        SetServerError(AutoConnectImpl(autoConnectLoaderToken));
    }

    private async Task AutoConnectImpl(LoadingScreenNode.Token autoConnectLoaderToken)
    {
        using (autoConnectLoaderToken)
        {
            var serverConfigDef = GameState.LoginServer.Target;
            var startupConnectionParams = _startupParams.ConnectionParams;
            var startupPlayParams = _startupParams.PlayParams;


            var connectionParams = new ConnectionParams(
                startupConnectionParams.ConnectionAddress ?? CurrentDedicAddress,
                startupConnectionParams.ConnectionPort != 0 ? startupConnectionParams.ConnectionPort : CurrentDedicPort,
                Version,
                _botDefaultConfig,
                "/SpawnSystem/SpawnPointTypes/Dropzone",
                startupConnectionParams.Code,
                startupConnectionParams.UserId
            );
            await AsyncUtils.RunAsyncTask(
                async () =>
                {
                    await GameState.Connect(
                        this,
                        serverConfigDef.VerifyServerAddress,
                        connectionParams,
                        startupPlayParams,
                        true
                    );
                }
            );
        }
    }

    private async Task ConnectToClusterImpl()
    {
        if (BotControl)
            LoadBotDef();

        var serverConfigDef = GameState.LoginServer.Target;
        var startupConnectionParams = _startupParams.ConnectionParams;
        var startupPlayParams = _startupParams.PlayParams;

        if (string.IsNullOrWhiteSpace(startupConnectionParams.UserId))
            startupConnectionParams = startupConnectionParams.With(CurrentDevUserName);

        var connectionParams = new ConnectionParams(
            startupConnectionParams.ConnectionAddress ?? CurrentDedicAddress,
            startupConnectionParams.ConnectionPort != 0 ? startupConnectionParams.ConnectionPort : CurrentDedicPort,
            Version,
            _botDefaultConfig,
            "/SpawnSystem/SpawnPointTypes/Dropzone",
            startupConnectionParams.Code,
            startupConnectionParams.UserId
        );


        await AsyncUtils.RunAsyncTask(
            () => GameState.Connect(
                this,
                GameState.LoginServer.Target.VerifyServerAddress,
                connectionParams,
                startupPlayParams
            )
        );
    }

    private async Task ConnectToClusterWithoutTokenImpl()
    {
        if (BotControl)
            LoadBotDef();
        var serverConfigDef = GameState.LoginServer.Target;
        var startupConnectionParams = _startupParams.ConnectionParams;
        var startupPlayParams = _startupParams.PlayParams;

        var connectionParams = new ConnectionParams(
            startupConnectionParams.ConnectionAddress ?? CurrentDedicAddress,
            startupConnectionParams.ConnectionPort != 0 ? startupConnectionParams.ConnectionPort : CurrentDedicPort,
            Version,
            _botDefaultConfig,
            "/SpawnSystem/SpawnPointTypes/Dropzone",
            startupConnectionParams.Code,
            startupConnectionParams.UserId
        );

        await AsyncUtils.RunAsyncTask(
            () => GameState.Connect(
                this,
                GameState.LoginServer.Target.VerifyServerAddress,
                connectionParams,
                startupPlayParams
            )
        );
    }

    private async Task HostClusterImpl()
    {
        if (BotControl)
            LoadBotDef();

        var config = _mapRoot.Target.HostConfig.Target;
        var shared = _mapRoot.Target.HostShared.Target;

        var host = "127.0.0.1";
        var port = shared.ClientEntryPoint.Target.Port;
        var serverConfigDef = GameState.LoginServer.Target;
        var startupConnectionParams = _startupParams.ConnectionParams;
        var startupPlayParams = _startupParams.PlayParams;
        if (string.IsNullOrWhiteSpace(startupConnectionParams.UserId))
            startupConnectionParams = startupConnectionParams.With(CurrentDevUserName);
        var connectionParams = new ConnectionParams(
            host,
            port,
            Version,
            _botDefaultConfig,
            "/SpawnSystem/SpawnPointTypes/Dropzone",
            startupConnectionParams.Code,
            startupConnectionParams.UserId
        );

        await AsyncUtils.RunAsyncTask(
            () => GameState.Host(
                this,
                GameState.LoginServer.Target.VerifyServerAddress,
                config,
                shared,
                connectionParams
            )
        );
    }

    private async void SetServerError(Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException e)
        {
            Logger.IfWarn()?.Exception(e).Write();
            await UnityQueueHelper.RunInUnityThread(() => LastServerErrorMessage = e.Message);
        }
        catch (Exception e)
        {
            Logger.IfError()?.Exception(e).Write();
            await UnityQueueHelper.RunInUnityThread(() => LastServerErrorMessage = e.Message);
        }
    }

    private void GameCameraNotifier_OnCameraCreated(Camera _)
    {
        _guiCamera.gameObject.SetActive(false);
        VegetationStudioManager.SetVegetationCamera(GameCamera.Camera);
    }

    private void GameCameraNotifier_OnCameraDestroyed()
    {
        if (_guiCamera)
            _guiCamera.gameObject.SetActive(true);
    }



    private LocalizedString GetLoginOutButtonLs()
    {
        return /*_connectionLkh.Ls4 : */_connectionLkh.Ls3; //"Logout" : "Login";
    }

    private LocalizedString GetConnectButtonLs()
    {
        return _connectionLkh.Ls1; //Connect / Connect as
    }

    private bool GetIsValidMapPortIp()
    {
        return CurrentDedicPort > 0 &&
               !string.IsNullOrWhiteSpace(CurrentDedicAddress);
    }

    private bool GetIsValidTokenMapPortIp()
    {
        return IsValidMapPortIp;
    }

    private void LoadBotDef()
    {
        _botDefaultConfig = ResourcesSystem.Loader.GameResourcesHolder.Instance.LoadResource<BotActionDef>(BotDefaultConfigPath);
    }

    
    void Update()
    {
        if (LastServerErrorMessage != GameState.Instance.NetIssueText)
            LastServerErrorMessage = GameState.Instance.NetIssueText;
    }

    // public Texture2D ServerButtonTexture;
    // public Texture2D SelectedServerButtonTexture;
    // public Texture2D FriendTexture;
    // public Texture2D PreviousServerTexture;
    // public Font ServerButtonFont;
    private StartupParams _startupParams;

    // void SetButtonCommon()
    // {
    //     GUI.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 1f);
    //     GUI.skin.button.normal.background = ServerButtonTexture;
    //     GUI.skin.button.hover.background = ServerButtonTexture;
    //     GUI.skin.button.active.background = ServerButtonTexture;
    // }
    //
    // void SetButtonSelected()
    // {
    //     GUI.color = new UnityEngine.Color(1f, 1f, 1f, 1f);
    //     GUI.skin.button.normal.background = SelectedServerButtonTexture;
    //     GUI.skin.button.hover.background = SelectedServerButtonTexture;
    //     GUI.skin.button.active.background = SelectedServerButtonTexture;
    // }

//     private void OnGUI()
//     {
//         if (SavedResponseWithServersData != null && SavedResponseWithServersData.data != null && _isVisible)
//         {
//             GUI.skin.button.font = ServerButtonFont;
//             SetButtonCommon();
//             GUILayout.BeginVertical();
//             if ((SavedResponseWithServersData.data.previous_servers != null &&
//                  SavedResponseWithServersData.data.previous_servers.Length != 0) ||
//                 (SavedResponseWithServersData.data.friend_servers != null && SavedResponseWithServersData.data.friend_servers.Length != 0))
//                 GUILayout.Label("Select server");
//             bool hasAny = false;
//             if (SavedResponseWithServersData.data.previous_servers != null)
//                 foreach (var prevServ in SavedResponseWithServersData.data.previous_servers)
//                 {
//                     if ((_customIp == prevServ.ip && _customPort == prevServ.port) ||
//                         (_customIp == null &&
//                          _customPort == null &&
//                          prevServ.ip == SavedResponseWithServersData.data.server_ip &&
//                          prevServ.port == SavedResponseWithServersData.data.server_port.ToString()))
//                     {
//                         SetButtonSelected();
//                     }
//
//                     if (GUILayout.Button(
//                         new GUIContent($"{prevServ.name}", PreviousServerTexture, "Previous server"),
//                         GUILayout.Width(200),
//                         GUILayout.Height(40)))
//                     {
//                         _customIp = prevServ.ip;
//                         _customPort = prevServ.port;
//                     }
//
//                     hasAny = true;
//                     SetButtonCommon();
//                 }
//
//             if (SavedResponseWithServersData.data.friend_servers != null)
//                 foreach (var friendServer in SavedResponseWithServersData.data.friend_servers)
//                 {
//                     if ((_customIp == friendServer.server.ip && _customPort == friendServer.server.port) ||
//                         (_customIp == null &&
//                          _customPort == null &&
//                          friendServer.server.ip == SavedResponseWithServersData.data.server_ip &&
//                          friendServer.server.port == SavedResponseWithServersData.data.server_port.ToString()))
//                     {
//                         SetButtonSelected();
//                     }
//
//                     if (GUILayout.Button(
//                         new GUIContent($"{friendServer.name} {friendServer.server.name}", FriendTexture, "Play with a friend"),
//                         GUILayout.Width(200),
//                         GUILayout.Height(40)))
//                     {
//                         _customIp = friendServer.server.ip;
//                         _customPort = friendServer.server.port;
//                     }
//
//                     hasAny = true;
//                     SetButtonCommon();
//                 }
//
//             /*if (hasAny)
//             {
//
//                 if (_customIp == null && _customPort == null)
//                     SetButtonSelected();
//                 else
//                     SetButtonCommon();
//
//                 if (GUILayout.Button(new GUIContent($"Default", PreviousServerTexture, "Default server"), GUILayout.Width(200), GUILayout.Height(40)))
//                 {
//                     _customIp = null;
//                     _customPort = null;
//                 }
//             }*/
//             SetButtonCommon();
//             if (GUI.tooltip != null)
//                 GUILayout.Label(GUI.tooltip, GUILayout.Width(200), GUILayout.Height(40));
//             GUILayout.EndVertical();
//         }
//     }
}