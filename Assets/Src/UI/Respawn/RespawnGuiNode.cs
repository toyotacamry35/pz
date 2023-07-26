using ColonyDI;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins.Settings;
using UnityEngine;

namespace Uins
{
    public class RespawnGuiNode : DependencyNodeWithChildren
    {
        [SerializeField, UsedImplicitly]
        private RespawnGuiWindow _respawnGuiWindow;

        [SerializeField, UsedImplicitly]
        public EscMenuGuiWindow _escMenuGuiWindow;

        [SerializeField, UsedImplicitly]
        public ReconnectWindow _reconnectWindow;

        [SerializeField, UsedImplicitly]
        private StuckWindow _stuckWindow;

        [SerializeField, UsedImplicitly]
        private TeleportWindow _teleportWindow;


        //=== Props ===========================================================

        [Dependency]
        private GameState GameState { get; set; }


        //=== Unity ===========================================================
        
        private void Awake()
        {
            _respawnGuiWindow.AssertIfNull(nameof(_respawnGuiWindow));
            _escMenuGuiWindow.AssertIfNull(nameof(_escMenuGuiWindow));
            _reconnectWindow.AssertIfNull(nameof(_reconnectWindow));
            _stuckWindow.AssertIfNull(nameof(_stuckWindow));
            _teleportWindow.AssertIfNull(nameof(_teleportWindow));

            // Inits (don't forget about `ShutDowns` also):
            InputManagerViewModel.Init();
            LanguageSetting.Init();
            ControlsSettingsViewModel.Init();
        }

        protected override void OnDestroy()
        {
            // ShutDowns (don't forget about `Inits` also):
            ControlsSettingsViewModel.ShutDown();
            LanguageSetting.ShutDown();
            InputManagerViewModel.ShutDown();

            base.OnDestroy();
        }

        //=== Public ==========================================================

        public void ChildrenWindowsUpdate()
        {
            _escMenuGuiWindow.OpenCheck();
            _reconnectWindow.ClosedHotkeyUpdate();
            _stuckWindow.ClosedHotkeyUpdate();
            _teleportWindow.ClosedHotkeyUpdate();
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            GameState.IsInGameRp.Action(D, b => _respawnGuiWindow.SwitchWorking(b));
        }
    }
}