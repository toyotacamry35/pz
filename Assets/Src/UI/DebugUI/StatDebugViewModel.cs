using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ContainerApis;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class StatDebugViewModel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private UpdateInterval _updateInterval;

        private StatResource _statResource;
        private AnyStatState _anyStatState;


        //=== Props ===========================================================

        private string _statName;

        [Binding]
        public string StatName
        {
            get => _statName;
            set
            {
                if (_statName != value)
                {
                    _statName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _min;

        [Binding]
        public float Min
        {
            get => _min;
            set
            {
                if (!Mathf.Approximately(_min, value))
                {
                    _min = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _max;

        [Binding]
        public float Max
        {
            get => _max;
            set
            {
                if (!Mathf.Approximately(_max, value))
                {
                    _max = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _value;

        [Binding]
        public float Value
        {
            get => _value;
            set
            {
                if (!Mathf.Approximately(_value, value))
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _ratio;

        [Binding]
        public float Ratio
        {
            get => _ratio;
            set
            {
                if (!Mathf.Approximately(_ratio, value))
                {
                    _ratio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _stateInfo;

        [Binding]
        public string StateInfo
        {
            get => _stateInfo;
            set
            {
                if (_stateInfo != value)
                {
                    _stateInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSubscribed;

        public bool IsSubscribed
        {
            get => _isSubscribed;
            set
            {
                if (_isSubscribed != value)
                {
                    _isSubscribed = value;
                    IsVisible = GetIsVisible();
                }
            }
        }

        private bool _isFilteredOut;

        public bool IsFilteredOut
        {
            get => _isFilteredOut;
            set
            {
                if (_isFilteredOut != value)
                {
                    _isFilteredOut = value;
                    IsVisible = GetIsVisible();
                }
            }
        }

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            private set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isBroadcast;

        [Binding]
        public bool IsBroadcast
        {
            get => _isBroadcast;
            private set
            {
                if (_isBroadcast != value)
                {
                    _isBroadcast = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Update()
        {
            if (!_isVisible ||
                _statResource == null ||
                _anyStatState.Kind != StatKind.Time ||
                !_updateInterval.IsItTime() ||
                !_anyStatState.NeeedToBeUpdated)
                return;

            Value = _anyStatState.Value;
            Ratio = GetRatio();
            StateInfo = GetStateInfo();
        }


        //=== Public ==========================================================

        public void Subscribe<T>(StatResource statResource, HasStatsApi<T> hasStatsApi, bool isBroadcast) where T : StatListenerBroadcast, new()
        {
            _statResource = statResource;
            StatName = GetStatName();
            IsBroadcast = isBroadcast;

            if (_statResource.AssertIfNull(nameof(_statResource)))
                return;

            hasStatsApi.SubscribeToStats(_statResource, OnStatChanged);

            IsSubscribed = true;
        }

        public void Unsubscribe<T>(HasStatsApi<T> hasStatsApi) where T : StatListenerBroadcast, new()
        {
            IsSubscribed = false;
            hasStatsApi.UnsubscribeFromStats(_statResource, OnStatChanged);
            OnStatChanged(_statResource, new AnyStatState());
            _statResource = null;
        }

        public void OnFilterChanged(string filter)
        {
            var filterToLower = filter.ToLower();
            IsFilteredOut = filter != "" && !StatName.ToLower().Contains(filterToLower);
        }


        //=== Private =========================================================

        private bool GetIsVisible()
        {
            return IsSubscribed && !IsFilteredOut;
        }

        private void OnStatChanged(StatResource statResource, AnyStatState anyStatState)
        {
            _anyStatState = anyStatState;

            StatName = GetStatName();
            Min = _anyStatState.Min;
            Max = _anyStatState.Max;
            Value = _anyStatState.Value;
            Ratio = GetRatio();
            StateInfo = GetStateInfo();
        }

        private string GetStatName()
        {
            return _statResource?.____GetDebugRootName() ?? "none";
        }

        private float GetRatio()
        {
            return _anyStatState.ZeroBasedRatio;
        }

        private string GetStateInfo()
        {
            return _anyStatState.Kind == StatKind.Time
                ? $"Time={0.001f * (SyncTime.Now - _anyStatState.TimeState.LastBreakPointTime):f0} " +
                  $"Last={_anyStatState.TimeState.LastBreakPointValue:f2}, Rate={_anyStatState.TimeState.ChangeRateCache:f2}"
                : "";
        }
    }
}