using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Environment.Logging.Extension;
using NLog;
using ResourceSystem.Reactions;

namespace ColonyShared.SharedCode.Reactions
{
    public class ReactionHandlersStack : IReactionHandlersStack
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HandlersStack _stack = new HandlersStack();
        private List<HandlerHolder> _activeHandlers = new List<HandlerHolder>();

        public event ReactionHandlersStackDelegate ActiveHandlersChanged;

        public IEnumerable<ReactionHandlerTuple> ActiveHandlers => _activeHandlers.Select(x => new ReactionHandlerTuple(x.Reaction, x.Handler));

        public void Modify(object owner, Action<IReactionHandlersStackModifier> modifier)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (modifier == null) throw new ArgumentNullException(nameof(modifier));
            lock (_stack)
                modifier.Invoke(_stack);
            GatherActiveHandlers();
        }

        private class HandlersStack : IReactionHandlersStackModifier
        {
            public readonly List<HandlerHolder> Handlers = new List<HandlerHolder>();

            public void AddHandler(object handlerOwner, ReactionDef reaction, IReactionHandlerDescriptor handler)
            {
                Handlers.Add(new HandlerHolder(handlerOwner, reaction, handler));
            }

            public void RemoveHandler(object handlerOwner, ReactionDef action, IReactionHandlerDescriptor handler)
            {
                if (handlerOwner == null) throw new ArgumentNullException(nameof(handlerOwner));
                if (handler == null) throw new ArgumentNullException(nameof(handler));
                var idx = Handlers.FindLastIndex(x =>
                    x.Owner.Equals(handlerOwner) && x.Handler.Equals(handler) && x.Reaction.Equals(action));
                if (idx == -1) return;
                var holder = Handlers[idx];
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Remove handler | {holder}").Write();
                Handlers.RemoveAt(idx);
            }

            public void RemoveHandler(object handlerOwner, ReactionDef action)
            {
                if (handlerOwner == null) throw new ArgumentNullException(nameof(handlerOwner));
                if (action == null) throw new ArgumentNullException(nameof(action));
                var idx = Handlers.FindLastIndex(x => x.Owner.Equals(handlerOwner) && x.Reaction.Equals(action));
                if (idx == -1) return;
                var holder = Handlers[idx];
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Remove handler | {holder}").Write();
                Handlers.RemoveAt(idx);
            }

            public void RemoveHandlers(object handlerOwner)
            {
                if (handlerOwner == null) throw new ArgumentNullException(nameof(handlerOwner));
                for (int i = Handlers.Count - 1; i >= 0; --i)
                {
                    var holder = Handlers[i];
                    if (!holder.Owner.Equals(handlerOwner)) continue;
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Remove handler | {holder}").Write();
                    Handlers.RemoveAt(i);
                }
            }
        }

        private void GatherActiveHandlers()
        {
            List<HandlerHolder> gatheredHandlers = new List<HandlerHolder>();
            int handlersCount = 0;
            lock (_stack)
                GatherActiveHandlers(_stack.Handlers, gatheredHandlers, ref handlersCount);
            var oldHandlers = Interlocked.Exchange(ref _activeHandlers, gatheredHandlers);
            var deactivated = oldHandlers.Except(gatheredHandlers).Select(x => new ReactionHandlerTuple(x.Reaction, x.Handler)).ToArray();
            var activated = gatheredHandlers.Except(oldHandlers).Select(x => new ReactionHandlerTuple(x.Reaction, x.Handler)).ToArray();
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message(
                $"Active handlers {_activeHandlers.Count} |\n{string.Join("\n", _activeHandlers.Select(x => "    " + x))}\n" +
                $"Activated |\n{string.Join("\n", activated.Select(x => "    " + x.ToString()))}\n" +
                $"Deactivated |\n{string.Join("\n", deactivated.Select(x => "    " + x.ToString()))}\n" +
                $"Total | Handlers:{handlersCount}"
            ).Write();            
            if(activated.Length > 0 || deactivated.Length > 0)
                ActiveHandlersChanged?.Invoke(activated, deactivated);
        }
        
        private static void GatherActiveHandlers(List<HandlerHolder> stack, List<HandlerHolder> handlers, ref int handlersCount)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var holder = stack[i];
                if (handlers.All(x => x.Reaction != holder.Reaction))
                {
                    if (!holder.Handler.IsDummy)
                        handlers.Add(holder);
                }
                ++handlersCount;
            }
        }

        private class HandlerHolder
        {
            public readonly ReactionDef Reaction;
            public readonly IReactionHandlerDescriptor Handler;
            public readonly object Owner;

            public HandlerHolder(object owner, ReactionDef reaction, IReactionHandlerDescriptor handler)
            {
                Owner = owner;
                Reaction = reaction;
                Handler = handler;
            }

            public override string ToString() => $"{Reaction.ReactionToString()} <-> {Handler.HandlerToString()} Owner:{Owner}";
        }
    }
}
