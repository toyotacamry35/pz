using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Tooltips
{
    [Binding]
    public class BuildRecipeSimpleViewModel : BindingViewModel
    {
        private Sprite _productSprite;

        [Binding]
        public Sprite ProductSprite
        {
            get { return _productSprite; }
            set
            {
                if (_productSprite != value)
                {
                    _productSprite = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isVisble;

        [Binding]
        public bool IsVisble //reusable
        {
            get { return _isVisble; }
            set
            {
                if (_isVisble != value)
                {
                    _isVisble = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}