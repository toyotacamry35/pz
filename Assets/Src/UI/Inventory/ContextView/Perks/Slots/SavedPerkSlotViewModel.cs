using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class SavedPerkSlotViewModel : PerkCollectionApiSlotViewModel<PerksSavedFullApi, PerkSlotsSavedFullApi>
    {
        //=== Props ===========================================================

        private bool _isHighlighted;

        [Binding]
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                if (_isHighlighted != value)
                {
                    _isHighlighted = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public override void SubscribeOnSomePerkDrag(DraggingHandler draggingHandler, bool isSubscribeNorUnsubscribe)
        {
            if (isSubscribeNorUnsubscribe)
            {
                if (draggingHandler.AssertIfNull(nameof(draggingHandler)))
                    return;

                draggingHandler.HighlightBegin += OnHighlightBegin;
                draggingHandler.HighlightEnd += OnHighlightEnd;
            }
            else
            {
                if (draggingHandler.AssertIfNull(nameof(draggingHandler)))
                    return;

                draggingHandler.HighlightBegin -= OnHighlightBegin;
                draggingHandler.HighlightEnd -= OnHighlightEnd;
            }
        }

        
        //=== Protected =======================================================

        protected override bool GetIsLocked()
        {
            return ItemTypeIndex < 0;
        }


        //=== Private =========================================================

        private void OnHighlightBegin(DraggableItem draggableItem)
        {
            if (draggableItem.AssertIfNull(nameof(draggableItem)) ||
                draggableItem.SlotViewModel.AssertIfNull(nameof(draggableItem.SlotViewModel)))
                return;

            IsHighlighted = CanMoveToThis(draggableItem.SlotViewModel);
        }

        private void OnHighlightEnd()
        {
            IsHighlighted = false;
        }
    }
}