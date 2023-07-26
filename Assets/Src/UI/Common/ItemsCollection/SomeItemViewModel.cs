using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class SomeItemViewModel<T> : BindingViewModel where T : class
    {
        //=== Props ==============================================================

        public int Index { get; set; }

        public virtual int SortingIndex => Index;

        private bool _isFirst;

        [Binding]
        public bool IsFirst
        {
            get => _isFirst;
            set
            {
                if (_isFirst != value)
                {
                    _isFirst = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isEmpty;

        [Binding]
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                if (_isEmpty != value)
                {
                    _isEmpty = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public abstract void Fill(T data);

        public void AfterFill()
        {
            IsEmpty = GetIsEmpty();
        }

        public virtual void AfterCollectionSorting()
        {
            IsFirst = transform.GetSiblingIndex() == 0;
        }


        //=== Protected =======================================================

        protected virtual bool GetIsEmpty()
        {
            return false;
        }
    }
}