using System;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.SpawnSystem;
using Assets.Src.Wizardry;
using Assets.Tools;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using UnityEngine;

namespace Src.InputActions
{
    public class InputActionHandlerInteraction : IInputActionTriggerHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISpellDoer _spellDoer;
        private readonly TargetHolder _targetHolder;
        private readonly InputActionDef _action;
        private readonly Delegate _delegate;
        private readonly int _bindingId;
        private readonly DisposableComposite _disposables = new DisposableComposite();
        private readonly object _lock = new object();
        private Context _context;

        public InputActionHandlerInteraction(InputActionDef action, ISpellDoer spellDoer, TargetHolder targetHolder, Delegate @delegate, int bindingId)
        {
            if (targetHolder == null) throw new ArgumentNullException(nameof(targetHolder));
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _spellDoer = spellDoer ?? throw new ArgumentNullException(nameof(spellDoer));
            _targetHolder = targetHolder;
            _delegate = @delegate;
            _bindingId = bindingId;
            _targetHolder.CurrentTarget.Action(_disposables, OnNewTarget);

        }

        public bool PassThrough => false;

        private void OnNewTarget(GameObject obj)
        {
            lock (_lock)
                _context = new Context(obj);
        }

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            if (@event.Activated)
            {                
                Context context;

                lock (_lock)
                    context = _context;

                if (context.CurrentTarget)
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        if (context.CurrentTarget)
                        {
                            var spell = await context.CurrentTarget.ChooseSpell(_spellDoer, _action);
                            if (spell != null && context.CurrentTarget)
                            { 
                                var order = _spellDoer.DoCast(new SpellCastBuilder().SetSpell(spell).SetTargetIfValid(context.CurrentTargetRef).Build());
                                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Activated | Binding:#{_bindingId} Action:{_action} Target:{context.CurrentTargetName} TargetRef:{context.CurrentTargetRef} Spell:{spell} OrderId:{order.Id}").Write();
                                if (_delegate != null)
                                    UnityQueueHelper.RunInUnityThreadNoWait(() => _delegate.Invoke(order, spell));
                            }
                        }
                    }, _spellDoer.Repository);
            }
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        public delegate void Delegate(ISpellDoerCastPipeline spellOrderId, SpellDef spell);

        private readonly struct Context
        {
            public readonly OuterRef<IEntityObject> CurrentTargetRef;
            public readonly Interactive CurrentTarget;
            public readonly string CurrentTargetName;

            public Context(GameObject obj)
            {
                CurrentTarget = obj ? obj.Kind<Interactive>() : null;
                CurrentTargetRef = obj ? obj.GetEntityRef() : OuterRef<IEntityObject>.Invalid;
                CurrentTargetName = (Logger.IsDebugEnabled && CurrentTarget != null) ? CurrentTarget.ToString() : null;
            }
        }

        public override string ToString() => $"{nameof(InputActionHandlerInteraction)}";
    }
}

