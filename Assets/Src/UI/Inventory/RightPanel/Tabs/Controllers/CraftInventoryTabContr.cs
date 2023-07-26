using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class CraftInventoryTabContr : InventoryTabContr
    {
        protected override void OnButtonClickAdditionalActions(bool prevIsOpen)
        {
            Vmodel?.Value?.ContextViewWithParamsVmodel?.SetContext(null); //при клике по табу очищаем контекст
        }
    }
}