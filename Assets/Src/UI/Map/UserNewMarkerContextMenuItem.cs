using Assets.ColonyShared.SharedCode.Aspects.Navigation;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class UserNewMarkerContextMenuItem : BindingViewModel
    {
        private UserNewMarkerContextMenu _userNewMarkerContextMenu;


        //=== Props ===========================================================

        private NavIndicatorDef _navIndicatorDef;

        public NavIndicatorDef NavIndicatorDef
        {
            get => _navIndicatorDef;
            private set
            {
                if (_navIndicatorDef != value)
                {
                    _navIndicatorDef = value;
                    Icon = _navIndicatorDef?.MapIcon?.Target;
                }
            }
        }

        private Sprite _icon;

        [Binding]
        public Sprite Icon
        {
            get => _icon;
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public void Init(UserNewMarkerContextMenu userNewMarkerContextMenu, NavIndicatorDef navIndicatorDef)
        {
            _userNewMarkerContextMenu = userNewMarkerContextMenu;
            NavIndicatorDef = navIndicatorDef;
            _userNewMarkerContextMenu.AssertIfNull(nameof(_userNewMarkerContextMenu), gameObject);
            NavIndicatorDef.AssertIfNull(nameof(NavIndicatorDef), gameObject);
        }

        public void OnClick()
        {
            _userNewMarkerContextMenu.OnSubitemClick(this);
        }
    }
}