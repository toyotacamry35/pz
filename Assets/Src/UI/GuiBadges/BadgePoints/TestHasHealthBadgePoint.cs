using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TestHasHealthBadgePoint : BindingViewModel, IHasHealthBadgePoint
    {
        private const float MaxHealthValue = 1000;
        private const float SmallChange = 4;
        private const float MediumChange = 50;
        private const float BigChange = 200;
        private const float RegenPeriod = 0.2f;
        private const float DamageWaitingTimeChangeDelta = 0.1f;
        private const float DamageRolloutSpeedChangeDelta = 0.001f;

        [SerializeField, UsedImplicitly]
        private HasHealthGuiBadge _hasHealthGuiBadge;

        private ReactiveProperty<bool> _isConnectedRp = new ReactiveProperty<bool>() {Value = false};
        private ReactiveProperty<float> _regenValueRp = new ReactiveProperty<float>() {Value = 0};


        //=== Props ===========================================================

        [Binding]
        public bool IsConnected { get; private set; }

        [Binding]
        public float Hp { get; private set; }

        [Binding]
        public float HpRatio { get; private set; }

        [Binding]
        public bool IsRegenActive { get; private set; }

        [Binding]
        public bool IsBleedingActive { get; private set; }

        [Binding]
        public float DamageWaitingTime { get; private set; } //DEBUG

        [Binding]
        public float DamageRolloutSpeed { get; private set; } //DEBUG

        public SharedCode.Utils.Vector3? Anchor => null;

        public bool HasPoint => false;

        public bool IsDebug => false;

        public ReactiveProperty<bool> IsVisibleLogicallyRp { get; } = new ReactiveProperty<bool>() {Value = true};

        public ReactiveProperty<bool> IsSelectedRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public IStream<bool> IsVisibleLogicallyAndSelectedStream { get; private set; }

        public IStream<bool> IsVisibleLogicallyAndUnselectedStream { get; private set; }

        public Vector3 Position => transform.position;

        public ReactiveProperty<float> CurrentHealthRp { get; } = new ReactiveProperty<float>() {Value = MaxHealthValue};

        public ReactiveProperty<float> MaxHealthRp { get; } = new ReactiveProperty<float>() {Value = MaxHealthValue};


        //=== Unity ===========================================================

        private void Awake()
        {
            if (TimeTicker.Instance == null)
                return;

            if (_hasHealthGuiBadge.AssertIfNull(nameof(_hasHealthGuiBadge)))
                return;

            D.Add(CurrentHealthRp);
            D.Add(MaxHealthRp);
            D.Add(IsSelectedRp);
            D.Add(IsVisibleLogicallyRp);
            D.Add(_isConnectedRp);
            D.Add(_regenValueRp);

            Bind(_hasHealthGuiBadge.DamageWaitingTimeRp, () => DamageWaitingTime);
            Bind(_hasHealthGuiBadge.DamageRolloutSpeedRp, () => DamageRolloutSpeed);

            IsVisibleLogicallyAndSelectedStream = IsVisibleLogicallyRp
                .Zip(D, IsSelectedRp)
                .Func(D, (isVisibleLogically, isSelected) => isVisibleLogically && isSelected);

            IsVisibleLogicallyAndUnselectedStream = IsVisibleLogicallyRp
                .Zip(D, IsSelectedRp)
                .Func(D, (isVisibleLogically, isSelected) => isVisibleLogically && !isSelected);

            var timer = TimeTicker.Instance.GetLocalTimer(RegenPeriod);

            timer.Action(D, dt =>
            {
                if (!Mathf.Approximately(0, _regenValueRp.Value))
                    ChangeHealth(_regenValueRp.Value);
            });

            Bind(_isConnectedRp, () => IsConnected);
            Bind(CurrentHealthRp, () => Hp);
            var hpRatioStream = CurrentHealthRp.Zip(D, MaxHealthRp).Func(D, (hp, maxHp) => hp / maxHp);
            Bind(hpRatioStream, () => HpRatio);
            Bind(_regenValueRp.Func(D, regen => regen > 0), () => IsRegenActive);
            Bind(_regenValueRp.Func(D, regen => regen < 0), () => IsBleedingActive);
        }


        //=== Public ==========================================================

        [UsedImplicitly]
        public void OnConnectDisconnectButton()
        {
            if (_isConnectedRp.Value)
            {
                _hasHealthGuiBadge.Disconnect();
            }
            else
            {
                _hasHealthGuiBadge.Connect(this);
            }

            _isConnectedRp.Value = !_isConnectedRp.Value;
        }

        [UsedImplicitly]
        public void OnResetButton()
        {
            _regenValueRp.Value = 0;
            CurrentHealthRp.Value = MaxHealthValue;
        }

        [UsedImplicitly]
        public void OnHpMediumChangeButton(bool isDamage)
        {
            ChangeHealth(isDamage ? -MediumChange : MediumChange);
        }

        [UsedImplicitly]
        public void OnHpBigChangeButton(bool isDamage)
        {
            ChangeHealth(isDamage ? -BigChange : BigChange);
        }

        [UsedImplicitly]
        public void OnRegenButton(bool isDamage)
        {
            if (Mathf.Approximately(_regenValueRp.Value, 0) || isDamage != (_regenValueRp.Value < 0))
            {
                _regenValueRp.Value = isDamage ? -SmallChange : SmallChange;
            }
            else
            {
                _regenValueRp.Value = 0;
            }
        }

        [UsedImplicitly]
        public void OnChangeDamageWaitingTimeButton(bool isIncrease)
        {
            var delta = isIncrease ? DamageWaitingTimeChangeDelta : -DamageWaitingTimeChangeDelta;
            _hasHealthGuiBadge.DamageWaitingTimeRp.Value = Mathf.Max(0, _hasHealthGuiBadge.DamageWaitingTimeRp.Value + delta);
        }

        [UsedImplicitly]
        public void OnChangeDamageRolloutSpeedButton(bool isIncrease)
        {
            var delta = isIncrease ? DamageRolloutSpeedChangeDelta : -DamageRolloutSpeedChangeDelta;
            _hasHealthGuiBadge.DamageRolloutSpeedRp.Value = Mathf.Max(0, _hasHealthGuiBadge.DamageRolloutSpeedRp.Value + delta);
        }
        

        //=== Private ==============================================================

        private void ChangeHealth(float delta)
        {
            CurrentHealthRp.Value = Mathf.Min(Mathf.Max(0, CurrentHealthRp.Value + delta), MaxHealthValue);
        }
    }
}