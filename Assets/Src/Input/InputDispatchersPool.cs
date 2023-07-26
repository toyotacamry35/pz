using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using Harmony;
using JetBrains.Annotations;
using NLog;

namespace Src.Input
{
    public class InputDispatchersPool
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(InputDispatchersPool));

        private readonly Dictionary<InputBindingsDef, Stack<IInputDispatcher>> _free = new Dictionary<InputBindingsDef, Stack<IInputDispatcher>>();
        private readonly Dictionary<IInputDispatcher, InputBindingsDef> _acquired = new Dictionary<IInputDispatcher, InputBindingsDef>();

        public IInputDispatcher Acquire([NotNull] InputBindingsDef bindings)
        {
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));

            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Acuire Input Dispatcher | Bindings:{bindings}").Write();

            if (!_free.TryGetValue(bindings, out var dispatchers))
                _free.Add(bindings, dispatchers = new Stack<IInputDispatcher>());

            IInputDispatcher dispatcher;
            if (dispatchers.Count == 0)
            {
                var builder = new InputDispatcherBuilder();
                builder.AddBindings(bindings);
                dispatcher = builder.Build();
            }
            else
                dispatcher = dispatchers.Pop();
            
            _acquired.Add(dispatcher, bindings);
            return dispatcher;
        }

        public void Release([NotNull] IInputDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

            if (!_acquired.TryGetValue(dispatcher, out var bindings))
            {
                Logger.IfWarn()?.Message("Releasing unacquired input dispatcher").Write();
                return;
            }
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Release Input Dispatcher | Bindings:{bindings}").Write();
            _acquired.Remove(dispatcher);
            if (_free.TryGetValue(bindings, out var dispatchers))
                dispatchers.Add(dispatcher);
        }
    }
}