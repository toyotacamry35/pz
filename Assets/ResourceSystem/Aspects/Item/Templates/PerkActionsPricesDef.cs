using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Science;

namespace SharedCode.Aspects.Item.Templates
{
    public class PerkActionsPricesDef : SaveableBaseResource
    {
        /// <summary>
        /// Дефолтные цены сохранения перков - от типа перка
        /// </summary>
        public PerkTypeActionPriceDef[] PerkSavingDefaultCosts;
        /// <summary>
        /// Цены сохранения особых перков
        /// </summary>
        public PerkActionPriceDef[] PerkSavingCustomCosts;

        /// <summary>
        /// Цены апгрейда permanent-слотов перков
        /// </summary>
        public PerkTypeActionPriceDef[] ToPerkSlotUpgradingCosts;

        /// <summary>
        /// Дефолтные прибыли разбора перков - от типа перка
        /// </summary>
        public PerkTypeActionPriceDef[] PerkDisassemblyDefaultBenefits;

        /// <summary>
        /// Прибыли разбора особых перков
        /// </summary>
        public PerkActionPriceDef[] PerkDisassemblyCustomBenefits;
    }
}