using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Stats;
using L10n;
using SharedCode.Aspects.Item.Templates;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    /// <summary>
    /// Тултип-VM для ингредиента крафт/верстак рецепта в ContextView
    /// </summary>
    [Binding]
    public class CraftElemTooltipViewModel : ItemPropsBaseViewModel
    {
        private readonly List<KeyValuePair<StatResource, float>> _mainStats = new List<KeyValuePair<StatResource, float>>();
        private List<KeyValuePair<StatResource, float>> _regularStats = new List<KeyValuePair<StatResource, float>>();


        //=== Props ===========================================================

        private CraftRecipeElementUI _craftRecipeElem;

        public override object TargetDescription
        {
            get => _craftRecipeElem;
            set
            {
                var craftRecipeElementUi = value as CraftRecipeElementUI;
                if (craftRecipeElementUi.AssertIfNull(nameof(craftRecipeElementUi)))
                    return;

                _craftRecipeElem = craftRecipeElementUi;
                var itemResource = _craftRecipeElem?.CraftRecipeModifier.Item.Item.Target as ItemResource;
                BigIcon = itemResource?.BigIcon.Target;
                ItemName = itemResource?.ItemNameLs ?? LsExtensions.Empty;
                Description = itemResource?.DescriptionLs ?? LsExtensions.Empty;
                ItemTier = itemResource?.Tier ?? 0;
                ItemWeight = itemResource?.Weight ?? 0;
                InventoryFiltrableType = itemResource?.InventoryFiltrableType;
                _regularStats = GetModifierRegularStats(_craftRecipeElem.CraftRecipeModifier);
                StatsViewModel.SetItemStats(_mainStats, _regularStats);
            }
        }


        //=== Protected =======================================================

        protected override void OnAwake()
        {
        }


        //=== Private =========================================================

        private List<KeyValuePair<StatResource, float>> GetModifierRegularStats(CraftRecipeModifier craftRecipeModifier)
        {
            var summaryGeneralStats = new Dictionary<StatResource, float>();
            var summarySpecificStats = new Dictionary<StatResource, float>();

            GatherShownStats(ref summaryGeneralStats,
                craftRecipeModifier.StatsModifiers?
                    .Where(v => v.StatType == StatType.General)
                    .Select(v => new StatModifier(v.StatResource, v.InitialValue))
                    .ToArray()
            );
            GatherShownStats(ref summarySpecificStats,
                craftRecipeModifier.StatsModifiers?
                    .Where(v => v.StatType == StatType.Specific)
                    .Select(v => new StatModifier(v.StatResource, v.InitialValue))
                    .ToArray());
            var lst = new List<KeyValuePair<StatResource, float>>();
            foreach (var kvp in summaryGeneralStats)
                lst.Add(kvp);
            foreach (var kvp in summarySpecificStats)
                lst.Add(kvp);

            return lst.Where(kvp => !Mathf.Approximately(kvp.Value, 0)).ToList(); //Отсекаем нулевые именно на этой стадии
        }
    }
}