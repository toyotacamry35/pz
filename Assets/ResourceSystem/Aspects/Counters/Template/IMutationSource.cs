using Assets.Src.Aspects.Impl.Factions.Template;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public interface IMutationSource
    {
        MutationStageDef Stage { get; }
    }
}