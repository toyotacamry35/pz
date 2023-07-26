using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.Src.Aspects.Impl.Stats;
using L10n;
using SharedCode.Aspects.Item.Templates;
using Uins.Slots;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class SlotPropsBaseViewModel : ItemPropsBaseViewModel
    {
        //=== Props ===========================================================

        private SlotViewModel _targetSlotViewModel;

        private string[] _propertiesWatchList = new[]
        {
            nameof(Durability),
            nameof(DurabilityMaxCurrent),
            nameof(DurabilityMaxAbsolute),
            nameof(DurabilityRatio),
            nameof(DurabilityAbsRatio),
            nameof(DurabilityMaxCurrentRatio),
            nameof(DurabilityMaxCurrentInvertRatio),
            nameof(HasDurability),
            nameof(IsBroken),
            nameof(IsAlmostBroken),
            nameof(ContainerItemName),
            nameof(HasAmmo),
            nameof(HasAmmoContainer),
        };

        public SlotViewModel TargetSlotViewModel
        {
            get => _targetSlotViewModel;
            set
            {
                if (_targetSlotViewModel != value)
                {
                    if (_targetSlotViewModel != null)
                        _targetSlotViewModel.PropertyChanged -= OnTargetSlotViewModelPropertyChanged;
                    _targetSlotViewModel = value;
                    var itemResource = _targetSlotViewModel?.ItemResource as ItemResource;
                    BigIcon = itemResource?.BigIcon?.Target;
                    ItemName = itemResource?.ItemNameLs ?? LsExtensions.Empty;
                    Description = itemResource?.DescriptionLs ?? LsExtensions.Empty;
                    ItemTier = itemResource?.Tier ?? 0;
                    Stack = _targetSlotViewModel?.Stack ?? 0;
                    ItemWeight = itemResource?.Weight ?? 0;
                    ItemsWeight = ItemWeight * Stack;
                    InventoryFiltrableType = itemResource?.InventoryFiltrableType.Target;
                    var regularStats = GetItemRegularStats(_targetSlotViewModel?.SelfSlotItem);
                    var mainStats = GetMainStats(_targetSlotViewModel?.SelfSlotItem?.ItemResource, regularStats);
                    StatsViewModel.SetItemStats(mainStats, regularStats);
                    DurabilitySource = _targetSlotViewModel;
                    NotifyPropertyChangedFromWatchList();
//                    UI.CallerLog($"Name={ItemName} x{Stack}  IsBroken{IsBroken.AsSign()}, " +
//                                 $"HD{HasDurability.AsSign()}, Dur={Durability}/{DurabilityMaxCurrent}/{DurabilityMaxAbsolute}  " +
//                                 $"DR={DurabilityRatio:f2}, DMCIR={DurabilityMaxCurrentInvertRatio:f2}{(IsAlmostBroken ? "ALMOST BROKEN" : "")}");
                    if (_targetSlotViewModel != null)
                        _targetSlotViewModel.PropertyChanged += OnTargetSlotViewModelPropertyChanged;
                }
            }
        }


        //=== Public ==========================================================

        /// <summary>
        /// Извлечение кастомных main-статов из regularStats (пока один типовой способ)
        /// </summary>
        public static List<KeyValuePair<StatResource, float>> GetMainStats(BaseItemResource itemResource,
            List<KeyValuePair<StatResource, float>> regularStats)
        {
            var mainStats = new List<KeyValuePair<StatResource, float>>();

            if (itemResource != null)
            {
                var itemType = itemResource.ItemType.Target;
                if (itemType != null && SlotContextViewModel.WeaponGroupType.IsAParentOf(itemType)) //Если это оружие
                {
                    var dpsStatValue = regularStats.Where(kvp => kvp.Key == SlotContextViewModel.DamageStat).Select(kvp => kvp.Value)
                        .FirstOrDefault();
                    if (!Mathf.Approximately(dpsStatValue, 0))
                        mainStats.Add(new KeyValuePair<StatResource, float>(SlotContextViewModel.DpsStat, dpsStatValue));
                }
            }

            return mainStats;
        }


        //=== Private =========================================================

        private void NotifyPropertyChangedFromWatchList()
        {
            foreach (var propName in _propertiesWatchList)
                NotifyPropertyChanged(propName);
        }

        private void OnTargetSlotViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_propertiesWatchList.Contains(e.PropertyName))
                return;

            NotifyPropertyChanged(e.PropertyName);
        }
    }
}