using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using GeneratedDefsForSpells;
using SharedCode.Utils.Threads;
using SharedCode.Wizardry;
using static ColonyShared.SharedCode.Wizardry.SpellCastBuilder;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionHandlerSpellContinuous : IInputActionTriggerHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private enum State { FinishedByRequest, Running, WaitForDelayedFinishByRequest, WaitForFinishByRequest, FinishedByItself }
        private enum Order { MustBeStarted, MustBeFinished, MustBeRestarted, FinishedAndMustBeRestartedByItself, Finished, FinishDelayComplete, FinishDelayCancelled }
        [Flags] private enum OrderFlags { Immediately = 0x10000 }

        private readonly ISpellDoer _spellDoer;
        private readonly int _bindingId;
        private readonly SpellDef _spell;
        private readonly IEnumerable<SpellParameterDef> _parameters;
        private readonly SpellRestartReasonMask _restartReason;
        private readonly SpellFinishMethod _finishMethod;
        private readonly int _finishDelay;
        private readonly bool _chain;
        private readonly object _orderLock = new object();
        private readonly object _contextLock = new object();
        private ISpellDoerCastPipeline _cast;
        private InputActionHandlerContext __context;
        private State __state;
        private bool _restartAfterFinish;
        private CancellationTokenSource _delayCancellation;

        private State _state
        {
            get => __state;
            set
            {
                if (Logger.IsTraceEnabled && __state != value) Logger.IfTrace()?.Message($"State changed | Binding:#{_bindingId} State:{value}").Write();
                __state = value;
            }
        }
        private InputActionHandlerContext _context
        {
            get
            {
                lock (_contextLock)
                    return __context;
            }
            set
            {
                lock (_contextLock)
                    __context = value;
            }
        }

        public InputActionHandlerSpellContinuous(IInputActionHandlerSpellContinuousDescriptor desc, ISpellDoer spellDoer, int bindingId)
        {
            if (desc == null) throw new ArgumentNullException(nameof(desc));
            _spellDoer = spellDoer ?? throw new ArgumentNullException(nameof(spellDoer));
            _bindingId = bindingId;
            _spell = desc.Spell ?? throw new ArgumentNullException(nameof(desc.Spell));
            _restartReason = desc.RestartIfReason;
            _finishDelay = (int)SyncTime.FromSeconds(desc.FinishDelay);
            _finishMethod = desc.FinishMethod;
            _parameters = desc.Parameters;
            _chain = desc.Chain;
        }

        public bool PassThrough => false;

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"ProcessEvent | Binding:#{_bindingId} Event:{@event} Context:{ctx}").Write();

            _context = ctx;

            if (@event.Activated)
            {
                ExecuteOrder(Order.MustBeRestarted, @event.Awaiter);
            }
            else
            if (@event.Active)
            {
                ExecuteOrder(Order.MustBeStarted, @event.Awaiter);
            }
            else
            {
                ExecuteOrder(Order.MustBeFinished, @event.Awaiter, (inactive ? OrderFlags.Immediately : 0));
            }
        }

        private void ExecuteOrder(Order order, AwaitableSpellDoerCast awaiter, OrderFlags flags = 0)
        {
            lock (_orderLock)
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"ExecuteOrder | Binding:#{_bindingId} State:{_state} Order:{order}").Write();
                switch (_state)
                {
                    case State.FinishedByRequest:
                        switch (order)
                        {
                            case Order.MustBeRestarted:
                            case Order.MustBeStarted:
                                _state = State.Running;
                                _restartAfterFinish = false;
                                _cast = StartCast(_context, awaiter);
                                break;
                        }

                        break;
                    case State.Running:
                        switch (order)
                        {
                            case Order.MustBeFinished:
                                _restartAfterFinish = false;
                                if (_finishDelay <= 0 || (flags & OrderFlags.Immediately) != 0)
                                {
                                    _state = State.WaitForFinishByRequest;
                                    FinishCast(_cast, (flags & OrderFlags.Immediately) != 0);
                                }
                                else
                                {
                                    _state = State.WaitForDelayedFinishByRequest;
                                    DelayedFinishCast(_cast, _finishDelay, awaiter);
                                }
                                break;
                            case Order.MustBeRestarted:
                                _restartAfterFinish = true;
                                break;
                            case Order.Finished:
                                _state = State.FinishedByItself;
                                break;
                            case Order.FinishedAndMustBeRestartedByItself:
                                _state = State.FinishedByRequest;
                                ExecuteOrder(Order.MustBeRestarted, awaiter);
                                break;
                        }

                        break;
                    case State.WaitForDelayedFinishByRequest:
                        switch (order)
                        {
                            case Order.MustBeFinished:
                                _restartAfterFinish = false;
                                if ((flags & OrderFlags.Immediately) != 0)
                                {
                                    CancelDelayedFinishCast();
                                    _state = State.WaitForFinishByRequest;
                                    FinishCast(_cast, true);
                                }
                                break;
                            case Order.MustBeRestarted:
                                CancelDelayedFinishCast();
                                _restartAfterFinish = true;
                                break;
                            case Order.Finished:
                            case Order.FinishedAndMustBeRestartedByItself:
                                CancelDelayedFinishCast();
                                _state = State.FinishedByRequest;
                                if (_restartAfterFinish)
                                    ExecuteOrder(Order.MustBeRestarted, awaiter);
                                break;
                            case Order.FinishDelayComplete:
                                _state = State.WaitForFinishByRequest;
                                break;
                            case Order.FinishDelayCancelled:
                                _state = State.Running;
                                break;
                        }

                        break;
                    case State.WaitForFinishByRequest:
                        switch (order)
                        {
                            case Order.MustBeFinished:
                                _restartAfterFinish = false;
                                if ((flags & OrderFlags.Immediately) != 0)
                                    FinishCast(_cast, true);
                                break;
                            case Order.MustBeRestarted:
                                _restartAfterFinish = true;
                                break;
                            case Order.Finished:
                            case Order.FinishedAndMustBeRestartedByItself:
                                _state = State.FinishedByRequest;
                                if (_restartAfterFinish)
                                    ExecuteOrder(Order.MustBeRestarted, awaiter);
                                break;
                        }

                        break;
                    case State.FinishedByItself: // сюда попадаем, только если спелл завершился сам и не было MustBeFinished и он не должен был рестартануть сам
                        switch (order)
                        {
                            case Order.MustBeFinished:
                                _state = State.FinishedByRequest;
                                break;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private ISpellDoerCastPipeline StartCast(InputActionHandlerContext ctx, AwaitableSpellDoerCast awaiter)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Start continuous spell | Binding:#{_bindingId} Spell:{_spell.____GetDebugAddress()} Context:[{ctx}]").Write();
            var prevSpell = _chain ? ctx.CurrentSpell : SpellId.Invalid;
            var spell = new SpellCastBuilder().SetSpell(_spell).SetParameters(_parameters, new SetParameterContext(prevSpellId: prevSpell));
            var cast = prevSpell.IsValid ? _spellDoer.DoCastChain(spell, prevSpell, awaiter) : _spellDoer.DoCast(spell, awaiter);
            if (Logger.IsTraceEnabled)
                cast.SpellGotIdTask.ContinueWith(t => Logger.IfTrace()?.Message($"Continuous spell started | Binding:#{_bindingId} Spell:{_spell.____GetDebugAddress()} SpellId:{t.Result}").Write());
            cast.SpellFinishedTask.ContinueWith(t =>
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Continuous spell finished | Binding:#{_bindingId} Spell:{_spell.____GetDebugAddress()} SpellId:{t.Result.Id} Reason:{t.Result.Reason}").Write();
                if (CanBeRestarted(cast, _restartReason))
                    ExecuteOrder(Order.FinishedAndMustBeRestartedByItself, awaiter);
                else
                    ExecuteOrder(Order.Finished, awaiter);
            });
            return cast;
        }

        private void FinishCast(ISpellDoerCastPipeline cast, bool immediately)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Finish continuous spell | Binding:#{_bindingId} Spell:{_spell.____GetDebugAddress()} SpellId:{cast.Id.ToString()} Method:{_finishMethod}").Write();
            cast.FinishCast(immediately ? SpellFinishMethod.Stop : _finishMethod);
        }

        private void DelayedFinishCast(ISpellDoerCastPipeline cast, int delay, AwaitableSpellDoerCast awaiter)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Finish continuous spell with delay | Binding:#{_bindingId} Spell:{_spell.____GetDebugAddress()} SpellId:{cast.Id.ToString()} Delay:{delay}").Write();
            var cancellation = new CancellationTokenSource();
            var cancellationToken = cancellation.Token;
            TaskEx.Run(async () =>
            {
                try
                {
                    await Task.Delay(delay, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    ExecuteOrder(Order.FinishDelayComplete, awaiter);
                    FinishCast(cast, false);
                }
                catch (TaskCanceledException)
                {
                    if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Delayed finish cancelled | Binding:#{_bindingId} SpellId:{cast.Id.ToString()}").Write();
                    ExecuteOrder(Order.FinishDelayCancelled, awaiter);
                }
            });
            _delayCancellation = cancellation;
        }

        private void CancelDelayedFinishCast()
        {
            var delayCancellation = _delayCancellation;
            _delayCancellation = null;
            delayCancellation.Cancel();
            delayCancellation.Dispose();
        }

        private static bool CanBeRestarted([NotNull] ISpellDoerCastPipeline order, SpellRestartReasonMask reasonMask)
        {
            switch (order.State)
            {
                case CastPipelineState.FailedToCastLocally:
                case CastPipelineState.FailedToCastRemote:
                    return (reasonMask & SpellRestartReasonMask.FailOnStart) != 0;
            }

            switch (order.FinishReason)
            {
                case FinishReasonType.None: return false;
                case FinishReasonType.Success: return (reasonMask & SpellRestartReasonMask.Success) != 0;
                case FinishReasonType.Fail: return (reasonMask & SpellRestartReasonMask.Fail) != 0;
                default: throw new ArgumentOutOfRangeException(nameof(order.FinishReason), order.FinishReason, null);
            }
        }

        public void Dispose()
        {
            var cast = _cast;
            cast?.StopCast();
            var delayCancellation = _delayCancellation;
            delayCancellation?.Cancel();
            delayCancellation?.Dispose();
        }

        public override string ToString() => $"{nameof(InputActionHandlerSpell)}(Spell:{_spell.____GetDebugAddress()} State:{_state})";
    }
}
