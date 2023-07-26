using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedCode.Utils.DebugCollector;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public partial class InputActionsProcessor
    {
        private interface IHandlersHolder : IDisposable
        {} 

        private abstract class HandlersHolder<THandler, TEvent> : ThreadSafeDisposable, IHandlersHolder where THandler : class, IInputActionHandler 
        {
            private readonly IInputActionHandlersFactory _handlersFactory;
            private readonly DisposableComposite _disposables;
            private readonly List<StackEntry> _stack = new List<StackEntry>();
            private readonly Action<Task, object> _disposeHandlersFunc;
            private readonly object _lock = new object();
            private Action<Task, object> _sendToHandlersFunc;
            private Task _jobsQueueTail = Task.CompletedTask;
            private TEvent _lastEvent;

            protected HandlersHolder(
                InputActionDef action, 
                IStream<TEvent> inputStream,
                IStream<IEnumerable<InputActionBinding>> stackStream,
                IInputActionHandlersFactory handlersFactory,
                IInputActionStatesRecipient redirector,
                DisposableComposite disposables,
                Guid entityId,
                NLog.Logger logger)
            {
                Logger = logger;
                EntityId = entityId;
                Action = action ?? throw new ArgumentNullException(nameof(action));
                _disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
                _handlersFactory = new InternalHandlersFactory(handlersFactory, redirector, ProcessLoopbackInputEvent);
                inputStream.Action(_disposables, ProcessInputEvent);
                stackStream.Action(_disposables, ProcessStackChanges);
                _disposeHandlersFunc = (t, obj) => { try { var list = (List<StackEntry>)obj; foreach (var entry in list) entry.Handler?.Dispose(); } catch (Exception e) { Logger.IfError()?.Exception(e).Write(); } };
            }

            protected abstract TEvent StateEvent(TEvent @event);
            protected abstract void LogSendEvent(TEvent @event);
            protected abstract HandlerWrapper CreateWrapper(THandler handler);
            protected abstract void ProcessLoopbackInputEvent(InputActionState @event, int bindingId);
            
            // ReSharper disable once MemberHidesStaticFromOuterClass
            protected readonly NLog.Logger Logger;
            protected readonly InputActionDef Action;
            protected readonly Guid EntityId;

            protected void ProcessInputEvent(TEvent @event, int bindingId)
            {
                if(EnterIfNotDisposed()) try
                {
                    lock (_lock)
                    {
                        if (!EqualityComparer<TEvent>.Default.Equals(@event, _lastEvent))
                        {
                            _lastEvent = StateEvent(@event); // запоминаем только Active/!Active, чтобы пропускать каждый приходящий Activated и Deactivated, даже если они повторяются
                            EnqueueSendState(@event, bindingId);
                        }
                    }
                } finally { ExitAndDisposeIfRequested(); }
            }

            private void ProcessInputEvent(TEvent @event)
            {
                ProcessInputEvent(@event, -1);
            }
            
            private void ProcessStackChanges(IEnumerable<InputActionBinding> stack)
            {
                if(EnterIfNotDisposed()) try
                {
                    lock (_lock)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfTrace()?.Message(EntityId, $"Process stack changes | Action:{Action.ActionToString()} Stack:[{string.Join(", ", stack)}]")
                            .Write();
                        
                        int idx = 0;
                        if (stack != null)
                            foreach (var binding in stack)
                            {
                                if (binding.Action != Action) throw new Exception($"Invalid binding action | Action:{Action} Binding:{binding}");
                                bool found = false;
                                for (int i = idx; i < _stack.Count; ++i)
                                {
                                    if (_stack[i].BindingId == binding.Id)
                                    {
                                        (_stack[idx], _stack[i]) = (_stack[i], _stack[idx]);
                                        found = true;
                                    }
                                }
                                if (!found)
                                {
                                    var handler = _handlersFactory.Create<THandler>(Action, binding.Handler, binding.Id);
                                    _stack.Insert(idx, new StackEntry(binding.Id, binding.Handler, binding.Context, CreateWrapper(handler)));
                                }
                                idx++;
                            }

                        if (_stack.Count > idx)
                        {
                            var toDispose = _stack.GetRange(idx, _stack.Count - idx);
                            _stack.RemoveRange(idx, _stack.Count - idx);
                            EnqueueDisposeHandlers(toDispose);
                        }

                        if (_stack.Count > 0)
                        {
                            var stackCopy = _stack.ToArray();

                            void SendToHandlersFunc(Task t, object obj)
                            {
                                var (@event, bindingId) = (ValueTuple<TEvent,int>) obj;
                                int i = 0;
                                for (; bindingId != -1 && i < stackCopy.Length; ++i)
                                    if (stackCopy[i].BindingId == bindingId)
                                        bindingId = -1;
                                if (bindingId != -1)
                                    i = 0;
                                bool active = true;
                                InputActionHandlerContext context = default;
                                for (; i < stackCopy.Length; ++i)
                                {
                                    var entry = stackCopy[i];
                                    MergeContext(ref context, stackCopy[i].Context);
                                    try 
                                    {
                                        if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Process event | Binding:#{entry.BindingId} Action:{Action.ActionToString()} Handler:{entry.Handler} Event:{@event} Context:{context} Inactive:{!active}").Write();
                                        entry.Handler.ProcessEvent(Action, active ? @event : default, context, !active, Logger);
                                        if (!entry.Handler.PassThrough) active = false;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.IfError()?.Message(e, $"Exception while processing event | Binding:#{entry.BindingId} Action:{Action.ActionToString()} Handler:{entry.Handler} Event:{@event} Inactive:{!active} \n {e.Message} \n {e.StackTrace}").Write();
                                    }
                                }

                            }

                            _sendToHandlersFunc = SendToHandlersFunc;
                        }
                        else
                            _sendToHandlersFunc = null;

                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Action.ActionToString()} <-> {string.Join("\n    ", _stack.Select(x => $"Binding:#{x.BindingId} Desc:{x.Desc.HandlerToString()} Handler:{x.Handler}"))}").Write();

                        EnqueueSendState(_lastEvent, 0);
                    }
                    
                } finally { ExitAndDisposeIfRequested(); }
            }

            private void EnqueueSendState(TEvent @event, int bindingId)
            {
                LogSendEvent(@event);
                if (_sendToHandlersFunc != null)
                    _jobsQueueTail = _jobsQueueTail.ContinueWith(_sendToHandlersFunc, (@event, bindingId), TaskContinuationOptions.RunContinuationsAsynchronously);
                else
                    if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"No handlers | Action:{Action.ActionToString()} Event:{@event}").Write();
            }

            private void EnqueueDisposeHandlers(List<StackEntry> list)
            {
                if (list.Count > 0)
                    _jobsQueueTail = _jobsQueueTail.ContinueWith(_disposeHandlersFunc, list, TaskContinuationOptions.RunContinuationsAsynchronously);
            }

            private void MergeContext(ref InputActionHandlerContext ctx, in InputActionHandlerContext @new)
            {
                ctx = new InputActionHandlerContext(ctx.CurrentSpell.IsValid ? ctx.CurrentSpell : @new.CurrentSpell);
            }
            
            protected override void DisposeImpl()
            {
                _disposables.Dispose();
                EnqueueDisposeHandlers(_stack);
            }

            private readonly struct StackEntry
            {
                public readonly long BindingId;
                public readonly HandlerWrapper Handler;
                public readonly InputActionHandlerContext Context;
                public readonly IInputActionHandlerDescriptor Desc;

                public StackEntry(long bindingId, IInputActionHandlerDescriptor desc, InputActionHandlerContext context, HandlerWrapper handler)
                {
                    BindingId = bindingId;
                    Desc = desc;
                    Context = context;
                    Handler = handler;
                }
            }

            protected abstract class HandlerWrapper
            {
                private TEvent _lastEvent;
                protected readonly THandler Handler;
                protected HandlerWrapper(THandler handler)
                {
                    Handler = handler;
                }
                public bool PassThrough => Handler.PassThrough;
                public void ProcessEvent(InputActionDef action, TEvent @event, InputActionHandlerContext ctx, bool inactive, NLog.Logger logger)
                {
                    if (Handler == InputActionHandler.Null)
                    {
                        if(logger.IsTraceEnabled) logger.IfTrace()?.Message($"Handler is null | Event:{@event}").Write();
                        return;
                    }
                    if (EqualityComparer<TEvent>.Default.Equals(@event, _lastEvent))
                    {
                        if (logger.IsTraceEnabled) logger.IfTrace()?.Message($"Event is same as previous. Skipping... | Hanler:{Handler} Event:{@event} PrevEvent:{_lastEvent}").Write();
                        return;
                    }
                    ProcessEventImpl(_lastEvent = @event, ctx, inactive);
                }
                public void Dispose() => Handler.Dispose();
                protected abstract void ProcessEventImpl(TEvent @event, InputActionHandlerContext ctx, bool inactive);
                public override string ToString() => Handler.ToString();
            }
        }
        
        
        //----------------------------------------------------------------------------------------------------------------------
        private class TriggerHandlersHolder : HandlersHolder<IInputActionTriggerHandler, InputActionTriggerState>
        {
            (long,TriggerHandlersHolder) _lastActivatedEventId;

            public TriggerHandlersHolder(
                InputActionTriggerDef action, 
                IStream<InputActionTriggerState> inputStream, 
                IStream<IEnumerable<InputActionBinding>> stackStream,
                IInputActionHandlersFactory handlersFactory,
                IInputActionStatesRecipient redirector,
                DisposableComposite disposables,
                Guid entityId) : base(action, inputStream, stackStream, handlersFactory, redirector, disposables, entityId, LoggerTriggers) {}
            
            protected override void LogSendEvent(InputActionTriggerState @event)
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Send event | Action:{Action.ActionToString()} Event:{@event}").Write();
                if (@event.Activated)
                    Collect.IfActive?.EventBgn($"InputActionState.{Action.____GetDebugShortName()}", EntityId, _lastActivatedEventId = (@event.Id, this));
                else if (@event.Deactivated && _lastActivatedEventId.Item2 != null)
                    Collect.IfActive?.EventEnd(_lastActivatedEventId);
            }
            protected override InputActionTriggerState StateEvent(InputActionTriggerState @event) => new InputActionTriggerState(@event.Active, false, false, @event.Awaiter);
            protected override HandlerWrapper CreateWrapper(IInputActionTriggerHandler handler) => new TriggerHandlerWrapper(handler);
            protected override void ProcessLoopbackInputEvent(InputActionState @event, int bindingId) => ProcessInputEvent(@event.TriggerState, bindingId);
            private class TriggerHandlerWrapper : HandlerWrapper
            {
                public TriggerHandlerWrapper(IInputActionTriggerHandler handler) : base(handler) {}
                protected override void ProcessEventImpl(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive) => Handler.ProcessEvent(@event, ctx, inactive);
            }
        }


        //----------------------------------------------------------------------------------------------------------------------
        private class ValueHandlersHolder : HandlersHolder<IInputActionValueHandler, InputActionValueState>
        {
            public ValueHandlersHolder(
                InputActionValueDef action, 
                IStream<InputActionValueState> inputStream, 
                IStream<IEnumerable<InputActionBinding>> stackStream,
                IInputActionHandlersFactory handlersFactory,
                IInputActionStatesRecipient redirector,
                DisposableComposite disposables,
                Guid entityId) : base(action, inputStream, stackStream, handlersFactory, redirector, disposables, entityId, LoggerValues) {}
            protected override InputActionValueState StateEvent(InputActionValueState @event) => new InputActionValueState(@event.Value);
            protected override void LogSendEvent(InputActionValueState @event)
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Send event | Action:{Action.ActionToString()} Event:{@event}").Write();
            }
            protected override HandlerWrapper CreateWrapper(IInputActionValueHandler handler) => new ValueHandlerWrapper(handler);
            protected override void ProcessLoopbackInputEvent(InputActionState @event, int bindingId) => ProcessInputEvent(@event.ValueState, bindingId);
            private class ValueHandlerWrapper : HandlerWrapper
            {
                public ValueHandlerWrapper(IInputActionValueHandler handler) : base(handler) {}
                protected override void ProcessEventImpl(InputActionValueState @event, InputActionHandlerContext ctx, bool inactive) => Handler.ProcessEvent(@event, ctx, inactive);
            }
        }
    }
}
