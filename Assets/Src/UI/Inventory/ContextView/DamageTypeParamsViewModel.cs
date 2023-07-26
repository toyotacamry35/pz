using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class DamageTypeParamsViewModel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private DamageTypeDefRef _damageTypeDefRef;


        //=== Props ===========================================================

        private LocalizedString _name;

        [Binding]
        public LocalizedString Name
        {
            get => _name;
            set
            {
                if (!_name.Equals(value))
                {
                    _name = value;
                    NotifyPropertyChanged();
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


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_damageTypeDefRef.Target.AssertIfNull(nameof(_damageTypeDefRef)))
                return;

            Icon = _damageTypeDefRef.Target.Sprite?.Target;
            Name = _damageTypeDefRef.Target.DisplayNameLs;
        }
    }
}