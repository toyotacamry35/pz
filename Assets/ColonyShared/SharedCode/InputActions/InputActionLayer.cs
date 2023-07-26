using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using SharedCode.Utils.DebugCollector;

namespace ColonyShared.SharedCode.InputActions
{
    public partial class InputActionLayersStack
    {
        private class Layer : IInputActionLayer
        {
            private readonly bool _isMaster;
            private readonly Guid _entityId;
            private readonly Func<int> _idGenerator;
            private readonly List<BindingHolder> _handlers = new List<BindingHolder>();
            private bool _unlocked;

            public Layer(object owner, bool isMaster, Guid entityId, Func<int> idGenerator)
            {
                Owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _isMaster = isMaster;
                _entityId = entityId;
                _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            }

            public object Owner { get; }
            
            public void Lock() => _unlocked = false;

            public void Unlock() => _unlocked = true;

            private string Mark => _isMaster ? "Master" : "Client"; 
 
            public void AddBinding(object handlerOwner, InputActionDef action, IInputActionHandlerDescriptor handler, InputActionHandlerContext ctx)
            {
                if (!_unlocked) throw new Exception("Modification of locked input actions layer!");
                if (handlerOwner == null) throw new ArgumentNullException(nameof(handlerOwner));
                switch (action)
                {
                    case InputActionTriggerDef _ when !(handler is IInputActionTriggerHandlerDescriptor):
                        throw new ArgumentException($"{handler.GetType()} can't be used with {nameof(InputActionTriggerDef)} | Handler:{handler.HandlerToString()} Action:{action.ActionToString()}");
                    case InputActionValueDef _ when !(handler is IInputActionValueHandlerDescriptor):
                        throw new ArgumentException($"{handler.GetType()} can't be used with {nameof(InputActionValueDef)} | Handler:{handler.HandlerToString()} Action:{action.ActionToString()}");
                    case InputActionDef _ when handler is IInputActionHandlerRedirectDescriptor d && d.Action == action:
                        throw new ArgumentException($"{handler.GetType()} can't redirect to same action | Handler:{handler.HandlerToString()} Action:{action.ActionToString()}");
                }
                var holder = new BindingHolder(_idGenerator(), action, handler, ctx, handlerOwner);
                if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Add binding | {holder}").Write();
//                Collect.IfActive?.EventBgn($"InputActionHandlersStack.{action.____GetDebugShortName()}.{handler}", _entityId, holder);

                lock(_handlers)
                    _handlers.Add(holder);
            }

            public void RemoveBinding(object bindingOwner, InputActionDef action, IInputActionHandlerDescriptor handler)
            {
                if (!_unlocked) throw new Exception("Modification of locked input actions layer!");
                if (bindingOwner == null) throw new ArgumentNullException(nameof(bindingOwner));
                if (handler == null) throw new ArgumentNullException(nameof(handler));

                lock(_handlers)
                {
                    var idx = _handlers.FindLastIndex(x => x.Owner.Equals(bindingOwner) && x.Handler.Equals(handler) && x.Action.Equals(action));
                    if (idx != -1)
                    {
                        var holder = _handlers[idx];
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Remove binding | {holder}").Write();
//                        Collect.IfActive?.EventEnd(holder);
                        _handlers.RemoveAt(idx);
                    }
                }                
            }

            public void RemoveBinding(object bindingOwner, InputActionDef action)
            {
                if (bindingOwner == null) throw new ArgumentNullException(nameof(bindingOwner));
                if (action == null) throw new ArgumentNullException(nameof(action));
                
                lock(_handlers)
                {
                    var idx = _handlers.FindLastIndex(x => x.Owner.Equals(bindingOwner) && x.Action.Equals(action));
                    if (idx != -1)
                    {
                        var holder = _handlers[idx];
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Remove binding | {holder}").Write();
   //                     Collect.IfActive?.EventEnd(holder);
                        _handlers.RemoveAt(idx);
                    }
                }                
            }
            
            public void RemoveBindings(object bindingOwner)
            {
                if (bindingOwner == null) throw new ArgumentNullException(nameof(bindingOwner));

                lock (_handlers)
                {
                    for (int i = _handlers.Count - 1; i >= 0; --i)
                    {
                        var holder = _handlers[i];
                        if (holder.Owner.Equals(bindingOwner))
                        {
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Remove binding | {holder}").Write();
       //                     Collect.IfActive?.EventEnd(holder);
                            _handlers.RemoveAt(i);
                        }
                    }
                }
            }
            
            public void GatherHandlers(GatherDelegate gather)
            {
                lock(_handlers)
                    for (int i = _handlers.Count - 1; i >= 0; --i)
                    {
                        var holder = _handlers[i];
                        gather(holder.Binding);
                    }
            }
            
            private readonly struct BindingHolder
            {
                public readonly InputActionBinding Binding;
                public readonly object Owner;
                public InputActionDef Action => Binding.Action; 
                public IInputActionHandlerDescriptor Handler => Binding.Handler;
            
                public BindingHolder(int id, InputActionDef action, IInputActionHandlerDescriptor handler, InputActionHandlerContext context, object owner)
                {
                    Owner = owner;
                    Binding = new InputActionBinding(id, action, handler, context);
                }

                public override string ToString() => $"{Binding.ToString()} Owner:{Owner}";
            }
        }
    }
}