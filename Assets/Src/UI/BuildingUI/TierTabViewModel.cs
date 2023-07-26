using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TierTabViewModel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private int _selfTier;

        private bool _isAwaken;


        //=== Props ===========================================================

        [Binding]
        public int TabTier => _selfTier;

        private bool _isActiveTab;

        [Binding]
        public bool IsActiveTab
        {
            get { return _isActiveTab; }
            set
            {
                if (_isActiveTab != value)
                {
                    _isActiveTab = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _displayedConstructionsTier;

        public int DisplayedConstructionsTier
        {
            get { return _displayedConstructionsTier; }
            set
            {
                if (_displayedConstructionsTier != value)
                {
                    _displayedConstructionsTier = value;
                    OnDisplayedConstructionsTierChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_selfTier == 0)
            {
                UI.Logger.IfError()?.Message($"'{transform.FullName()}' {nameof(_selfTier)} hasn't value").Write();
                return;
            }

            _isAwaken = true;
            OnDisplayedConstructionsTierChanged();
        }


        //=== Private =========================================================

        private void OnDisplayedConstructionsTierChanged()
        {
            if (!_isAwaken)
                return;

            IsActiveTab = _selfTier == DisplayedConstructionsTier;
        }
    }
}