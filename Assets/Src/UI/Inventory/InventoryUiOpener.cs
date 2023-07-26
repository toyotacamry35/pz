using Assets.Src.Effects.UIEffects;
using Uins.ContainerWindow;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;

namespace Uins
{
    public class InventoryUiOpener : BindingViewModel
    {
        [UsedImplicitly]
        [SerializeField]
        private ContainerGuiWindow _containerGuiWindow;

        private WindowsManager _windowsManager;
        private IGuiWindow _inventoryWindow;


        //=== Props ===============================================================

        public static InventoryUiOpener Instance { get; private set; }


        //=== Unity ===============================================================

        private void Awake()
        {
            _containerGuiWindow.AssertIfNull(nameof(_containerGuiWindow));
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==============================================================

        public void Init(IGuiWindow inventoryWindow, WindowsManager windowsManager)
        {
            if (inventoryWindow.AssertIfNull(nameof(inventoryWindow)) ||
                windowsManager.AssertIfNull(nameof(windowsManager)))
                return;

            _windowsManager = windowsManager;
            _inventoryWindow = inventoryWindow;
        }

        public void OnOpenContainerFromSpell(BaseEffectOpenUi.EffectData effectData, bool isAuthoredContainer, ContainerMode containerType)
        {
            _containerGuiWindow.OnOpenContainerFromSpell(effectData, isAuthoredContainer, containerType);
        }

        public void OnCloseContainerFromSpell(SpellWordCastData cast)
        {
            _containerGuiWindow.OnCloseContainerFromSpell(cast);
        }

        public void OnOpenInventoryMachineTabFromSpell(BaseEffectOpenUi.EffectData effectData)
        {
            _windowsManager.Open(
                _inventoryWindow,
                null,
                new InventoryNode.OpenParams(
                    new CraftSourceVmodel()
                    {
                        Cast = effectData.Cast,
                        TargetOuterRef = effectData.FinalTargetOuterRef,
                        WorldPersonalMachineDef = effectData.StartTargetDef as WorldPersonalMachineDef,
                        Repository = effectData.Repo
                    }));
        }

        public void OnCloseInventoryMachineTabFromSpell(BaseEffectOpenUi.EffectData effectData)
        {
            if (_inventoryWindow.State.Value != GuiWindowState.Closed)
                _windowsManager.Close(_inventoryWindow);
        }
        
        public void CloseInventoryWindow()
        {
            if (_inventoryWindow.State.Value != GuiWindowState.Closed)
                _windowsManager.Close(_inventoryWindow);
        }
    }

    public enum ContainerMode
    {
        None,
        Inventory,
        Bank
    }
}