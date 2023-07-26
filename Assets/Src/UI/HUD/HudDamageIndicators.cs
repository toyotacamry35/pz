using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using Assets.Src.Lib.DOTweenAdds;
using ReactivePropsNs;
using Uins;
using UnityWeld.Binding;

[Binding]
public class HudDamageIndicators : BindingViewModel
{
    public PlayerMainStatsViewModel ViewModel;
    public HudWarningFrame HudWarningFrame;

    [Range(0, 10)]
    public float MinDisplayedDamageValue = 1;

    [Range(0.05f, 5)]
    public float Duration = 1;

    [Header("Digits")]
    [Range(0.05f, 0.95f)]
    public float DigitsFadeInPart = .5f;

    [Range(0.05f, 0.95f)]
    public float DigitsFadeOutPart = .5f;

    public TweenSettingsAlpha DigitsFadeInSettings;
    public TweenSettingsAlpha DigitsFadeOutSettings;
    public TweenSettingsVector3 DigitsJumpOutSettings;
    public TweenSettingsFloat DigitsScaleSettings;

    public JumpOutDigit[] JumpOutDigits;

    [Header("Damage sector")]
    public TweenSettingsFloat DamageSectorDisappearingSettings;

    public float ZeroHpRatioAngle;
    public float MaxHpRatioAngle;

    private LinearRelation _angleLinearRelation;

    /// <summary>
    /// здоровье, относительно которого были показаны вылетевшие цифры урона
    /// </summary>
    private float _registeredDamageDigitsHp;

    private int _lastShownJumpOutDigitIndex;

    private Sequence _damageSectorWidthSequence;
    private float _damageShowEndTime;
    private bool _resetOldHp = true;


    //=== Props ==============================================================

    private ReactiveProperty<bool> _isShownDamageRp = new ReactiveProperty<bool>() {Value = false};

    /// <summary>
    /// Показывается ли индикатор урона
    /// </summary>
    [Binding]
    public bool IsShownDamage { get; private set; }

    private ReactiveProperty<float> _hpRatioRp = new ReactiveProperty<float>() {Value = 1};

    /// <summary>
    /// Здоровье, относительно которого показывается урон
    /// </summary>
    private ReactiveProperty<float> _oldHpRatioRp = new ReactiveProperty<float>() {Value = 1};

    [Binding]
    public float HpRatio { get; private set; }

    [Binding]
    public float DamageRatio { get; private set; }

    [Binding]
    public float DamageRotationAngle { get; private set; }


    //=== Unity ==============================================================

    private void Awake()
    {
        ViewModel.AssertIfNull(nameof(ViewModel));
        HudWarningFrame.AssertIfNull(nameof(HudWarningFrame));
        DigitsFadeInSettings.AssertIfNull(nameof(DigitsFadeInSettings));
        DigitsFadeOutSettings.AssertIfNull(nameof(DigitsFadeOutSettings));
        DigitsJumpOutSettings.AssertIfNull(nameof(DigitsJumpOutSettings));
        DigitsScaleSettings.AssertIfNull(nameof(DigitsScaleSettings));
        JumpOutDigits.AssertIfNull(nameof(JumpOutDigits));
        JumpOutDigits.IsNullOrEmptyOrHasNullElements(nameof(JumpOutDigits));
        DamageSectorDisappearingSettings.AssertIfNull(nameof(DamageSectorDisappearingSettings));

        SettingsDurationSetup();
        _angleLinearRelation = new LinearRelation(0, ZeroHpRatioAngle, 1, MaxHpRatioAngle);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        _hpRatioRp.Action(D, DamageIndicatorCheck);
        Bind(_hpRatioRp, () => HpRatio);
        var damageRatioSream = _hpRatioRp
            .Zip(D, _oldHpRatioRp)
            .Func(D, (hpRatio, oldHpRatio) => Mathf.Max(0, oldHpRatio - hpRatio));
        Bind(damageRatioSream, () => DamageRatio);
        Bind(_isShownDamageRp, () => IsShownDamage);
        _isShownDamageRp.Action(D, OnIsShownDamageChanged);

        var damageRotationZStream = _hpRatioRp.Func(D, hpRatio => _angleLinearRelation.GetY(hpRatio));
        Bind(damageRotationZStream, () => DamageRotationAngle);
    }

    private void Start()
    {
        for (int i = 0; i < JumpOutDigits.Length; i++)
            JumpOutDigits[i].Setup(DigitsFadeInSettings, DigitsFadeOutSettings, DigitsJumpOutSettings, DigitsScaleSettings);
        _lastShownJumpOutDigitIndex = JumpOutDigits.Length - 1;
    }


    //=== Private =============================================================

    private void SettingsDurationSetup()
    {
        var fadeInDuration = DigitsFadeInPart * Duration; //время проявления цифры урона
        var fadeOutDuration = Mathf.Min(1 - DigitsFadeInPart, DigitsFadeOutPart) * Duration; //время затухания
        var delayBeforeFadeOut = Mathf.Max(Duration - fadeInDuration - fadeOutDuration, 0); //время от конца проявления до начала затухания
        DigitsFadeInSettings.Duration = fadeInDuration;
        DigitsFadeOutSettings.Duration = fadeOutDuration;
        DigitsFadeOutSettings.Delay = delayBeforeFadeOut;
        DigitsJumpOutSettings.Duration = Duration;
        DigitsScaleSettings.Delay = Duration - fadeOutDuration;
        DigitsScaleSettings.Duration = fadeOutDuration; //уменьшаем цифру вместе с затуханием
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        switch (args.PropertyName)
        {
            case nameof(PlayerMainStatsViewModel.HealthCurrentRatioClamped):
                _hpRatioRp.Value = ViewModel.HealthCurrentRatioClamped;
                break;

            case nameof(PlayerMainStatsViewModel.HealthCurrent):
                DamageDigitsCheck(ViewModel.HealthCurrent);
                break;
        }
    }

    //=== Красный сектор урона

    private void DamageIndicatorCheck(float newHpRatio)
    {
        var negativeDamageRatio = newHpRatio - _oldHpRatioRp.Value;
        if (_isShownDamageRp.Value)
        {
            //урон сейчас отображается
            if (negativeDamageRatio >= 0)
            {
                //результирующий урон отсутствует - убираем показ
                _isShownDamageRp.Value = false;
            }
            else
            {
                //продолжаем/перезапускаем показ урона
                ResetDamageVisualizationIfNeed();
            }
        }
        else
        {
            if (negativeDamageRatio > 0) //здоровье улучшилось, урон сейчас не отображается - нечего делать
            {
                _oldHpRatioRp.Value = newHpRatio; //запоминаем новый уровень здоровья
            }
            else
            {
                _isShownDamageRp.Value = true;
            }
        }
    }

    private void OnIsShownDamageChanged(bool isOn)
    {
        if (isOn)
        {
            HudWarningFrame.CriticaBlinking();
            _damageShowEndTime = Time.time + DamageSectorDisappearingSettings.Delay + DamageSectorDisappearingSettings.Duration;
            _damageSectorWidthSequence.KillIfExistsAndActive();
            _damageSectorWidthSequence = DOTween.Sequence() //отложенное уменьшение индикатора урона до нуля
                .AppendInterval(DamageSectorDisappearingSettings.Delay)
                .Append(DamageRatioDecreasing())
                .AppendCallback(() => _isShownDamageRp.Value = false);
        }
        else
        {
            _damageSectorWidthSequence.KillIfExistsAndActive();
            _damageShowEndTime = 0;
            if (_resetOldHp)
                _oldHpRatioRp.Value = _hpRatioRp.Value;
        }
    }

    private Tween DamageRatioDecreasing()
    {
        return DOTween.To(() => _oldHpRatioRp.Value, f => _oldHpRatioRp.Value = f, _hpRatioRp.Value, DamageSectorDisappearingSettings.Duration);
    }

    private void ResetDamageVisualizationIfNeed()
    {
        if (_damageShowEndTime - Time.time - DamageSectorDisappearingSettings.Duration > 0) //еще ждем, ничего не трогаем
            return;

        _resetOldHp = false;
        _isShownDamageRp.Value = false;
        _isShownDamageRp.Value = true;
        _resetOldHp = true;
    }

    //=== Отлетающие циферки

    private void DamageDigitsCheck(float newHp)
    {
        var negativeDamage = newHp - _registeredDamageDigitsHp; //отрицательное - урон
        if (negativeDamage < 0)
        {
            //урон
            if (-negativeDamage > MinDisplayedDamageValue)
            {
                //надо показывать
                var intNegativeDamage = (int) negativeDamage;
                _registeredDamageDigitsHp = _registeredDamageDigitsHp + intNegativeDamage; //запоминаем hp с учетом отлетающих (целых) цифр урона
                StartShowDigits(intNegativeDamage);
            }
        }
        else
        {
            //прирост
            _registeredDamageDigitsHp = newHp; //запоминаем новый уровень здоровья
        }
    }

    private void StartShowDigits(int negativeDamage)
    {
        var jumpOutDigit = GetJumpOutDigit();
        if (jumpOutDigit.AssertIfNull(nameof(jumpOutDigit)))
            return;

        jumpOutDigit.StartShowDamage(negativeDamage);
    }

    private JumpOutDigit GetJumpOutDigit()
    {
        for (int i = 0, len = JumpOutDigits.Length; i < len; i++)
        {
            var candidate = JumpOutDigits[i];
            if (!candidate.IsShown)
            {
                _lastShownJumpOutDigitIndex = i;
                return candidate;
            }
        }

        return GetNextJumpOutDigit();
    }

    private JumpOutDigit GetNextJumpOutDigit()
    {
        if (++_lastShownJumpOutDigitIndex >= JumpOutDigits.Length)
            _lastShownJumpOutDigitIndex = 0;

        return JumpOutDigits[_lastShownJumpOutDigitIndex];
    }
}