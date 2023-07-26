using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.Lib.Extensions;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Entities.Reactions;
using ColonyShared.SharedCode.Reactions;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using NLog;
using ResourceSystem.Reactions;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils.DebugCollector;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    public partial class ReactionsOwner : IHookOnDestroy, IHookOnReplicationLevelChanged
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<ReactionDef, HandlersHolder> _activeHandlers = new ConcurrentDictionary<ReactionDef, HandlersHolder>();

        private IReactionHandlersStack _stack;

        public IReactionHandlersStack ReactionHandlersStack => _stack;

        public async Task InvokeReactionImpl(ReactionDef reaction, ArgTuple[] args)
        {
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(ParentEntityId, $"InvokeReaction | {reaction.ReactionToString()} Args:[{string.Join(", ", args.Select(x => x.ToString()))}]")
                .Write();
            Collect.IfActive?.Event($"HasReactions.InvokeReaction.{reaction.____GetDebugRootName()}", parentEntity.Id);
            if (_activeHandlers.TryGetValue(reaction, out var handler))
            {
                await handler.Invoke(args);
            }
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            if (oldReplicationMask != (long) ReplicationLevel.Master && newReplicationMask == (long) ReplicationLevel.Master)
            {
                if (_stack == null && Interlocked.CompareExchange(ref _stack, new ReactionHandlersStack(), null) == null)
                    AsyncUtils.RunAsyncTask(InitializeHandlersStack, EntitiesRepository);
            }
            else if (oldReplicationMask == (long) ReplicationLevel.Master && newReplicationMask != (long) ReplicationLevel.Master)
            {
                var stack = Interlocked.Exchange(ref _stack, null); 
                if (stack != null)
                    AsyncUtils.RunAsyncTask(() => DeinitializeHandlersStack(stack), EntitiesRepository);
            }
        }

        public async Task OnDestroy()
        {
            var stack = Interlocked.Exchange(ref _stack, null); 
            if (stack != null)
                await DeinitializeHandlersStack(stack);
        }

        private async Task InitializeHandlersStack()
        {
            using (await parentEntity.GetThisExclusive())
            {
                _stack.ActiveHandlersChanged += OnActiveHandlersChanged;
                
                var ownerDef = (parentEntity as IEntityObject)?.Def as IHasReactionsDef;
                if (ownerDef == null)
                    Logger.Warn(
                        $"Def {(parentEntity as IEntityObject)?.Def.____GetDebugShortName()} of entity {parentEntity.Id} is not {nameof(IHasReactionsDef)}");
               
                if (ownerDef?.ReactionHandlers != null && ownerDef.ReactionHandlers.Target?.Reactions != null)
                    _stack.Modify(this, mod =>
                    {
                        foreach (var kv in ownerDef.ReactionHandlers.Target?.Reactions)
                        {
                            mod.AddHandler(this, kv.Key, kv.Value.Target);
                        }
                    });
            }
        }

        private Task DeinitializeHandlersStack(IReactionHandlersStack stack)
        {
            stack.ActiveHandlersChanged -= OnActiveHandlersChanged;
            foreach (var holder in _activeHandlers.Values)
                foreach (var handler in holder.RemoveAll())
                    ReleaseHandler(handler);                                            
            return Task.CompletedTask;
        }

        private void OnActiveHandlersChanged(IEnumerable<ReactionHandlerTuple> activated, IEnumerable<ReactionHandlerTuple> deactivated)
        {
            foreach (var tuple in deactivated)
                try
                {
                    Logger.IfDebug()
                        ?.Message(ParentEntityId, $"Deactivate handler | {tuple}")
                        .Write();
                    var handler = _activeHandlers.TryGetValue(tuple.Reaction, out var holder) ? holder.Remove(tuple.Handler) : null;
                    if (handler != null)
                        ReleaseHandler(handler);
                    else
                        throw new KeyNotFoundException($"Handler for {tuple.Reaction.ReactionToString()} not exists");
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, $"Exception while removing handler {tuple}: {e.Message}").Write();
                }

            foreach (var tuple in activated)
            {
                Logger.IfDebug()
                    ?.Message(ParentEntityId, $"Activate handler | {tuple}")
                    .Write();
                try
                {
                    if (!tuple.Handler.IsDummy)
                    {
                        var handler = CreateHandler(tuple.Handler);
                        _activeHandlers.GetOrAdd(tuple.Reaction, r => new HandlersHolder(r)).Add(tuple.Handler, handler);
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        private IReactionHandler CreateHandler(IReactionHandlerDescriptor desc)
        {
            switch (desc)
            {
                case IReactionHandlerSpellProlongableDescriptor d:
                    return new ReactionHandlerSpellProlongable(d, new OuterRef<IHasWizardEntity>(parentEntity), EntitiesRepository);
                case IReactionHandlerSpellDescriptor d:
                    return new ReactionHandlerSpell(d, new OuterRef<IHasWizardEntity>(parentEntity), EntitiesRepository);
                case IReactionHandlerSpellOnTargetDescriptor d:
                    return new ReactionHandlerSpellOnTarget(d, EntitiesRepository);
                default:
                    throw new NotImplementedException(desc?.GetType().Name);
            }
        }

        private void ReleaseHandler(IReactionHandler handler)
        {
            (handler as IDisposable)?.Dispose();
        }


        private class HandlersHolder
        {
            private ValueTuple<IReactionHandlerDescriptor, IReactionHandler>[] _handlers = Array.Empty<ValueTuple<IReactionHandlerDescriptor, IReactionHandler>>();
            private readonly ReactionDef _reaction;
            private readonly object _lock = new object();

            public HandlersHolder(ReactionDef reaction)
            {
                _reaction = reaction;
            }

            public int HandlersCount => _handlers?.Length ?? 0;

            public async Task Invoke(ArgTuple[] args)
            {
                var handlers = _handlers;
                foreach (var tuple in handlers)
                    await tuple.Item2.Invoke(args);
            }

            public void Add(IReactionHandlerDescriptor desc, IReactionHandler handler)
            {
                lock(_lock)
                    _handlers = _handlers.Append(ValueTuple.Create(desc, handler)).ToArray();
            }

            public IReactionHandler Remove(IReactionHandlerDescriptor desc)
            {
                lock(_lock)
                {
                    var idx = Array.FindLastIndex(_handlers, x => x.Item1 == desc);
                    if (idx != -1)
                    {
                        var removed = _handlers[idx];
                        _handlers = _handlers.Where((x, i) => i != idx).ToArray();
                        return removed.Item2;
                    }
                    throw new KeyNotFoundException($"Handler {desc.HandlerToString()} for {_reaction.ReactionToString()} not exists");
                }
            }

            public IEnumerable<IReactionHandler> RemoveAll()
            {
                lock (_lock)
                {
                    var removed = _handlers;
                    _handlers = Array.Empty<ValueTuple<IReactionHandlerDescriptor, IReactionHandler>>();
                    return removed.Select(x => x.Item2);
                }
            }

            public override string ToString()
            {
                var handlers = _handlers;
                return
                    $"{_reaction.ReactionToString()} <-> [{string.Join("\n", handlers.Select(x => "    " + x.Item1.HandlerToString()))}]";
            }
        }
    }
}