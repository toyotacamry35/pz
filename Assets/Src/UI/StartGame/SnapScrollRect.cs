using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Uins
{
    [SelectionBase]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class SnapScrollRect : ScrollRect
    {
        [Serializable]
        public class ScrollPosChangedEvent : UnityEvent<int>
        {
        }

        [SerializeField]
        private int ScrollStep;

        [SerializeField]
        private ScrollPosChangedEvent OnScrollPosChanged = new ScrollPosChangedEvent();

        public ScrollPosChangedEvent OnScrollPosChangedEvent
        {
            get => OnScrollPosChanged;
            set => OnScrollPosChanged = value;
        }

        private int _scrollPos;

        public int ScrollPos
        {
            get => _scrollPos;
            set
            {
                _scrollPos = value;
                UpdatePosition();
                OnScrollPosChanged.Invoke(_scrollPos);
            }
        }


        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            ScrollPos = (int) Math.Round(-1 * position.x / ScrollStep);
        }

        private void UpdatePosition()
        {
            var position = Vector2.zero;
            position.x = -1 * _scrollPos * ScrollStep;
            base.SetContentAnchoredPosition(position);
        }
    }
}