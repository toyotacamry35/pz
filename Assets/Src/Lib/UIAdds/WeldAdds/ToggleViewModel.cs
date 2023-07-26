using JetBrains.Annotations;
using Uins;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace WeldAdds
{
    [Binding]
    class ToggleViewModel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private Toggle _toggle;


        //=== Props ===========================================================

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_toggle.AssertIfNull(nameof(_toggle)))
                return;

            _toggle.onValueChanged.AddListener(OnToggleChanged);
            IsVisible = _toggle.isOn;
        }


        //=== Private =========================================================

        private void OnToggleChanged(bool isOn)
        {
            IsVisible = isOn;
        }
    }
}