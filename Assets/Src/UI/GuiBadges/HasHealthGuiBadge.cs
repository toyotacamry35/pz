using System;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class HasHealthGuiBadge : GuiBadge
    {
        /// <summary>
        /// Интервал проверки времени показа урона на барчике
        /// </summary>
        private const float WaitTimerInterval = 0.2f;

        /// <summary>
        /// Время показа урона на барчике
        /// </summary>
        private const float OrgDamageWaitingTime = 0.9f;

        /// <summary>
        /// Интервал обновления анимации уменьшения урона
        /// </summary>
        private const float RollTimerInterval = 0.002f;

        /// <summary>
        /// Скорость анимации уменьшения урона
        /// </summary>
        private const float OrgDamageRolloutSpeed = 0.003f;

        /// <summary>
        /// Время показа барчика (начиная с последнего изменения здоровья)
        /// </summary>
        private const float HpBarVisibleTime = 10f;

        private DateTime _hpTrackingStartTime;

        private DateTime _lastHpChangingTime;

//        private IStream<DateTime> _waitingTimeStream;
//        private IStream<DateTime> _rollTimeStream;
        private ReactiveProperty<float> _oldHpRatioRp = new ReactiveProperty<float>() {Value = 1};
        private ReactiveProperty<bool> _isVisibleHpRp = new ReactiveProperty<bool>() {Value = false};
        private ReactiveProperty<bool> _isHpTrackedRp = new ReactiveProperty<bool>();
        private ReactiveProperty<float> _shownDamageRatioRp = new ReactiveProperty<float>();
        private bool _isConnectTime;


        //=== Props ===========================================================

        public ReactiveProperty<float> DamageWaitingTimeRp { get; } = new ReactiveProperty<float>() {Value = OrgDamageWaitingTime};

        public ReactiveProperty<float> DamageRolloutSpeedRp { get; } = new ReactiveProperty<float>() {Value = OrgDamageRolloutSpeed};

        [Binding]
        public float HealthRatio { get; protected set; }

        [Binding]
        public float DamageRatio { get; protected set; }

        [Binding]
        public bool IsVisibleHp { get; protected set; }


        //=== Unity ===========================================================

        protected override void Awake() //1
        {
            base.Awake();
            D.Add(_oldHpRatioRp);
            D.Add(_isVisibleHpRp);
            D.Add(_isHpTrackedRp);
            D.Add(_shownDamageRatioRp);
            D.Add(DamageWaitingTimeRp);
            D.Add(DamageRolloutSpeedRp);
        }


        //=== Public ==========================================================

        public override void Connect(IBadgePoint badgePoint)
        {
            base.Connect(badgePoint);

            var hasHealthBadgePoint = badgePoint as IHasHealthBadgePoint;
            if (hasHealthBadgePoint.AssertIfNull(nameof(hasHealthBadgePoint), gameObject))
                return;

            _isConnectTime = true;
            hasHealthBadgePoint.CurrentHealthRp.Action(ConnectionD, hp =>
            {
                if (_isConnectTime)
                {
                    _isConnectTime = false;
                }
                else
                {
                    _lastHpChangingTime = DateTime.Now;
                    _isVisibleHpRp.Value = true;
                }
            });

            var hpRatioStream = hasHealthBadgePoint.CurrentHealthRp
                .Zip(ConnectionD, hasHealthBadgePoint.MaxHealthRp)
                .Func(ConnectionD, (curr, max) => Mathf.Clamp01(Mathf.Approximately(max, 0) ? 1 : curr / max));
            hpRatioStream.Bind(ConnectionD, this, () => HealthRatio);
            _oldHpRatioRp.Value = HealthRatio; //до подписок выравниваем

            hpRatioStream.Subscribe(ConnectionD, hpRatio =>
                {
                    if (!Mathf.Approximately(hpRatio, _oldHpRatioRp.Value))
                    {
                        if (_isHpTrackedRp.Value)
                        {
                            if (hpRatio >= _oldHpRatioRp.Value)
                            {
                                _isHpTrackedRp.Value = false;
                                _oldHpRatioRp.Value = hpRatio;
                            }
                        }
                        else
                        {
                            if (hpRatio < _oldHpRatioRp.Value)
                            {
                                _isHpTrackedRp.Value = true;
                                _hpTrackingStartTime = DateTime.Now;
                                _oldHpRatioRp.Value = _shownDamageRatioRp.Value + hpRatio;
                            }
                            else
                            {
                                _oldHpRatioRp.Value = hpRatio;
                            }
                        }
                    }
                },
                () =>
                {
                    _isHpTrackedRp.Value = false;
                    _oldHpRatioRp.Value = 1;
                    _shownDamageRatioRp.Value = 0;
                });

            var waitingTimeStream = TimeTicker.Instance.GetLocalTimer(WaitTimerInterval);
            waitingTimeStream
                .Where(ConnectionD, dt => _isHpTrackedRp.Value)
                .Action(ConnectionD, dt =>
                {
                    //выход из отслеживания по таймауту
                    if ((DateTime.Now - _hpTrackingStartTime).TotalSeconds > DamageWaitingTimeRp.Value)
                    {
                        _isHpTrackedRp.Value = false;
                        _oldHpRatioRp.Value = HealthRatio;
                    }
                });

            //скрытие барчика здоровья по таймауту
            waitingTimeStream.Subscribe(ConnectionD, dt =>
                {
                    if (_isVisibleHpRp.Value && (DateTime.Now - _lastHpChangingTime).TotalSeconds > HpBarVisibleTime)
                    {
                        _isVisibleHpRp.Value = false;
                    }
                },
                () => _isVisibleHpRp.Value = false);

            var damageRatioStream = _oldHpRatioRp
                .Zip(ConnectionD, hpRatioStream)
                .Func(ConnectionD, (oldHpRatio, hpRatio) => Mathf.Max(oldHpRatio - hpRatio, 0));

            //-- Плавное уменьшение не отслеживаеого более урона
            var rollTimeStream = TimeTicker.Instance.GetLocalTimer(RollTimerInterval);
            rollTimeStream
                .Zip(ConnectionD, damageRatioStream)
                .Action(ConnectionD, (dt, dmgRatio) =>
                {
                    if (Mathf.Approximately(_shownDamageRatioRp.Value, dmgRatio))
                        return;

                    if (_isHpTrackedRp.Value || _shownDamageRatioRp.Value < dmgRatio)
                    {
                        _shownDamageRatioRp.Value = dmgRatio;
                    }
                    else
                    {
                        _shownDamageRatioRp.Value = Mathf.Max(dmgRatio, _shownDamageRatioRp.Value - DamageRolloutSpeedRp.Value);
                    }
                });

            _shownDamageRatioRp.Bind(ConnectionD, this, () => DamageRatio);
            _isVisibleHpRp.Bind(ConnectionD, this, () => IsVisibleHp);
            if (badgePoint.IsDebug)
                _isVisibleHpRp.Log(ConnectionD, $"{transform.FullName()} {nameof(_isVisibleHpRp)}"); //DEBUG
        }
    }
}