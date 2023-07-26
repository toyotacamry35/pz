using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Uins.Tooltips
{
    public class ItemTooltip : BaseTooltip
    {
        [SerializeField, UsedImplicitly]
        private float _restOfBackgroundHeight = 166;

        [SerializeField, UsedImplicitly]
        private float _minStatBlockHeight = 90;

        [SerializeField, UsedImplicitly]
        private RectTransform _statsBlockRectTransform;

        [SerializeField, UsedImplicitly]
        protected ItemPropsBaseViewModel ViewModel;


        //=== Props ===========================================================

        protected float DesiredBackgroundHeight =>
            _restOfBackgroundHeight + Mathf.Max(_minStatBlockHeight, _statsBlockRectTransform.sizeDelta.y);


        //=== Protected =======================================================

        protected override void OnAwake()
        {
            _statsBlockRectTransform.AssertIfNull(nameof(_statsBlockRectTransform), gameObject);
            ViewModel.AssertIfNull(nameof(ViewModel), gameObject);
        }

        protected override void OnSetup(MonoBehaviour monoBehaviour)
        {
            var tooltipDescription = monoBehaviour as BaseTooltipDescription;
            if (!tooltipDescription.AssertIfNull(nameof(tooltipDescription)))
                ViewModel.TargetDescription = tooltipDescription.Description;

            HeightLineup();
        }

        protected void HeightLineup()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_statsBlockRectTransform);
            SetBackgroundHeight(DesiredBackgroundHeight);
        }
    }
}