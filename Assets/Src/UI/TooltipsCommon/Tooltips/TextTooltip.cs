using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins.Tooltips
{
    [Binding]
    public class TextTooltip : BaseTooltip
    {
        [SerializeField, UsedImplicitly]
        private RectTransform _descriptionRectTransform;


        //=== Props ===============================================================

        private string _description;

        [Binding]
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Protected ===========================================================

        protected override void OnAwake()
        {
            _descriptionRectTransform.AssertIfNull(nameof(_descriptionRectTransform));
        }

        protected override void OnSetup(MonoBehaviour monoBehaviour)
        {
            var tooltipDescription = monoBehaviour as ITooltipDescription;
            Description = tooltipDescription == null
                ? $"Unhandled type: {monoBehaviour.GetType().NiceName()}"
                : tooltipDescription.Description as string;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_descriptionRectTransform);
        }
    }
}