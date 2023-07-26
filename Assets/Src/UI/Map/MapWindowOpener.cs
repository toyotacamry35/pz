using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    public class MapWindowOpener : DependencyEndNode
    {
        [SerializeField, UsedImplicitly]
        private WindowId _inventoryWindowId;

        [SerializeField, UsedImplicitly]
        private WindowId _mapWindowId;

        private IGuiWindow _mapGuiWindow;
        private IGuiWindow _inventoryGuiWindow;


        //=== Unity ===========================================================

        private void Awake()
        {
            _inventoryWindowId.AssertIfNull(nameof(_inventoryWindowId));
            _mapWindowId.AssertIfNull(nameof(_mapWindowId));
        }


        //=== Public ==========================================================

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            _inventoryGuiWindow = WindowsManager.GetWindow(_inventoryWindowId);
            _mapGuiWindow = WindowsManager.GetWindow(_mapWindowId);
        }

        public void OpenInventoryUpdate()
        {
            _mapGuiWindow.ClosedHotkeyUpdate(() => { WindowsManager.Close(_inventoryGuiWindow); });
        }

        public void OpenMapUpdate()
        {
            _inventoryGuiWindow.ClosedHotkeyUpdate(() => { WindowsManager.Close(_mapGuiWindow); });
        }

        public void MapGuiWindowDirectOpen()
        {
            WindowsManager.Close(_inventoryGuiWindow);
            WindowsManager.Open(_mapGuiWindow);
        }
    }
}