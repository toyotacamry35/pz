using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class MachineCraftSideViewModel : CraftSideViewModel
    {
        public override bool IsMachineTab => true;
    }
}