using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using SharedCode.Wizardry;
using NLog;
using SharedCode.Utils;
using System.Threading.Tasks;
using SharedCode.Serializers;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using System;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using ReactivePropsNs.ThreadSafe;
using ColonyShared.SharedCode.Utils;
using Assets.Src.Arithmetic;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using ResourceSystem.Utils;
using SharedCode.AI;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class DoInputAction : BehaviourNode
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private DoInputActionDef _def;
        private SpellId _currentOrder;
        private Action<SpellId, SpellFinishReason> _onOrderFinished;
        private long _timeToStop = 0;
        private enum SpellState
        {
            None,
            Running,
            Failed,
            Succeeded
        }
        private SpellState _spellState;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (DoInputActionDef)def;
            _currentOrder = default;
            _spellState = SpellState.None;
            _onOrderFinished = OnOrderFinished;
        }
        DisposableComposite _disposables = new DisposableComposite();
        bool _popped = false;
        AwaitableSpellDoerCast _awaiterToTerminate;
        public override async ValueTask<ScriptResultType> OnStart()
        {

            _popped = false;
            _timeToStop = 0;
            StatusDescription = null;
            _currentOrder = default;

            await SetupTarget(true);
            
            var awaiter = new AwaitableSpellDoerCast() { Finished = _onOrderFinished };
            _awaiterToTerminate = awaiter;
            if (_def.BreakOnTriggerWindow != null)
            {
                var handlers = HostStrategy.CurrentLegionary.Repository.TryGetLockfree<IHasInputActionHandlersClientFull>(HostStrategy.CurrentLegionary.Ref, ReplicationLevel.ClientFull);
                handlers?.InputActionHandlers?.BindingsSource?.Bindings()?.Action(_disposables, (stream) =>
                {
                    foreach (var s in stream)
                        s.Stream.Action(_disposables, (iabs) =>
                        {
                            if (!awaiter.ShouldReactToBindings)
                                return;
                            foreach (var iab in iabs)
                                if (iab.Action == _def.BreakOnTriggerWindow.Target && awaiter.SpellId != default && iab.Handler is InputActionHandlerInputWindowDescriptor iah)
                                {
                                    AsyncUtils.RunAsyncTask(async () =>
                                    {
                                        var secDelay = TimeSpan.FromSeconds(SyncTime.ToSeconds(iah.ActivationTime - SyncTime.Now) - _def.BreakBeforeWindowDelayIn);
                                        if (secDelay.TotalSeconds > 0)
                                            await Task.Delay(secDelay);
                                        if (awaiter.FinishReason == SpellFinishReason.None)
                                        {
                                            awaiter.FinishReason = SpellFinishReason.SucessOnDemand;
                                            awaiter.Finished(awaiter.SpellId, SpellFinishReason.SucessOnDemand);
                                        }
                                    });

                                }

                        });
                });
            }
            HostStrategy.CurrentLegionary.InputActions.PushTrigger(this, _def.Trigger, awaiter);
            if (_def.TimeToHold.Target == null)
            {
                _popped = true;
                HostStrategy.CurrentLegionary.InputActions.PopTrigger(this, _def.Trigger);
            }
            else
            {
                var holdSeconds = await _def.TimeToHold.Target.CalcAsync(HostStrategy.CurrentLegionary.Ref, HostStrategy.CurrentLegionary.Repository);
                _timeToStop = SyncTime.Now + SyncTime.FromSeconds(holdSeconds);
                HostStrategy.ShouldTickWithDelay(holdSeconds + 0.05f);
            }
            _spellState = SpellState.Running;
            awaiter.Finished = _onOrderFinished;

            var value = AsyncEntitiesRepositoryRequestContext.Head?.Context;
            if (value != null)
            {
                Logger.IfFatal()?.Message("Async context is not cleared: {0}. In: DoInputAction", value).Write();
            }
            var resTask = await Task.WhenAny(Task.Delay(400), awaiter.CastResultAwaiter.Task);
            SpellId res = default;
            if (resTask is Task<SpellId> tsi)
            {
                res = await tsi;
            }
            else
            {

            }
            if (res == default)
            {
                return ScriptResultType.Failed;
            }
            _currentOrder = res;
            if (awaiter.FinishReason != SpellFinishReason.None)
            {
                if (awaiter.FinishReason == SpellFinishReason.SucessOnDemand || awaiter.FinishReason == SpellFinishReason.SucessOnTime)
                {
                    return ScriptResultType.Succeeded;
                }
            }

            awaiter.ShouldReactToBindings = true;
            return ScriptResultType.Running;
        }

        public override async ValueTask OnFinish()
        {
            if(!_popped)
                {
                    _popped = true;
                    if (_def.Trigger.Target is InputActionMetaTriggerDef metaTrigger2)
                    {
                        foreach (var trig in metaTrigger2.Actions)
                            HostStrategy.CurrentLegionary.InputActions.PopTrigger(this, trig);
                    }
                    else
                    {
                        HostStrategy.CurrentLegionary.InputActions.PopTrigger(this, _def.Trigger);
                    }
                }
            _disposables.Clear();
            await SetupTarget(false);
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            lock (this)
            {
                if (_spellState == SpellState.Succeeded)
                    return ScriptResultType.Succeeded;
                else if (_spellState == SpellState.Failed)
                    return ScriptResultType.Failed;

                if (_timeToStop < SyncTime.Now && _timeToStop != 0 && !_popped)
                {
                    _popped = true;
                    HostStrategy.CurrentLegionary.InputActions.PopTrigger(this, _def.Trigger);
                }
                return ScriptResultType.Running;

            }
        }

        private void OnOrderFinished(SpellId sid, SpellFinishReason finishReason)
        {
            lock (this)
            {
                //Logger.IfError()?.Message($"OnOrderFinished {_currentOrder} {args.Value.Id} {args.Value.CastData.Def.____GetDebugShortName()}").Write();
                if (_currentOrder != sid)
                    return;
                if (finishReason == SpellFinishReason.FailOnStart)
                {
                    HostStrategy.ShouldTickWithDelay(1f);
                    _spellState = SpellState.Failed;
                }
                else if (finishReason == SpellFinishReason.FailOnDemand || finishReason == SpellFinishReason.FailOnEnd)
                {
                    HostStrategy.ShouldTickImmediately();
                    _spellState = SpellState.Failed;
                }
                else if (finishReason == SpellFinishReason.SucessOnDemand)
                    HostStrategy.ShouldTickImmediately();
                _spellState = SpellState.Succeeded;
                _currentOrder = default;
            }

        }

        public override async ValueTask OnTerminate()
        {
            if (_currentOrder != null)
            {
                _currentOrder = default;
                _spellState = SpellState.Failed;
                if (!_popped)
                {
                    _popped = true;
                    HostStrategy.CurrentLegionary.InputActions.PopTrigger(this, _def.Trigger);
                    if(_awaiterToTerminate.SpellId != default)
                    {
                        using(var ww = await HostStrategy.CurrentLegionary.Repository.Get<IWizardEntityClientFull>(HostStrategy.CurrentLegionary.Ref.Guid))
                        {
                            var wiz = ww.Get<IWizardEntityClientFull>(HostStrategy.CurrentLegionary.Ref.Guid);
                            if (wiz != null)
                                await wiz.CancelSpell(_awaiterToTerminate.SpellId);
                        }
                    }
                }
            }
        }


        private async ValueTask SetupTarget(bool setup)
        {
            if (_def.Target.IsValid)
            {
                Legionary targetAgent = null;
                if (setup)
                {
                    var targetExpr = (TargetSelector) await _def.Target.ExprOptional(HostStrategy);
                    if (targetExpr != null)
                        targetAgent = await targetExpr.SelectTarget(HostStrategy.CurrentLegionary);
                }

                var repo = HostStrategy.CurrentLegionary.Repository;
                var entityRef = HostStrategy.CurrentLegionary.Ref;
                
                Logger.IfDebug()
                    ?.Message(entityRef.Guid, $"Set target | Target:{targetAgent.Ref}")
                    .Write();

                using (var cnt = await repo.Get(entityRef))
                {
                    if (cnt.TryGet<IHasAiTargetRecipient>(entityRef.TypeId, entityRef.Guid, out var hasTargetRecipient))
                    {
                        await hasTargetRecipient.AiTargetRecipient.SetTarget(targetAgent != null && targetAgent.IsValid ? targetAgent.Ref.To() : OuterRef.Invalid);
                    }
                    else if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message(HostStrategy.CurrentLegionary.Ref.Guid, $"Can't get {nameof(IAiTargetRecipient)}").Write();
                }
            }
        }
    }
}