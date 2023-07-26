using L10n;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class SidePanelRecipeSlotViewModel : BindingViewModel
    {

        //=== Props ===========================================================

        private BaseItemResource _itemResource;

        public BaseItemResource ItemResource
        {
            get => _itemResource;
            set
            {
                if (_itemResource != value)
                {
                    _itemResource = value;
                    NotifyPropertyChanged(nameof(Icon));
                    NotifyPropertyChanged(nameof(ItemName));
                }
            }
        }

        [Binding]
        public Sprite Icon => ItemResource?.Icon.Target;

        [Binding]
        public string ItemName => ItemResource?.ItemNameLs.GetText();

        private int _stack;

        [Binding]
        public int Stack
        {
            get => _stack;
            set
            {
                if (_stack != value)
                {
                    _stack = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isEmpty = true;

        [Binding]
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                if (_isEmpty != value)
                {
                    _isEmpty = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public void SetItemStack(BaseItemResource itemResource, int stack = 0)
        {
            ItemResource = itemResource;
            Stack = stack;
            IsEmpty = Stack == 0;
        }
    }
}