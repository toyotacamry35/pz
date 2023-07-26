using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class ItemsCollection<ItemType, ItemDataType>
        : BindingViewModel where ItemType : SomeItemViewModel<ItemDataType> where ItemDataType : class
    {
        [SerializeField, UsedImplicitly]
        protected Transform ItemsRoot;

        [SerializeField, UsedImplicitly]
        protected ItemType ItemPrefab;


        //=== Props ===========================================================

        public List<ItemType> Items { get; protected set; } = new List<ItemType>();

        protected virtual bool CanReuseItems => false;

        protected virtual bool DoItemsSorting => false;

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


        //=== Unity ==============================================================

        private void Awake()
        {
            ItemsRoot.AssertIfNull(nameof(ItemsRoot), gameObject);
            ItemPrefab.AssertIfNull(nameof(ItemPrefab), gameObject);
        }


        //=== Public ==============================================================

        public void FillCollection(ItemDataType[] data)
        {
            if (!CanReuseItems)
                ClearCollection();

            if (data != null && data.Length > 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var item = GetItem(i);
                    if (item != null)
                    {
                        item.Fill(data[i]);
                        item.AfterFill();
                    }
                }
            }

            if (CanReuseItems && (data?.Length ?? 0) < Items.Count)
            {
                for (int i = (data?.Length ?? 0); i < Items.Count; i++)
                {
                    var item = Items[i];
                    if (item != null)
                    {
                        item.Fill(null); //Соглашение: элемент с нулевыми данными должен визуально скрываться
                        item.AfterFill();
                    }
                }
            }

            if (DoItemsSorting)
                SortItems();

            AfterFill();
        }

        public void ClearCollection()
        {
            while (Items.Count > 0)
            {
                var lastIndex = Items.Count - 1;
                var itemToRemove = Items[lastIndex];
                Items.RemoveAt(lastIndex);
                Destroy(itemToRemove.gameObject);
            }
        }


        //=== Protected =======================================================

        protected virtual void AfterFill()
        {
            IsEmpty = GetIsEmpty();
        }

        protected bool GetIsEmpty()
        {
            return Items.Count == 0 || Items.All(i => i.IsEmpty);
        }


        //=== Private =========================================================

        private ItemType GetItem(int index)
        {
            if (index < Items.Count)
                return Items[index];

            var newItem = Instantiate(ItemPrefab, ItemsRoot);
            if (newItem.AssertIfNull(nameof(newItem)))
                return null;

            newItem.name = $"{ItemPrefab.name}{index}";
            newItem.Index = index;
            Items.Add(newItem);
            return newItem;
        }

        private void SortItems()
        {
            ItemsSorting.SortSiblings(Items, item => item.SortingIndex);
            foreach (var item in Items)
                item.AfterCollectionSorting();
        }
    }
}