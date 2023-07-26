using ColonyDI;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins.Inventory
{
    public class SeparateOutputNode : DependencyEndNode
    {
        [SerializeField, UsedImplicitly] private SeparateOutputCtrl SeparateOutputCtrl;
        [SerializeField, UsedImplicitly] private InventoryNode InventoryNode;

        [Dependency]
        protected SurvivalGuiNode SurvivalGui { get; set; }

        private bool _awakeReady;
        private bool _dependencyReady;

        private void Awake()
        {
            _awakeReady = true;
            TryInit();
        }

        public override void AfterDependenciesInjected()
        {
            _dependencyReady = true;
            TryInit();
        }

        private void TryInit()
        {
            if (!_awakeReady || !_dependencyReady)
                return;

            Init();
        }

        private void Init()
        {
            var vm = new SeparateOutputVM(InventoryNode, SurvivalGui);
            SeparateOutputCtrl.SetVmodel(vm);
        }
    }
}