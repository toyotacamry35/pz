using Assets.Src.Aspects.Impl.Factions.Template;

namespace Assets.Src.Aspects
{
    public interface IMutationStageProvider
    {
        MutationStageDef MutationStage { get; }
    }
}