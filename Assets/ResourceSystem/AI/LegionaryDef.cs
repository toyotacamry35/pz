using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree;
using System.Collections.Generic;
using Assets.Src.Aspects.Impl.Factions.Template;

namespace Assets.Src.RubiconAI
{
    public class LegionaryDef : BaseResource
    {
        public ResourceRef<StrategyDef> MainStrategy { get; set; }
        public List<ResourceRef<KnowledgeSourceDef>> KnowledgeSources { get; set; } = new List<ResourceRef<KnowledgeSourceDef>>();
        public ResourceRef<LegionDef> DefaultLegion { get; set; }
        public List<EventHandler> EventHandlers { get; set; } = new List<EventHandler>();
    }
}
