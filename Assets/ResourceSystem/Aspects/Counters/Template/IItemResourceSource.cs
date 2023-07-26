using SharedCode.Aspects.Item.Templates;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public interface IItemResourceSource
    {
        BaseItemResource Item { get; }
        int Count { get; }
    }
}