using Assets.Src.GameObjectAssembler.Res;

namespace Assets.ColonyShared.SharedCode.Aspects.Templates
{
    public class CorpseViewProviderDef : ComponentDef
    {
        public string PlayAnimation               { get; set; }
        public string PlayAnimationParameter      { get; set; }
        public int    PlayAnimationParameterValue { get; set; }
    }
}
