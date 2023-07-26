using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Uins
{
    public class InteractableDispatcher :
        UIBehaviour,
        IPointerClickHandler,
        ISubmitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [FormerlySerializedAs("Interactable")]
        [SerializeField]
        private bool InteractableFlag = true;

        public bool Interactable
        {
            get => InteractableFlag;
            set => InteractableFlag = value;
        }

        [FormerlySerializedAs("OnClick")]
        [SerializeField]
        private Button.ButtonClickedEvent Click = new Button.ButtonClickedEvent();

        public Button.ButtonClickedEvent OnClickEvent
        {
            get => Click;
            set => Click = value;
        }

        [FormerlySerializedAs("OnPointerEnter")]
        [SerializeField]
        private Button.ButtonClickedEvent PointerEnter = new Button.ButtonClickedEvent();

        public Button.ButtonClickedEvent OnPointerEnterEvent
        {
            get => PointerEnter;
            set => PointerEnter = value;
        }

        [FormerlySerializedAs("OnPointerExit")]
        [SerializeField]
        private Button.ButtonClickedEvent PointerExit = new Button.ButtonClickedEvent();

        public Button.ButtonClickedEvent OnPointerExitEvent
        {
            get => PointerExit;
            set => PointerExit = value;
        }

        [FormerlySerializedAs("OnPointerDown")]
        [SerializeField]
        private Button.ButtonClickedEvent PointerDown = new Button.ButtonClickedEvent();

        public Button.ButtonClickedEvent OnPointerDownEvent
        {
            get => PointerDown;
            set => PointerDown = value;
        }

        [FormerlySerializedAs("OnPointerUp")]
        [SerializeField]
        private Button.ButtonClickedEvent PointerUp = new Button.ButtonClickedEvent();

        public Button.ButtonClickedEvent OnPointerUpEvent
        {
            get => PointerUp;
            set => PointerUp = value;
        }

        private bool IsPointerInside { get; set; }
        private bool IsPointerDown { get; set; }
        private bool IsActiveAndInteractable => IsActive() && _groupsAllowInteraction && Interactable;

        private readonly List<CanvasGroup> _canvasGroupCache = new List<CanvasGroup>();
        private bool _groupsAllowInteraction = true;

        protected bool IsHighlighted()
        {
            return IsActiveAndInteractable && IsPointerInside && !IsPointerDown;
        }

        protected bool IsPressed()
        {
            return IsActiveAndInteractable && IsPointerDown;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!IsActiveAndInteractable)
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            IsPointerDown = true;
            OnPointerDownEvent.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!IsActiveAndInteractable)
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            IsPointerDown = false;
            OnPointerUpEvent.Invoke();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsActiveAndInteractable)
                return;

            IsPointerInside = true;
            OnPointerEnterEvent.Invoke();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!IsActiveAndInteractable)
                return;

            IsPointerInside = false;
            OnPointerExitEvent.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActiveAndInteractable)
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;


            Click.Invoke();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (!IsActiveAndInteractable)
                return;

            Click.Invoke();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            OnCanvasGroupChanged();
        }

        protected override void OnCanvasGroupChanged()
        {
            var groupAllowInteraction = true;
            var t = transform;
            while (t != null)
            {
                t.GetComponents(_canvasGroupCache);
                var shouldBreak = false;
                foreach (var canvasGroup in _canvasGroupCache)
                {
                    if (!canvasGroup.interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }

                    if (canvasGroup.ignoreParentGroups)
                        shouldBreak = true;
                }

                if (shouldBreak)
                    break;

                t = t.parent;
            }

            _groupsAllowInteraction = groupAllowInteraction;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            IsPointerDown = false;
            IsPointerInside = false;
        }

        protected override void OnDisable()
        {
            IsPointerInside = false;
            IsPointerDown = false;

            base.OnDisable();
        }
    }
}