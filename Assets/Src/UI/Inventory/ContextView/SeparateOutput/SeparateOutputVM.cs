using ReactivePropsNs;

namespace Uins.Inventory
{
    public class SeparateOutputVM : BindingVmodel
    {
        public SurvivalGuiNode SurvivalGui { get; }

        public readonly InventoryNode InventoryNode;

        public SeparateOutputVM(InventoryNode inventoryNode, SurvivalGuiNode survivalGui)
        {
            InventoryNode = inventoryNode;
            SurvivalGui = survivalGui;

            Init();
        }

        private void Init()
        {
        }
    }
}