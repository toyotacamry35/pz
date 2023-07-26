using Assets.Src.ResourceSystem;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityWeld.Binding;

namespace Uins
{
    public delegate void InventoryFiltrableTypeEventHandler(InventoryFiltrableTypeDef inventoryFiltrableTypeDef);

    [Binding]
    public class FiltrableTypeFilterViewModel : BindingViewModel
    {
        public event InventoryFiltrableTypeEventHandler FiltrableTypeFilterChanged;

        [SerializeField, UsedImplicitly]
        protected ToggleGroupWithIndex FiltrableTypeToggleGroupWithIndex;

        [SerializeField, UsedImplicitly]
        [FormerlySerializedAs("_filtrableTypeDefRefs")]
        protected InventoryFiltrableTypeDefRef[] FiltrableTypeDefRefs;

        [SerializeField, UsedImplicitly]
        protected LocalizationKeyPairsDefRef NoFilterNameDefRef;

        private InventoryFiltrableTypeDef[] _filtrableTypeDefs;


        //=== Props ===========================================================

        public InventoryFiltrableTypeDef CurrentFiltrableTypeFilter
        {
            get
            {
                if (_filtrableTypeDefs == null && FiltrableTypeDefRefs != null && FiltrableTypeDefRefs.Length > 0)
                    _filtrableTypeDefs = GetFiltrableTypeDefs(FiltrableTypeDefRefs);
                return _filtrableTypeDefs?[FiltrableTypeToggleGroupWithIndex.SelectedIndex];
            }
        }

        private LocalizedString _filtrableTypeText;

        [Binding]
        public LocalizedString FiltrableTypeText
        {
            get => _filtrableTypeText;
            set
            {
                if (!_filtrableTypeText.Equals(value))
                {
                    _filtrableTypeText = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            NoFilterNameDefRef.Target.AssertIfNull(nameof(NoFilterNameDefRef));
            TryInit();
        }


        //=== Protected =======================================================

        protected virtual bool TryInit()
        {
            if (FiltrableTypeDefRefs.AssertIfNull(nameof(FiltrableTypeDefRefs)) ||
                FiltrableTypeToggleGroupWithIndex.AssertIfNull(nameof(FiltrableTypeToggleGroupWithIndex)))
                return false;

            FiltrableTypeToggleGroupWithIndex.OnIndexChanged += OnFiltrableTypeToggleIndexChanged;
            return true;
        }


        //=== Private =========================================================

        private InventoryFiltrableTypeDef[] GetFiltrableTypeDefs(InventoryFiltrableTypeDefRef[] refs)
        {
            var filtrableTypeDefs = new InventoryFiltrableTypeDef[refs.Length];
            for (int i = 0, len = refs.Length; i < len; i++)
                filtrableTypeDefs[i] = refs[i] != null ? refs[i].Target : null;
            return filtrableTypeDefs;
        }

        private void OnFiltrableTypeToggleIndexChanged(int newindex)
        {
            FiltrableTypeText = GetFiltrableTypeText();
            FiltrableTypeFilterChanged?.Invoke(CurrentFiltrableTypeFilter);
        }

        private LocalizedString GetFiltrableTypeText()
        {
            return CurrentFiltrableTypeFilter == null ? NoFilterNameDefRef.Target.Ls1 : CurrentFiltrableTypeFilter.DisplayNameLs;
        }
    }
}