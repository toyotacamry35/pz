using Assets.Src.Aspects.Impl.Factions.Template;
using ReactivePropsNs;

namespace Uins
{
    public struct CharacterStreamsData
    {
        public IStream<MutationStageDef> MutationStageStream;
        public IStream<int> AccountLevelStream;
        public IPawnSource PawnSource;
        public ICharacterPoints CharacterPoints;
    }
}