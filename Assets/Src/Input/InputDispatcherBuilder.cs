using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Input;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;

namespace Src.Input
{
    public class InputDispatcherBuilder
    {
        private readonly List<InputBindingDef> _bindings;
        private readonly List<InputActionDef> _blockers;

        public InputDispatcherBuilder()
        {
            _bindings = new List<InputBindingDef>();
            _blockers = new List<InputActionDef>();
        }

        public InputDispatcherBuilder AddBinding([NotNull] InputBindingDef binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));
            _bindings.Add(binding);
            return this;
        }

        public InputDispatcherBuilder AddBlocker([NotNull] InputActionDef action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _blockers.Add(action);
            return this;
        }

        public InputDispatcherBuilder AddBindings(InputBindingsDef bindings)
        {
            if (bindings.Bindings != null)
                _bindings.AddRange(bindings.Bindings.Select(x => x.Target));
            if (bindings.BlockList.IsValid)
                _blockers.AddRange(bindings.BlockList.Target.AllActions);
            return this;
        }

        public InputDispatcher Build()
        {
            var processor = new InputDispatcher();
            
            foreach (var group in _bindings.GroupBy(x => x.Slot))
            {
                var sources = group.SelectMany(x => x.Sources ?? new [] { x.Source } ).Select(x => CreateInputSource(x));
                processor.AddBinding(new InputListener(sources, group.Key.Target.Threshold, group.Key.Target.HoldTime), group.Key);    
            }

            foreach (var action in _blockers.Distinct().Where(x => x != null))
            {
                processor.AddBlocker(action);
            }
            
            return processor;
        }

        public IInputSource CreateInputSource(InputSourceDef binding)
        {
            if (binding is InputKeyDef)
                return new InputSourceKey((InputKeyDef)binding);
            if (binding is InputAxisDef)
                return new InputSourceAxis((InputAxisDef)binding);
            if (binding is InputAxisRdpWorkaroundDef)
                return new InputSourceAxisRdpWorkaround((InputAxisRdpWorkaroundDef)binding);
            throw new NotImplementedException($"{binding}");
        }
    }
}