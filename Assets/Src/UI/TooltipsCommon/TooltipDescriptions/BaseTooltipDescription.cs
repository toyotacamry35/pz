using UnityEngine;
using UnityEngine.EventSystems;

namespace Uins.Tooltips
{
    public abstract class BaseTooltipDescription : MonoBehaviour, ITooltipDescription, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _isShown;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (HasDescription)
            {
                BaseTooltip.Show(this);
                _isShown = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isShown)
            {
                _isShown = false;
                BaseTooltip.Hide();
            }
        }

        public abstract object Description { get; }

        public abstract bool HasDescription { get; }
    }
}