using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Reactions
{
    public class ReactionDef : BaseResource
    {
        public ResourceRef<ArgDef>[] Args { get; set; }
        
        public string ReactionToString() => ____GetDebugAddress();
    }
}