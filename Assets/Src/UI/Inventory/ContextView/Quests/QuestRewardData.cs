using Assets.ColonyShared.SharedCode.Aspects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;

namespace Uins
{
    public enum QuestRewardType
    {
        Item,
        Science,
        Recipe,
        TechPoint
    }

    public class QuestRewardData
    {
        public QuestRewardType RewardType;
        public BaseItemResource Item;
        public ScienceDef Science;
        public BaseRecipeDef Recipe;
        public CurrencyResource TechPoint;
        public int Count;
    }
}