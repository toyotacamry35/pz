using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Uins.Slots
{
    public class DraggingHandler : MonoBehaviour
    {
        private List<DraggableItem> _draggableItems = new List<DraggableItem>();
        private DraggableItem _currentDraggingItem;

        public event Action<DraggableItem> HighlightBegin;
        public event Action HighlightEnd;


        //=== Public ==============================================================

        public void Register(DraggableItem draggableItem)
        {
            lock (_draggableItems)
            {
                _draggableItems.Add(draggableItem);
            }
        }

        public void Unregister(DraggableItem draggableItem)
        {
            lock (_draggableItems)
            {
                if (draggableItem == null || !_draggableItems.Contains(draggableItem))
                {
                    UI.Logger.IfError()?.Message($"{nameof(draggableItem)} is null or not found: {draggableItem?.name}").Write();
                    return;
                }
            }

            if (draggableItem == _currentDraggingItem)
                ForceEndOfCurrentDraggingItem(nameof(Unregister));

            lock (_draggableItems)
            {
                _draggableItems.Remove(draggableItem);
            }
        }

        public void OnItemDragBegin(DraggableItem draggingItem)
        {
            if (draggingItem.AssertIfNull(nameof(draggingItem)))
                return;

            if (_currentDraggingItem != null)
                ForceEndOfCurrentDraggingItem(nameof(OnItemDragBegin));

            _currentDraggingItem = draggingItem;
            lock (_draggableItems)
            {
                foreach (var draggableItem in _draggableItems)
                {
                    if (draggableItem != draggingItem)
                        draggableItem.HighlightBegin(_currentDraggingItem);
                }
            }
            HighlightBegin?.Invoke(_currentDraggingItem);
        }

        public void OnItemDragEnd(DraggableItem draggingItem = null)
        {
            if (draggingItem != null && _currentDraggingItem != draggingItem)
                return;

            lock (_draggableItems)
            {
                foreach (var draggableItem in _draggableItems)
                {
                    if (draggableItem != _currentDraggingItem)
                        draggableItem.HighlightEnd();
                }
            }
            HighlightEnd?.Invoke();
            _currentDraggingItem = null;
        }


        //=== Private =============================================================

        private void ForceEndOfCurrentDraggingItem(string callerName)
        {
            lock (_draggableItems)
            {
                foreach (var draggableItem in _draggableItems)
                    draggableItem.ResetFromDraggingState();
            }

            if (_currentDraggingItem == null)
                return;

            OnItemDragEnd(_currentDraggingItem);
        }
    }
}