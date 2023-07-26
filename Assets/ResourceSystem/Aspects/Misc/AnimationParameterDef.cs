using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Aspects.Misc
{
    public class AnimationParameterDef : BaseResource
    {
        public string Name { get; set; }
    }
    
    public class AnimationParameterTupleDef : AnimationParameterDef
    {
        public string Parameter2Name { get; set; }
    }

    public class AnimationLayerDef : BaseResource
    {
        public string Name { get; set; }
    }

    
    public class AnimationStateDef : BaseResource
    {
    }    
}
