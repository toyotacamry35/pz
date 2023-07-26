using System.ComponentModel;
using System.IO;
using Assets.Src.Debugging.Log;
using ColonyDI;
using Core.Cheats;
using Core.Environment.Logging;
using Core.Environment.Logging.Extension;
using Core.Environment.Resources;
using JetBrains.Annotations;
using LoggerExtensions;
using NLog;
using ReactivePropsNs;
using SharedCode.Repositories;
using Src.Debugging;
using TMPro;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class DevelopLobbyGuiNode : DependencyEndNode
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private const string DevUserName = "DevUserName";
        private const string DedicAddressKey = "ConnectionAddress";
        private const string DedicPortKey = "ConnectionPort";

        [SerializeField, UsedImplicitly]
        private TMP_InputField _dedicAddressInput;

        [SerializeField, UsedImplicitly]
        private TMP_InputField _dedicPortInput;

        [SerializeField, UsedImplicitly]
        private TMP_InputField _customSceneInput;

        private static RestServer _restServer { get; set; }

        private const string FallbackConfig =
@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd"" 
  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" autoReload=""true"" throwExceptions=""false"" throwConfigExceptions=""false"" internalLogLevel=""Off"">


  <variable name=""basedir"" value="""" />
  <variable name=""base_file_name"" value=""${var:basedir}/game"" />

  <targets async=""true"">

    <target xsi:type=""File"" name=""file_target"" filename=""${var:base_file_name}.log"" layout=""${date:format=o}|${level:uppercase=true}|${logger}|${message}${onexception:${newline}${exception:format=tostring}}|${callsiteza}"" forceManaged=""true"" concurrentWrites=""false"" keepFileOpen=""true"" />

  </targets>

  <rules>
    <logger name=""Telemetry-*"" minlevel=""Info"" final=""true"" enabled=""true""/>

    <logger name=""LocomotionDamperNode"" minlevel=""Debug"" final=""true"" enabled=""false"" />
    <logger name=""LocomotionDamperNode"" minlevel=""Debug"" final=""true"" />

    <logger name=""SpellSystem"" maxlevel=""Warn"" final=""true"" />
    <logger name=""UnitySynchronizationContext"" minlevel=""Debug"" final=""true"" />
    <logger name=""Microsoft.AspNetCore.*"" maxLevel=""Info"" final=""true""/>

    <logger name=""Wwise"" minlevel=""Debug"" final=""true"" enabled=""true""/>

    <logger name=""*"" minlevel=""Info"" writeTo=""file_target"" />    
  </rules>
  
</nlog>";

        private const string LogConfigPathClient = "System/NLog.config-client.xml";
        private const string LogConfigPathPlayMode = "System/NLog.config-play.xml";
        private const string LogConfigPathDedic = "System/NLog.config-dedic.xml";

        //=== Props ===========================================================

        [Dependency, UsedImplicitly]
        private LobbyGuiNode LobbyGuiNode { get; set; }

        [Binding]
        public bool IsVisible { get; private set; }

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

        public static string HACK_MAP_NAME_FROM_DEV;
        public string SavedMapName
        {
            get { return UniquePlayerPrefs.GetString(DevUserName, LobbyGuiNode.DefaultDevUserName); }
            set
            {
                HACK_MAP_NAME_FROM_DEV = value;
                UniquePlayerPrefs.SetString(DevUserName, value);
                LobbyGuiNode.CurrentDevUserName = value;
                if (_customSceneInput.text != value)
                    _customSceneInput.text = value;
            }
        }

        public string SavedDedicAddress
        {
            get { return UniquePlayerPrefs.GetString(DedicAddressKey, LobbyGuiNode.DefaultDedicAddress); }
            set
            {
                UniquePlayerPrefs.SetString(DedicAddressKey, value);
                LobbyGuiNode.CurrentDedicAddress = value;
                if (_dedicAddressInput.text != value)
                    _dedicAddressInput.text = value;
            }
        }

        public int SavedDedicPort
        {
            get { return UniquePlayerPrefs.GetInt(DedicPortKey, LobbyGuiNode.DefaultDedicPort); }
            set
            {
                UniquePlayerPrefs.SetInt(DedicPortKey, value);
                LobbyGuiNode.CurrentDedicPort = value;
                var asString = value.ToString();
                if (_dedicPortInput.text != asString)
                    _dedicPortInput.text = asString;
            }
        }

        private bool BotControl { get; set; } = false;

        [Binding]
        public bool IsBot
        {
            get { return BotControl; }
            set
            {
                if (BotControl != value)
                {
                    BotControl = value;
                    if (LobbyGuiNode)
                        LobbyGuiNode.BotControl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void InitLogger()
        {
            if (PlayerPrefs.GetString("Logger") == "Enabled")
            {
                Debug.LogWarning("LOG SYSTEM DISABLED BY REQUEST");
                PlayerPrefs.SetString("Logger", "Disabled");
            }
            else
            {
                Debug.LogWarning("LOG SYSTEM ENABLED BY REQUEST");
                PlayerPrefs.SetString("Logger", "Enabled");
                Init_Log_System();
            }
        }

        //=== Awake ===========================================================

        private void Awake()
        {
            _dedicAddressInput.AssertIfNull(nameof(_dedicAddressInput));
            _dedicPortInput.AssertIfNull(nameof(_dedicPortInput));
            _customSceneInput.AssertIfNull(nameof(_customSceneInput));
            NLog.LogManager.ConfigurationChanged += LogManager_ConfigurationChanged;
        }

        private void LogManager_ConfigurationChanged(object sender, NLog.Config.LoggingConfigurationChangedEventArgs e)
        {
            CanEnableLogger = e.ActivatedConfiguration == null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_restServer != null)
            {
                _restServer.Dispose();
                _restServer = null;
            }

            LogManager.Flush();
            LogManager.Shutdown();
        }

        private bool _canEnableLogger = true;
        [Binding]
        public bool CanEnableLogger
        {
            get => true;// _canEnableLogger;
            set
            {
                if (_canEnableLogger != value)
                {
                    _canEnableLogger = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Cheat]
        public static void Init_Log_System()
        {
            Debug.LogWarning("LOG SYSTEM ENABLED");
            bool noGraphics = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
            bool isEditor = Application.isEditor;

            var cfgFile = noGraphics
                ? LogConfigPathDedic
                : isEditor
                    ? LogConfigPathPlayMode
                    : LogConfigPathClient;

            var resourceRoot = ResourceSystemInit.ResourceRootDefaultLocation;
            var logConfigFile = new FileInfo(Path.Combine(resourceRoot.FullName, cfgFile));

            LogSystemInit.Init();
            if (logConfigFile.Exists)
            {
                Debug.Log("Initializaing Log system with regular config");
                LogInitializer.InitLogger(logConfigFile);
            }
            else
            {
                Debug.Log("Initializaing Log system with fallback config");
                using (var stream = new StringReader(FallbackConfig))
                    LogInitializer.InitLogger(stream, null, new DirectoryInfo(Path.Combine(Application.dataPath, "..", "..", "..", "..")));
            }
            _restServer = RestServer.Create(false);

            Logger.IfInfo()?.Message("Build version: {0} Network version: {1}",  GeneratedCode.VersionHelper.GeneratedCodeVersion.GitCommitId(), ReplicaTypeRegistry.NetworkHash).Write();
        }

        //=== Public ==========================================================

        public void OnDefaultServerAddressButton()
        {
            SavedDedicAddress = LobbyGuiNode.DefaultDedicAddress;
        }

        public void OnDefaultDedicPortButton()
        {
            SavedDedicPort = LobbyGuiNode.DefaultDedicPort;
        }

        public void OnDefaultSceneNameButton()
        {
            SavedMapName = LobbyGuiNode.DefaultDevUserName;
        }

        public void OnFromLobbyToHostClusterButton()
        {
            LobbyGuiNode?.OnFromLobbyToHostClusterButton();
        }

        public void OnFromLobbyToClusterWithoutTokenButton()
        {
            LobbyGuiNode.OnFromLobbyToClusterWithoutTokenButton();
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            SavedDedicAddress = SavedDedicAddress;
            SavedDedicPort = SavedDedicPort;
            SavedMapName = SavedMapName;

            _dedicAddressInput.onValueChanged.AddListener(delegate { OnDedicAddressValueChanged(); });
            _dedicPortInput.onValueChanged.AddListener(delegate { OnDedicPortValueChanged(); });
            _customSceneInput.onValueChanged.AddListener(delegate { OnCustomSceneValueChanged(); });

            LobbyGuiNode.UseCustomSettings = true;
            LobbyGuiNode.BotControl = BotControl;
            IsValidMapPortIp = LobbyGuiNode.IsValidMapPortIp;
            IsValidTokenMapPortIp = LobbyGuiNode.IsValidTokenMapPortIp;
            LobbyGuiNode.PropertyChanged += OnLobbyGuiNodePropertyChanged;
            var isVisibleStream = LobbyGuiNode.IsVisibleRp
                .Zip(D, LobbyGuiNode.CreditsContr.IsVisibleRp)
                .Func(D, (isVisibleLobbyGuiNode, isVisibleCredits) => isVisibleLobbyGuiNode && !isVisibleCredits);
            Bind(isVisibleStream, () => IsVisible);

            CanEnableLogger = NLog.LogManager.Configuration == null;
        }


        //=== Private =========================================================

        private void OnDedicAddressValueChanged()
        {
            SavedDedicAddress = _dedicAddressInput.text;
            LobbyGuiNode.InteractableButtonsCheck();
        }

        private void OnDedicPortValueChanged()
        {
            SavedDedicPort = GetPortFromText(_dedicPortInput.text);
            LobbyGuiNode.InteractableButtonsCheck();
        }

        private int GetPortFromText(string asText)
        {
            int asInt;
            if (!int.TryParse(asText, out asInt))
                return 0;

            return asInt;
        }

        private void OnCustomSceneValueChanged()
        {
            SavedMapName = _customSceneInput.text;
            LobbyGuiNode.InteractableButtonsCheck();
        }

        private void OnLobbyGuiNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LobbyGuiNode.IsValidMapPortIp))
            {
                IsValidMapPortIp = LobbyGuiNode.IsValidMapPortIp;
            }

            if (e.PropertyName == nameof(LobbyGuiNode.IsValidTokenMapPortIp))
            {
                IsValidTokenMapPortIp = LobbyGuiNode.IsValidTokenMapPortIp;
            }
        }
    }
}