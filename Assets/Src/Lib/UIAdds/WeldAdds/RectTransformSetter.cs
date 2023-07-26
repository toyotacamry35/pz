using Assets.Src.Lib.DOTweenAdds;
using UnityEngine;

namespace WeldAdds
{
    public enum LimitedUsageType
    {
        Amount,
        Flag
    }

    public class RectTransformSetter : MonoBehaviour
    {
        public RectTransform RectTransform;

        public PositionTweenComponent.RectTransformParam TargetType;

        public float TargetMinValue = 0;

        public float TargetMaxValue = 100;

        public float AmountMinValue = 0;

        public float AmountMaxValue = 100;

        public LimitedUsageType UsageType;

        public bool AmountClamping = true;

        private LinearRelation _linearRelation;

        private bool _isAfterAwake;


        //=== Props ===========================================================

        private float _amount;

        public float Amount
        {
            get => _amount;
            set
            {
                var clampedValue = AmountClamping ? Mathf.Clamp(value, AmountMinValue, AmountMaxValue) : value;

                if (!Mathf.Approximately(_amount, clampedValue))
                {
                    _amount = clampedValue;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private bool _flag;

        public bool Flag
        {
            get => _flag;
            set
            {
                if (!enabled)
                    return;

                if (_flag != value)
                {
                    _flag = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private float TargetValue
        {
            get
            {
                switch (TargetType)
                {
                    case PositionTweenComponent.RectTransformParam.AnchoredPosX:
                        return RectTransform.anchoredPosition.x;

                    case PositionTweenComponent.RectTransformParam.AnchoredPosY:
                        return RectTransform.anchoredPosition.y;

                    case PositionTweenComponent.RectTransformParam.SizeDeltaX:
                        return RectTransform.sizeDelta.x;

                    default:
                        return RectTransform.sizeDelta.y;
                }
            }

            set
            {
                switch (TargetType)
                {
                    case PositionTweenComponent.RectTransformParam.AnchoredPosX:
                        RectTransform.anchoredPosition = new Vector2(value, RectTransform.anchoredPosition.y);
                        break;

                    case PositionTweenComponent.RectTransformParam.AnchoredPosY:
                        RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, value);
                        break;

                    case PositionTweenComponent.RectTransformParam.SizeDeltaX:
                        RectTransform.sizeDelta = new Vector2(value, RectTransform.sizeDelta.y);
                        break;

                    default:
                        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, value);
                        break;
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _isAfterAwake = true;
            LinearRelationSetup();
            if (UsageType == LimitedUsageType.Amount && (Amount > AmountMaxValue || Amount < AmountMinValue))
                Amount = Amount;
            else
                SyncIfWoken();
        }

        public void OnEnable()
        {
            LinearRelationSetup();
        }


        //=== Public ==============================================================

        public override string ToString()
        {
            return $"<{GetType()}> '{name}' UT={UsageType} AfterAwake{_isAfterAwake.AsSign()} " +
                   $"{(UsageType == LimitedUsageType.Amount ? $"{nameof(Amount)}={Amount}" : $"{nameof(Flag)}{Flag.AsSign()}")} " +
                   $"{nameof(TargetValue)}={TargetValue}\n" +
                   $"{nameof(TargetType)}={TargetType}, AmountVals={AmountMinValue}...{AmountMaxValue}, TargetVals={TargetMinValue}...{TargetMaxValue}";
        }

        //=== Private =============================================================

        private void LinearRelationSetup()
        {
            _linearRelation = new LinearRelation(AmountMinValue, TargetMinValue, AmountMaxValue, TargetMaxValue);
        }

        private void SyncIfWoken()
        {
            //UI.CallerLog($"SYNC begin {this}");
            var amount = UsageType == LimitedUsageType.Amount
                ? Amount
                : (Flag ? AmountMaxValue : AmountMinValue);

            TargetValue = _linearRelation.GetY(amount);
            //UI.CallerLog($"SYNC end {this}");
        }
    }
}