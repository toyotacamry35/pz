using Core.Environment.Logging.Extension;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PerksCollectionSortViewModel : BindingViewModel
    {
        public ToggleGroupWithIndex ToggleGroupWithIndex;


        //=== Props ===========================================================

        private int _items1Count;

        [Binding]
        public int Items1Count
        {
            get => _items1Count;
            private set
            {
                if (_items1Count != value)
                {
                    _items1Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _items2Count;

        [Binding]
        public int Items2Count
        {
            get => _items2Count;
            private set
            {
                if (_items2Count != value)
                {
                    _items2Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _items3Count;

        [Binding]
        public int Items3Count
        {
            get => _items3Count;
            private set
            {
                if (_items3Count != value)
                {
                    _items3Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _totalSlots1Count;

        [Binding]
        public int TotalSlots1Count
        {
            get => _totalSlots1Count;
            private set
            {
                if (_totalSlots1Count != value)
                {
                    _totalSlots1Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _totalSlots2Count;

        [Binding]
        public int TotalSlots2Count
        {
            get => _totalSlots2Count;
            private set
            {
                if (_totalSlots2Count != value)
                {
                    _totalSlots2Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _totalSlots3Count;

        [Binding]
        public int TotalSlots3Count
        {
            get => _totalSlots3Count;
            private set
            {
                if (_totalSlots3Count != value)
                {
                    _totalSlots3Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _emptySlots1Count;

        [Binding]
        public int EmptySlots1Count
        {
            get => _emptySlots1Count;
            private set
            {
                if (_emptySlots1Count != value)
                {
                    _emptySlots1Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _emptySlots2Count;

        [Binding]
        public int EmptySlots2Count
        {
            get => _emptySlots2Count;
            private set
            {
                if (_emptySlots2Count != value)
                {
                    _emptySlots2Count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _emptySlots3Count;

        [Binding]
        public int EmptySlots3Count
        {
            get => _emptySlots3Count;
            private set
            {
                if (_emptySlots3Count != value)
                {
                    _emptySlots3Count = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            ToggleGroupWithIndex.AssertIfNull(nameof(ToggleGroupWithIndex));
        }

        public void Init(IItemsCountsSource itemsCountsSource)
        {
            if (itemsCountsSource.AssertIfNull(nameof(itemsCountsSource)))
                return;
            itemsCountsSource.ItemsCountsChanged += OnItemsCountsChanged;
        }

        private const int ItemsTypesCount = 3;

        private void OnItemsCountsChanged(int[] perksCounts, int[] totalSlotsCounts, int[] emptySlotsCounts)
        {
            if (perksCounts == null || perksCounts.Length != ItemsTypesCount ||
                totalSlotsCounts == null || totalSlotsCounts.Length != ItemsTypesCount ||
                emptySlotsCounts == null || emptySlotsCounts.Length != ItemsTypesCount)
            {
                UI.Logger.IfError()?.Message($"Empty or wrong counts arrays {nameof(perksCounts)}, {nameof(emptySlotsCounts)}, {nameof(emptySlotsCounts)}").Write();
                return;
            }

            Items1Count = perksCounts[0];
            Items2Count = perksCounts[1];
            Items3Count = perksCounts[2];
            TotalSlots1Count = totalSlotsCounts[0];
            TotalSlots2Count = totalSlotsCounts[1];
            TotalSlots3Count = totalSlotsCounts[2];
            EmptySlots1Count = emptySlotsCounts[0];
            EmptySlots2Count = emptySlotsCounts[1];
            EmptySlots3Count = emptySlotsCounts[2];
        }
    }
}