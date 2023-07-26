using System;
using ResourceSystem.Reactions;

namespace ColonyShared.SharedCode.Reactions
{
    public struct ReactionHandlerTuple
    {
        public readonly ReactionDef Reaction;
        public readonly IReactionHandlerDescriptor Handler;

        public ReactionHandlerTuple(ReactionDef action, IReactionHandlerDescriptor handler)
        {
            Reaction = action ?? throw new ArgumentNullException(nameof(action));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
        
        public override string ToString() => $"{Reaction.ReactionToString()} <-> {Handler.HandlerToString()}";
    }
}