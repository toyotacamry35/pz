using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Uins
{
    public class PassiveToggle : Toggle
    {
        [SerializeField]
        public UnityEvent OnClick = new UnityEvent();

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalSubmit();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            InternalSubmit();
        }

        private void InternalSubmit()
        {
            if (!IsActive() || !IsInteractable())
                return;

            OnClick.Invoke();
        }
    }
}