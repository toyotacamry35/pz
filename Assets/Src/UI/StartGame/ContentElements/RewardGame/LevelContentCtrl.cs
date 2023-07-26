using System;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using DG.Tweening;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class LevelContentCtrl : BindingController<LevelContentVM>
    {
        private readonly ReactiveProperty<float> _currentBgAlphaRp = new ReactiveProperty<float>();
        private readonly ReactiveProperty<int> _levelExpRp = new ReactiveProperty<int>();
        private readonly ReactiveProperty<int> _levelMaxExpRp = new ReactiveProperty<int>();

        private readonly ReactiveProperty<int> _levelRp = new ReactiveProperty<int>();
        private readonly ReactiveProperty<int> _levelTargetExpRp = new ReactiveProperty<int>();
        private readonly ReactiveProperty<float> _levelUpBgAlphaRp = new ReactiveProperty<float>();

        private DisposableComposite _stateD;
        private IStream<int> _currentTotalExpStream;
        private IStream<int> _incomingTotalExpStream;
        private int _newLevel;
        private int _newLevelExp;

        public ReactiveProperty<bool> ConsumeButtonEnabled { get; } = new ReactiveProperty<bool>();

        [Binding, UsedImplicitly]
        public int Level { get; set; }

        [Binding, UsedImplicitly]
        public int LevelExp { get; set; }

        [Binding, UsedImplicitly]
        public int LevelMaxExp { get; set; }

        [Binding, UsedImplicitly]
        public float LevelExpAmount { get; set; }

        [Binding, UsedImplicitly]
        public float LevelTargetExpAmount { get; set; }

        [Binding, UsedImplicitly]
        public float CurrentBgAlpha { get; set; }

        [Binding, UsedImplicitly]
        public float LevelUpBgAlpha { get; set; }

        private int CurrentTotalExp => LevelUpDatasHelpers.CalcTotalExpNeededToReachLvl(_levelRp.Value) + _levelExpRp.Value;

        private void Awake()
        {
            _currentTotalExpStream = Vmodel.SubStream(D, vm => vm.CurrentTotalExp, -1);
            _incomingTotalExpStream = Vmodel.SubStream(D, vm => vm.IncomingTotalExp, -1);

            Bind(_levelRp, () => Level);
            Bind(_levelExpRp, () => LevelExp);
            Bind(_levelMaxExpRp, () => LevelMaxExp);
            var levelExpAmount = _levelMaxExpRp
                .Zip(D, _levelExpRp)
                .Func(D, (max, current) => Mathf.Clamp((float) current / max, 0, 1));
            Bind(levelExpAmount, () => LevelExpAmount);

            var levelTargetExpAmount = _levelMaxExpRp
                .Zip(D, _levelTargetExpRp)
                .Func(D, (max, current) => Mathf.Clamp((float) current / max, 0, 1));
            Bind(levelTargetExpAmount, () => LevelTargetExpAmount);
            Bind(_currentBgAlphaRp, () => CurrentBgAlpha);
            Bind(_levelUpBgAlphaRp, () => LevelUpBgAlpha);

            Vmodel.Action(D, vm => { SetState(vm == null ? State.Idle : State.SetupCurrent); });
        }

        private void SetState(State state)
        {
            if (_stateD != null)
                D.DisposeInnerD(_stateD);
            _stateD = D.CreateInnerD();
            switch (state)
            {
                case State.SetupCurrent:
                    SetupCurrent(_stateD);
                    break;
                case State.WaitIncoming:
                    WaitIncoming(_stateD);
                    break;
                case State.AddExp:
                    AddExp(_stateD);
                    break;
                case State.Idle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void SetupCurrent(DisposableComposite localD)
        {
            _currentTotalExpStream
                .Action(
                    localD,
                    exp =>
                    {
                        if (exp == -1)
                            return;

                        var (level, levelExp, levelMaxEp) = CreateExpStatus(exp);

                        //Pass Result To Outer Scope
                        _levelRp.Value = level;
                        _levelExpRp.Value = levelExp;
                        _levelMaxExpRp.Value = levelMaxEp;
                        _currentBgAlphaRp.Value = 1f;
                        _levelUpBgAlphaRp.Value = 0f;
                        ConsumeButtonEnabled.Value = false;

                        SetState(State.WaitIncoming);
                    }
                );
        }

        private void WaitIncoming(DisposableComposite localD)
        {
            _incomingTotalExpStream.Action(
                localD,
                targetTotalExp =>
                {
                    if (targetTotalExp == -1)
                        return;

                    var deltaExp = targetTotalExp - CurrentTotalExp;
                    if (deltaExp != 0)
                    {
                        //Pass Result To Outer Scope
                        _newLevelExp = Mathf.Clamp(_levelExpRp.Value + deltaExp, 0, _levelMaxExpRp.Value);
                        _newLevel = Mathf.Clamp(
                            _levelRp.Value + (_newLevelExp == 0 ? -1 : _newLevelExp == _levelMaxExpRp.Value ? 1 : 0),
                            0,
                            LevelUpDatasHelpers.MaxLevel
                        );
                        if (_newLevelExp != _levelExpRp.Value || _newLevel != _levelRp.Value)
                        {
                            SetState(State.AddExp);
                            return;
                        }
                    }

                    ConsumeButtonEnabled.Value = true;
                }
            );
        }

        private void AddExp(DisposableComposite localD)
        {
            ConsumeButtonEnabled.Value = false;
            var needLevelUp = _newLevel != _levelRp.Value;

            const float totalGrowDuration = 1.2f;
            var sequence = DOTween.Sequence();
            localD.Add(new DisposeAgent(sequence.KillIfExistsAndActive));
            sequence
                .Append(
                    DOTween.To(
                        () => _levelTargetExpRp.Value,
                        value => _levelTargetExpRp.Value = value,
                        _newLevelExp,
                        totalGrowDuration
                    )
                );
            if (needLevelUp)
                sequence.AppendCallback(
                    () => { _levelRp.Value = _newLevel; }
                );
            sequence.Append(
                DOTween.To(
                    () => _levelExpRp.Value,
                    value => _levelExpRp.Value = value,
                    _newLevelExp,
                    totalGrowDuration
                )
            );
            if (needLevelUp)
                sequence.AppendCallback(
                    () =>
                    {
                        _levelExpRp.Value = 0;
                        _levelTargetExpRp.Value = 0;
                        _levelMaxExpRp.Value = LevelUpDatasHelpers.GetDeltaExpNeededToGetNextLevel(_newLevel);
                    }
                );
            if (needLevelUp)
                sequence
                    .Insert(
                        0f,
                        DOTween.To(
                            () => _currentBgAlphaRp.Value,
                            value => _currentBgAlphaRp.Value = value,
                            0f,
                            totalGrowDuration
                        )
                    )
                    .Join(
                        DOTween.To(
                            () => _levelUpBgAlphaRp.Value,
                            value => _levelUpBgAlphaRp.Value = value,
                            1f,
                            totalGrowDuration
                        )
                    )
                    .Append(
                        DOTween.To(
                            () => _levelUpBgAlphaRp.Value,
                            value => _levelUpBgAlphaRp.Value = value,
                            0f,
                            totalGrowDuration
                        )
                    )
                    .Join(
                        DOTween.To(
                            () => _currentBgAlphaRp.Value,
                            value => _currentBgAlphaRp.Value = value,
                            1f,
                            totalGrowDuration
                        )
                    );
            sequence.AppendCallback(
                () => { SetState(State.WaitIncoming); });
        }

        private static (int level, int levelExp, int levelMaxEp) CreateExpStatus(int totalExp)
        {
            var level = LevelUpDatasHelpers.CalcAccLevel(totalExp);
            var levelMaxExp = LevelUpDatasHelpers.GetDeltaExpNeededToGetNextLevel(level);
            var expToReachCurrLvl = LevelUpDatasHelpers.CalcTotalExpNeededToReachLvl(level);
            var levelExp = totalExp - expToReachCurrLvl;
            return (level, levelExp, levelMaxExp);
        }

        private enum State
        {
            Idle,
            SetupCurrent,
            WaitIncoming,
            AddExp
        }
    }
}