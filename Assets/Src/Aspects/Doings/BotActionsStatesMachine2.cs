using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs.ThreadSafe;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Aspects.Doings
{
    public class BotActionsStatesMachine2 : IDisposable, IInputActionStatesSource, IInputActionStatesGenerator
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Bot");

        public readonly IEntitiesRepository Repository;
        public readonly OuterRef EntityRef;
        public Guid EntityId => EntityRef.Guid;
        public Vector3 _camera;
        public Transform Transform;
        public float _time = 0;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly Task _task;
        private readonly BotActionDef _botBehavior;
        private readonly InputActionStatesGenerator _inputActionStates;

        private readonly Stack<BotActionDef> _currentActionsStack = new Stack<BotActionDef>(); // Для отладочных целей


        public Vector3? CameraDirection { get; set; }
        public float SmoothTime { get; set; }
        public ISpellDoer SpellDoer { get; }

        public Stack<BotActionDef> CurrentActionsStack
        {
            get
            {
                lock (_currentActionsStack)
                    return _currentActionsStack;
            }
        }

        public BotActionsStatesMachine2(BotActionDef botBehavior, ISpellDoer spellDoer, OuterRef entityRef, IEntitiesRepository repository)
        {
            //if (botBehavior == null) throw new ArgumentNullException(nameof(botBehavior));

            SpellDoer = spellDoer ?? throw new ArgumentNullException(nameof(spellDoer));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            EntityRef = entityRef;
            

            _inputActionStates = new InputActionStatesGenerator();

            //_task = DoThinking(botBehavior, _cts.Token);
            //_botBehavior = botBehavior;
        }

        private async Task DoThinking(BotActionDef botBehavior, CancellationToken cancellation)
        {
            var knowledgeRoot = new ConcurrentDictionary<string, object>();
            while (true)
            {
                try
                {
                    var res = await BotActions2.InvokeAction(this, botBehavior, knowledgeRoot, _cts.Token, -1);
                    Logger.IfWarn()?.Message("Bot {0} root action is finished with return {1}, restarting", EntityId, res).Write();
                }
                catch (TaskCanceledException e)
                {
                    Logger.IfInfo()?.Message(e, "Cancellation requested for bot {0}", EntityId).Write();
                    break;
                }
                catch (ObjectDisposedException e)
                {
                    Logger.IfWarn()?.Message(e, "Token is disposed, shutting down bot {0}", EntityId).Write();
                    break;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "Exception in bot brain {0}, restarting", EntityId).Write();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellation);
            }
        }

        public override string ToString()
        {
            return $"Bot {EntityId}";
        }

        public void PushCurrentAction(BotActionDef action)
        {
            lock (_currentActionsStack)
                _currentActionsStack.Push(action);
        }

        public void PopCurrentAction(BotActionDef action)
        {
            lock (_currentActionsStack)
                _currentActionsStack.Pop();
        }
        
        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts.Cancel();
                    _cts.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public void PushTrigger(object causer, InputActionTriggerDef action, AwaitableSpellDoerCast cast = default) => _inputActionStates.PushTrigger(causer, action, cast);

        public void PopTrigger(object causer, InputActionTriggerDef action) => _inputActionStates.PopTrigger(causer, action);

        public bool TryPopTrigger(object causer, InputActionTriggerDef action, bool all) => _inputActionStates.TryPopTrigger(causer, action, all);

        public void SetValue(InputActionValueDef action, float value) => _inputActionStates.SetValue(action, value);
        
        IStream<InputActionTriggerState> IInputActionStatesSource.Stream(InputActionTriggerDef action) => _inputActionStates.Stream(action);

        IStream<InputActionValueState> IInputActionStatesSource.Stream(InputActionValueDef action) => _inputActionStates.Stream(action);
    }
}
