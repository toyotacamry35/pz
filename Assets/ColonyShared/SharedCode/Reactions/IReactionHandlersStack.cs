using System;
using System.Collections.Generic;
using ResourceSystem.Reactions;

namespace ColonyShared.SharedCode.Reactions
{
    
    public delegate void ReactionHandlersStackDelegate(IEnumerable<ReactionHandlerTuple> activated, IEnumerable<ReactionHandlerTuple> deactivated);
    
    public interface IReactionHandlersStack
    {
        IEnumerable<ReactionHandlerTuple> ActiveHandlers { get; }
        
        event ReactionHandlersStackDelegate ActiveHandlersChanged;

        void Modify(object handlerOwner, Action<IReactionHandlersStackModifier> modifier);
    }

    public interface IReactionHandlersStackModifier
    {
        void AddHandler(object handlerOwner, ReactionDef reaction, IReactionHandlerDescriptor handler);

        void RemoveHandler(object handlerOwner, ReactionDef action, IReactionHandlerDescriptor handler);

        void RemoveHandler(object handlerOwner, ReactionDef action);

        void RemoveHandlers(object handlerOwner);     
    }
}