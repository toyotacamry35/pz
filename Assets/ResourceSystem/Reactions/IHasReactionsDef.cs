using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;

namespace ResourceSystem.Reactions
{
    public interface IHasReactionsDef
    {
        ResourceRef<ReactionsDef> ReactionHandlers { get; }
    }

    public class ReactionsDef: BaseResource
    {
        public Dictionary<ResourceRef<ReactionDef>, ResourceRef<ReactionHandlerDef>> Reactions { get; set; }
    }
}