using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Shared;
using Assets.Src.SpatialSystem;

namespace Assets.Src.Shared
{
    public enum LayersEnum
    {
        Default,
        WorldCell,
        Terrain,
        Trigger,
        Destructables,
        DestructChunks,
        DetachedDestructChunks,
        Interactive,
        Active,
        Vision
    }
}

namespace Assets.Src.RubiconAI.KnowledgeSystem
{

    public class SpatialKnowledgeSourceDef : KnowledgeSourceDef
    {
        public LayersEnum Layer { get; set; }
        public string FeedName { get; set; }
        public float FeedRange { get; set; }
    }
}
