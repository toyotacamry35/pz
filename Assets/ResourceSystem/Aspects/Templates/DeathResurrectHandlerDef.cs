using Assets.Src.GameObjectAssembler.Res;

namespace Assets.ColonyShared.SharedCode.Aspects.Templates
{
    public class DeathResurrectHandlerDef : ComponentDef
    {
        public string PlayAnimation { get; set; }
        public float  DestroyDelay  { get; set; }
    }
}
