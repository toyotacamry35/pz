using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;

namespace Uins
{
    public interface IPerkActionsPricesCalculator
    {
        PriceDef GetPerkSavingCosts(BaseItemResource perk);
        PriceDef GetPerkDisassemblyBenefits(BaseItemResource perk);
        PriceDef GetToPerkSlotUpgradingCosts(ItemTypeResource toPerkSlotType);
        ReactiveProperty<int> PerkActionsMultiplierRp { get; }
    }
}