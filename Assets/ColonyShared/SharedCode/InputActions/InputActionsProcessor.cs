using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
     public partial class InputActionsProcessor : ThreadSafeDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger LoggerTriggers = LogManager.GetLogger($"{Logger.Name}.Triggers");
        private static readonly NLog.Logger LoggerValues = LogManager.GetLogger($"{Logger.Name}.Values");

        private readonly IInputActionHandlersFactory _handlersFactory;
        private readonly IInputActionStatesSource _actionStatesSource;
        private readonly InputActionStatesPipe _redirectionPipe;
        private readonly InputActionStatesCapacitor _capacitor;
        private readonly Dictionary<InputActionDef,IHandlersHolder> _handlers = new Dictionary<InputActionDef, IHandlersHolder>();
        private readonly DisposableComposite _disposables = new DisposableComposite();
        private readonly Guid _entityId;

        public InputActionsProcessor(
            [NotNull] IInputActionBindingsSource bindingsSource, 
            [NotNull] IInputActionHandlersFactory handlersFactory, 
            [NotNull] IInputActionStatesSource actionStatesSource,
            Guid entityId)
        {
            _handlersFactory = handlersFactory ?? throw new ArgumentNullException(nameof(handlersFactory));
            if(actionStatesSource == null) throw new ArgumentNullException(nameof(actionStatesSource));
            _redirectionPipe = new InputActionStatesPipe(LoggerTriggers, LoggerValues);
            _disposables.Add(_redirectionPipe);
            var compositeSource = new InputActionStatesCompositeSource(new[]{_redirectionPipe, actionStatesSource}, LoggerTriggers, LoggerValues);
            _disposables.Add(compositeSource);
            _capacitor = new InputActionStatesCapacitor(compositeSource, LoggerTriggers, LoggerValues);
            _disposables.Add(_capacitor);
            _actionStatesSource = _capacitor;
            _entityId = entityId;
            bindingsSource.Bindings().Action(_disposables, ProcessBindingsChanges);
            bindingsSource.BindingsWait().Action(_disposables, ProcessBindingsWait);
        }

        protected override void DisposeImpl()
        {
            _handlers.Clear();
            _disposables.Dispose();
        }

        private void ProcessBindingsChanges(IEnumerable<InputActionBindingsStream> bindings)
        {
            if(EnterIfNotDisposed()) try
            {
                if (Logger.IsDebugEnabled) Logger.IfTrace()?.Message(_entityId, $"Process binding changes | Actions:[{string.Join("\n", bindings.Select(x => x.Action.ActionToString()))}]")
                    .Write();
                foreach (var binding in bindings)
                {
                    lock(_handlers)
                        if (!_handlers.ContainsKey(binding.Action))
                        {
                            if (Logger.IsDebugEnabled) Logger.IfTrace()?.Message(_entityId, $"Add holder | Action:{binding.Action.ActionToString()}").Write();
                            var holder = CreateHolder(binding);
                            _handlers[binding.Action] = holder;
                            _disposables.Add(holder);
                        }
                }
            } finally { ExitAndDisposeIfRequested(); }
        }

        private void ProcessBindingsWait(bool wait)
        {
            if (wait)
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_entityId, "Processing suspended").Write();
                _capacitor.Accumulate();
            }
            else
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_entityId, "Processing resumed").Write();
                _capacitor.Flush();
            }
        }
        
        private IHandlersHolder CreateHolder(InputActionBindingsStream binding)
        {
            var disposables = new DisposableComposite();
            switch (binding.Action)
            {
                case InputActionMetaTriggerDef metaAction:
                    return new TriggerHandlersHolder(metaAction, ConstructMetaTriggerStream(metaAction, _actionStatesSource, disposables), binding.Stream, _handlersFactory, _redirectionPipe, disposables, _entityId);
                case InputActionTriggerDef triggerAction:
                    return new TriggerHandlersHolder(triggerAction, _actionStatesSource.Stream(triggerAction), binding.Stream, _handlersFactory, _redirectionPipe, disposables, _entityId);
                case InputActionValueDef valueAction:
                    return new ValueHandlersHolder(valueAction, _actionStatesSource.Stream(valueAction), binding.Stream, _handlersFactory, _redirectionPipe, disposables, _entityId);
                default: 
                    throw new NotSupportedException($"{nameof(binding.Action)}.GetType()={binding.Action.GetType()}");
            }
        }
        
        private IStream<InputActionTriggerState> ConstructMetaTriggerStream(InputActionMetaTriggerDef action, IInputActionStatesSource source, DisposableComposite disposables)
        {
            IStream<InputActionTriggerState> metaStream = null;
            if (action.Actions != null)
                for(var itr = action.Actions.GetEnumerator(); itr.MoveNext(); )
                {
                    var otherStream = source.Stream(itr.Current);
                    metaStream = metaStream == null
                        ? otherStream
                        : metaStream.ZipFunc(disposables, otherStream, (x, y) => 
                            new InputActionTriggerState(
                                active: x.Active && y.Active, 
                                activated: x.Active && y.Active && (x.Activated || y.Activated), 
                                deactivated: x.Deactivated && y.Active || y.Deactivated && x.Active, 
                                x.Awaiter ?? y.Awaiter), 
                            useDefaultValue: true);
                }
            
            if (metaStream == null) throw new Exception($"{action} not contains actions!");
            
            IStream<InputActionTriggerState> notStream = null;
            if (action.NotActions != null)
                for(var itr = action.NotActions.GetEnumerator(); itr.MoveNext(); )
                {
                    var otherStream = source.Stream(itr.Current);
                    notStream = notStream == null
                        ? otherStream
                        : notStream.ZipFunc(disposables, otherStream, (x, y) => 
                            new InputActionTriggerState(
                                active: x.Active || y.Active, 
                                activated: x.Activated && !y.Active || y.Activated && !x.Active, 
                                deactivated: !x.Active && !y.Active && (x.Deactivated || y.Deactivated), 
                                x.Awaiter ?? y.Awaiter),
                            useDefaultValue: true);
                }

            if (notStream != null)
            {
                metaStream = metaStream.ZipFunc(
                    disposables,
                    notStream,
                    (x, y) =>
                        new InputActionTriggerState(
                            active: x.Active && !y.Active,
                            activated: x.Activated && !y.Active || y.Deactivated && x.Active,
                            deactivated: x.Deactivated && !y.Active || y.Activated && (x.Active || x.Deactivated),
                            x.Awaiter ?? y.Awaiter),
                    useDefaultValue: true);
            }

            var noMetaStream = source.Stream(action);
            var finalStream = metaStream.ZipFunc(disposables, noMetaStream, (l, r) =>
                new InputActionTriggerState(
                    active: l.Active || r.Active,
                    activated: l.Activated && (!r.Active || r.Activated) || r.Activated && !l.Active,
                    deactivated: l.Deactivated && !r.Active || r.Deactivated && !l.Active,
                    l.Awaiter ?? r.Awaiter),
                useDefaultValue: true);

            return finalStream;
        }
    }
}