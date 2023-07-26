using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.Inventory;
using L10n;
using SharedCode.Aspects.Item.Templates;
using Uins.Slots;
using UnityEngine;
using UnityEngine.Serialization;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class ItemPropsBaseViewModel : BindingViewModel
    {
        [FormerlySerializedAs("_statsViewModel")]
        public StatsViewModel StatsViewModel;

        protected SlotViewModel DurabilitySource;


        //=== Props ===========================================================

        public virtual object TargetDescription { get; set; }

        private int _itemTier;

        [Binding]
        public int ItemTier
        {
            get => _itemTier;
            set
            {
                if (_itemTier != value)
                {
                    _itemTier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _itemName;

        [Binding]
        public LocalizedString ItemName
        {
            get => _itemName;
            set
            {
                if (!_itemName.Equals(value))
                {
                    _itemName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _description;

        [Binding]
        public LocalizedString Description
        {
            get => _description;
            set
            {
                if (!_description.Equals(value))
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _bigIcon;

        [Binding]
        public Sprite BigIcon
        {
            get => _bigIcon;
            set
            {
                if (_bigIcon != value)
                {
                    var oldIsBigIconEmpty = IsBigIconEmpty;
                    _bigIcon = value;
                    NotifyPropertyChanged();
                    if (oldIsBigIconEmpty != IsBigIconEmpty)
                        NotifyPropertyChanged(nameof(IsBigIconEmpty));
                }
            }
        }

        [Binding]
        public bool IsBigIconEmpty => BigIcon == null;

        private float _itemWeight;

        [Binding]
        public float ItemWeight
        {
            get => _itemWeight;
            set
            {
                if (!Mathf.Approximately(_itemWeight, value))
                {
                    _itemWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _itemsWeight;

        [Binding]
        public float ItemsWeight
        {
            get => _itemsWeight;
            set
            {
                if (!Mathf.Approximately(_itemsWeight, value))
                {
                    _itemsWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _stack;

        [Binding]
        public int Stack
        {
            get => _stack;
            set
            {
                if (_stack != value)
                {
                    var oldHasStack = HasStack;
                    _stack = value;
                    NotifyPropertyChanged();
                    if (oldHasStack != HasStack)
                        NotifyPropertyChanged(nameof(HasStack));
                }
            }
        }

        [Binding]
        public bool HasStack => Stack > 1;

        private InventoryFiltrableTypeDef _inventoryFiltrableType;

        public InventoryFiltrableTypeDef InventoryFiltrableType
        {
            get => _inventoryFiltrableType;
            set
            {
                if (_inventoryFiltrableType != value)
                {
                    var oldHasFiltrableTypeIcon = HasFiltrableTypeIcon;
                    _inventoryFiltrableType = value;
                    NotifyPropertyChanged(nameof(FiltrableTypeIcon));
                    if (oldHasFiltrableTypeIcon != HasFiltrableTypeIcon)
                        NotifyPropertyChanged(nameof(HasFiltrableTypeIcon));
                }
            }
        }

        [Binding]
        public Sprite FiltrableTypeIcon => _inventoryFiltrableType?.Icon?.Target;

        [Binding]
        public bool HasFiltrableTypeIcon => FiltrableTypeIcon != null;

        [Binding]
        public float Durability => DurabilitySource?.Durability ?? 0;

        [Binding]
        public float DurabilityRatio => DurabilitySource?.DurabilityRatio ?? 0;

        [Binding]
        public float DurabilityAbsRatio => DurabilitySource?.DurabilityAbsRatio ?? 0;

        [Binding]
        public bool IsBroken => DurabilitySource?.IsBroken ?? false;

        [Binding]
        public bool IsAlmostBroken => DurabilitySource?.IsAlmostBroken ?? false;

        [Binding]
        public float DurabilityMaxCurrent => DurabilitySource?.DurabilityMaxCurrent ?? 1;

        [Binding]
        public float DurabilityMaxCurrentRatio => DurabilitySource?.DurabilityMaxCurrentRatio ?? 0;

        [Binding]
        public float DurabilityMaxCurrentInvertRatio => DurabilitySource?.DurabilityMaxCurrentInvertRatio ?? 0;

        [Binding]
        public float DurabilityMaxAbsolute => DurabilitySource?.DurabilityMaxAbsolute ?? 1;

        [Binding]
        public bool HasDurability => DurabilitySource?.HasDurability ?? false;

        [Binding]
        public bool HasAmmoContainer => DurabilitySource?.HasAmmoContainer ?? false;

        [Binding]
        public bool HasAmmo => DurabilitySource?.HasAmmo ?? false;

        [Binding]
        public string ContainerItemName => DurabilitySource?.ContainerItemName ?? "";


        //=== Unity ===========================================================

        private void Awake()
        {
            if (!StatsViewModel.AssertIfNull(nameof(StatsViewModel)))
                OnAwake();
        }


        //=== Public ==========================================================

        /// <summary>
        /// Собирает в summaryStats статы и их значения из statModifiers
        /// </summary>
        /// <param name="summaryStats"></param>
        /// <param name="statModifiers"></param>
        public static void GatherShownStats(ref Dictionary<StatResource, float> summaryStats, StatModifier[] statModifiers)
        {
            if (statModifiers == null || statModifiers.Length == 0)
                return;

            for (int i = 0; i < statModifiers.Length; i++)
            {
                var statModifier = statModifiers[i];
                if (statModifier.Stat == null || statModifier.Stat.Target == null)
                {
                    //UI.Logger.IfError()?.Message($"{nameof(statModifiers)}[{i}].{nameof(statModifier.Stat)} is null").Write();
                    continue;
                }

                if (statModifier.Stat.Target.DontShow)
                    continue;

                if (summaryStats.ContainsKey(statModifier.Stat.Target))
                    summaryStats[statModifier.Stat.Target] += statModifier.Value;
                else
                    summaryStats[statModifier.Stat.Target] = statModifier.Value;
            }
        }


        //=== Protected =======================================================

        protected abstract void OnAwake();

        /// <summary>
        /// Собрать статы с предмета: с базы (ItemResource) + со статов самого предмета.
        /// Пока все складываем в общий список
        /// </summary>
        protected static List<KeyValuePair<StatResource, float>> GetItemRegularStats(SlotItem slotItem)
        {
            var summaryGeneralStats = new Dictionary<StatResource, float>();
            var summarySpecificStats = new Dictionary<StatResource, float>();

            GatherShownStats(ref summaryGeneralStats, (slotItem?.ItemResource as IHasStatsResource)?.GeneralStats.Target?.Stats);
            GatherShownStats(ref summaryGeneralStats, slotItem?.GeneralStats);
            GatherShownStats(ref summarySpecificStats, (slotItem?.ItemResource as IHasStatsResource)?.SpecificStats.Target?.Stats);
            GatherShownStats(ref summarySpecificStats, slotItem?.SpecificStats);
            var lst = new List<KeyValuePair<StatResource, float>>();
            foreach (var kvp in summaryGeneralStats)
                lst.Add(kvp);
            foreach (var kvp in summarySpecificStats)
                lst.Add(kvp);

            return lst.Where(kvp => !Mathf.Approximately(kvp.Value, 0)).ToList();
        }

        /// <summary>
        /// Собрать статы с базы предмета (ItemResource).
        /// Пока все складываем в общий список
        /// </summary>
        protected static List<KeyValuePair<StatResource, float>> GetItemRegularStats(BaseItemResource itemResource)
        {
            var summaryGeneralStats = new Dictionary<StatResource, float>();
            var summarySpecificStats = new Dictionary<StatResource, float>();

            GatherShownStats(ref summaryGeneralStats, (itemResource as IHasStatsResource)?.GeneralStats.Target?.Stats);
            GatherShownStats(ref summarySpecificStats, (itemResource as IHasStatsResource)?.SpecificStats.Target?.Stats);
            var lst = new List<KeyValuePair<StatResource, float>>();
            foreach (var kvp in summaryGeneralStats)
                lst.Add(kvp);
            foreach (var kvp in summarySpecificStats)
                lst.Add(kvp);

            return lst.Where(kvp => !Mathf.Approximately(kvp.Value, 0)).ToList();
        }
    }
}