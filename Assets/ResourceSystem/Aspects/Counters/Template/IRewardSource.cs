using Assets.ColonyShared.SharedCode.Aspects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public interface IRewardSource
    {
    }

    public interface IManyRewardsSource
    {
        IRewardSource[] Rewards { get; }
    }

    public interface IItemRewardSource : IRewardSource
    {
        BaseItemResource Item { get; }
        int Count { get; }
    }

    public interface IScienceRewardSource : IRewardSource
    {
        ScienceDef Science { get; }
        int Count { get; }
    }

    public interface IRecipeRewardSource : IRewardSource
    {
        BaseRecipeDef Recipe { get; }
    }

    public interface ITechPointRewardSource : IRewardSource
    {
        CurrencyResource TechPoint { get; }
        int Count { get; }
    }
}