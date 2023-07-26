using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Modifiers;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using NLog;
using GeneratedDefsForSpells;
using ResourceSystem.Aspects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using Src.ManualDefsForSpells;

namespace SharedCode.Wizardry
{
    internal class SpellDoerCastPipeline : ISpellDoerCastPipeline
    {
        private Action<Func<IEntitiesRepository, Task>> _taskRunner;
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Spells");

        public CastPipelineState State { get { return (CastPipelineState)_state; } private set { _state = (int)value; } }
        public FinishReasonType FinishReason { get { return _finishReason; } private set { _finishReason = value; } }
        public SpellId Id { get { return _id.InterlockedGet(); } private set { _id.InterlockedSet(value); } }
        public Task<SpellId> SpellGotIdTask => _taskSpellGotId.Task;
        public Task<SpellId> SpellCastedTask => _taskSpellCasted.Task;
        public Task<(SpellId Id, SpellFinishReason Reason)> SpellFinishedTask => _taskSpellFinished.Task;
        public SpellDef SpellDef => _spellDef;

        private int _state = (int)CastPipelineState.None;
        private FinishReasonType _finishReason;
        private Action<SpellDoerCastPipeline> _spellStarted;
        private Action<SpellDoerCastPipeline> _spellFinished;
        private Action _brokeOnTimeout;
        private SpellId _id = SpellId.Invalid;
        private SpellDef _spellDef;
        private int _stopped = 0;
        private OuterRef<IWizardEntity> _localWizard;
        private readonly TaskCompletionSource<SpellId> _taskSpellGotId;
        private readonly TaskCompletionSource<SpellId> _taskSpellCasted;
        private readonly TaskCompletionSource<(SpellId, SpellFinishReason)> _taskSpellFinished;
        private ClientSpellRunner _localRunner;
        private int _attemptsToStop = 0;
        private bool _debug;
        AwaitableSpellDoerCast _awaiter;

        public SpellDoerCastPipeline()
        {
            _taskSpellGotId = new TaskCompletionSource<SpellId>(TaskCreationOptions.RunContinuationsAsynchronously);
            _taskSpellCasted = new TaskCompletionSource<SpellId>(TaskCreationOptions.RunContinuationsAsynchronously);
            _taskSpellFinished = new TaskCompletionSource<(SpellId, SpellFinishReason)>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        internal async void SpellFinished(SpellId id, SpellFinishReason reason)
        {
            await _taskSpellGotId.Task;
            if (id == _id)
                SpellFinished(reason);
        }

        private async void SpellFinished(SpellFinishReason reason)
        {
            _stopped = 1;
            await _taskSpellCasted.Task;
            FinishReason = reason.IsFail() ? FinishReasonType.Fail : FinishReasonType.Success;
            State = FinishReason == FinishReasonType.Fail ? CastPipelineState.Failed : CastPipelineState.Succeded;
            _taskSpellFinished.SetResult((Id, reason));
            if (_debug && Logger.IsDebugEnabled) Logger.IfDebug()?.Message("SpellDoerCastPipeline:{0} finished with reason: {1} ({2})", _spellDef, FinishReason, State).Write();
            try
            {
                OnSpellFinished(reason);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }

        public void CastSpell(
            Action<Func<IEntitiesRepository, Task>> taskRunner,
            bool debug,
            Action<SpellDoerCastPipeline> spellStarted,
            Action<SpellDoerCastPipeline> spellFinished,
            Action notifyBrokeOnTimeout,
            SpellCast cast,
            SpellId prevSpell,
            OuterRef<IWizardEntity> wizard,
            ClientSpellRunner localRunner,
            AwaitableSpellDoerCast awaiter)
        {
            if (cast == null) throw new ArgumentNullException(nameof(cast));

            _taskRunner = taskRunner;
            _spellStarted = spellStarted;
            _spellFinished = spellFinished;
            _localWizard = wizard;
            _brokeOnTimeout = notifyBrokeOnTimeout;
            _localRunner = localRunner;
            _debug = debug;
            _awaiter = awaiter;
            if (Interlocked.CompareExchange(ref _state, (int)CastPipelineState.Casting, (int)CastPipelineState.None) != (int)CastPipelineState.None)
                throw new Exception($"You should not reuse SpellDoerCastPipeline (spell:{cast.Def.____GetDebugAddress()})");
            CastSpellInternal(cast, prevSpell);
        }

        private void CastSpellInternal(SpellCast cast, SpellId prevSpell)
        {
            if (_debug && Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("TRY_CAST:{0}",  cast.Def.____GetDebugAddress()).Write();

            _spellDef = cast.Def ?? throw new ArgumentNullException(nameof(cast.Def));

            if (_spellDef == Constants.WorldConstants.NullSpell)
            {
                MakeFinished(SpellId.Invalid);
                OnSpellFinished(SpellFinishReason.SucessOnTime);
                return;
            }
            Func<IEntitiesRepository, Task> lambda = (async repository =>
            {
                bool success = false;
                using (var w = await repository.Get(_localWizard))
                {
                    if (w == null)
                        _brokeOnTimeout();
                    var wizard = w?.Get<IWizardEntity>(_localWizard);
                    if (wizard == null)
                    {
                        Logger.IfError()?.Message($"{nameof(wizard)} == null").Write();
                        State = CastPipelineState.FailedToCastLocally;
                        _taskSpellGotId.SetResult(SpellId.Invalid);
                    }
                    else
                    {
                        var failedPredicates = new List<SpellPredicateDef>();
                        bool canCast = await wizard.CheckSpellCastPredicates(SyncTime.Now, cast, failedPredicates);
                        if (!canCast)
                        {
                            if (_debug && Logger.IsInfoEnabled) Logger.IfDebug()?.Message("FAIL_CAST: predicates failed on client [{0}]", cast.Def.____GetDebugAddress()).Write();
                            var fallbackSpell = failedPredicates.OfType<PredicateFallbackDef>().Select(x => x.Spell.Target).FirstOrDefault(x => x != null);
                            if (fallbackSpell != null)
                            {
                                if (_debug && Logger.IsInfoEnabled) Logger.IfDebug()?.Message("Try to cast fallback spell {0}", fallbackSpell.____GetDebugAddress()).Write();
                                cast.Def = fallbackSpell;
                                CastSpellInternal(cast, prevSpell);
                                return;
                            }
                            if (!_awaiter?.CastResultAwaiter.Task.IsCompleted ?? false)
                                _awaiter?.CastResultAwaiter.SetResult(default);
                            State = CastPipelineState.FailedToCastLocally;
                            _taskSpellGotId.SetResult(SpellId.Invalid);
                        }
                        else
                        {
                            if (wizard.SlaveWizardMark?.OnClient ?? false)
                                success = await CastFromClient(wizard, cast, prevSpell);
                            else
                                success = await CastFromServer(wizard, cast, prevSpell);
                        }
                    }
                }

                if (!success)
                {
                    FinishReason = FinishReasonType.Fail;
                    if (!_awaiter?.CastResultAwaiter.Task.IsCompleted ?? false)
                        _awaiter?.CastResultAwaiter.SetResult(default);
                    _taskSpellFinished.SetResult((Id, SpellFinishReason.FailOnStart));
                    OnSpellFinished(SpellFinishReason.FailOnStart);
                }
                else
                {
                    if (!_awaiter?.CastResultAwaiter.Task.IsCompleted ?? false)
                        _awaiter?.CastResultAwaiter.SetResult(Id);
                    _spellStarted(this);
                    if (_debug && Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("SUCCESSFUL_CAST:{0} Id:{1}",  cast.Def.____GetDebugAddress(), Id).Write();
                }

                _taskSpellCasted.SetResult(Id);
            });
            _taskRunner(lambda);
        }

        private async Task<bool> CastFromClient(IWizardEntity wizard, SpellCast cast, SpellId prevSpell)
        {
            var spellId = await wizard.NewSpellId();
            if (!spellId.IsGeneratedBySlave) throw new Exception($"Invalid spell id for client side spell {spellId}");
            cast.StartAt = SyncTime.Now;

            if (_localRunner != null)
            {
                var modifiers = await SpellModifiers.GetModifiersForSpell(cast, wizard.Owner, wizard.EntitiesRepository);
                if (!_localRunner.StartSpell(spellId, cast, prevSpell, modifiers))
                {
                    if (_debug && Logger.IsDebugEnabled)
                        Logger.IfDebug()?.Message("FAIL_CAST: blocked on client by local runner [{0}]", cast.Def.____GetDebugAddress()).Write();
                    State = CastPipelineState.FailedToCastLocally;
                    _taskSpellGotId.SetResult(SpellId.Invalid);
                    return false;
                }
            }

            Id = spellId;
            if (_awaiter != null)
                _awaiter.SpellId = Id;
            _taskSpellGotId.SetResult(Id);
            var remoteSpellId = await (prevSpell.IsValid ? wizard.CastSpell(cast, spellId, prevSpell) : wizard.CastSpell(cast, spellId));  // <<-- Main point
            if (remoteSpellId == SpellId.Invalid)
            {
                if (_debug && Logger.IsDebugEnabled)
                    Logger.IfDebug()?.Message("FAIL_CAST: cast failed on wizard [{0} Id:{1}]", cast.Def.____GetDebugAddress(), Id).Write();
                State = CastPipelineState.FailedToCastRemote;
                return false;
            }

            State = CastPipelineState.Casted;
            if (spellId != remoteSpellId) throw new Exception($"Invalid spell id for client side spell {spellId}");

            return true;
        }

        private async Task<bool> CastFromServer(IWizardEntity wizard, SpellCast cast, SpellId prevSpell)
        {
            Id = await (prevSpell.IsValid ? wizard.CastSpell(cast, SpellId.Invalid, prevSpell) : wizard.CastSpell(cast)); // <<-- Main point

            if (_awaiter != null)
                _awaiter.SpellId = Id;
            _taskSpellGotId.SetResult(Id);
            if (Id == SpellId.Invalid)
            {
                if (_debug && Logger.IsDebugEnabled)
                    Logger.IfDebug()?.Message("FAIL_CAST: cast failed on wizard [{0} Id:{1}]", cast.Def.____GetDebugAddress(), Id).Write();
                State = CastPipelineState.FailedToCastRemote;
                return false;
            }

            State = CastPipelineState.Casted;
            return true;
        }

        public void FinishCast(SpellFinishMethod method, FinishReasonType reason = FinishReasonType.Success)
        {
            switch (method)
            {
                case SpellFinishMethod.Stop:
                    StopCast(reason);
                    break;
                case SpellFinishMethod.Cancel:
                    CancelCast();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"method:{method}");
            }
        }

        public void StopCast(FinishReasonType reason = FinishReasonType.Success)
        {
            if (_debug && Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("TRY_STOP_CAST:{0} Id:{2} Reason:{1}",  _spellDef.____GetDebugAddress(), reason, Id).Write();
            if (Interlocked.Exchange(ref _stopped, 1) != 0)
            {
                if (_debug && Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("Spell:{0} Id:{1} Already stoped",  _spellDef.____GetDebugAddress(), Id).Write();
                return;
            }
            SpellFinishReason finishReason = reason == FinishReasonType.Fail ? SpellFinishReason.FailOnDemand : SpellFinishReason.SucessOnDemand;
            StopSpell(finishReason);
        }

        public void CancelCast()
        {
            _taskRunner(async repository =>
            {
                try
                {
                    await SpellGotIdTask;
                    if (_stopped == 0)
                    {
                        if (_debug && Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("TRY_CANCEL_CAST:{0} Id:{1}",  _spellDef.____GetDebugAddress(), Id).Write();
                        using (var w = await repository.Get(_localWizard))
                        {
                            var wiz = w.Get<IWizardEntity>(_localWizard);
                            if (wiz != null)
                                await wiz.CancelSpell(_id);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message($"FAIL_CANCEL:{Id} {SpellDef.____GetDebugAddress()}: {e}").Write();
                }
            });
        }

        private void StopSpell(SpellFinishReason finishReason)
        {
            if (_debug && Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("STOP_CAST:{0} Id:{1} Reason:{2}",  _spellDef.____GetDebugAddress(), Id, finishReason).Write();
            _taskRunner(async repository =>
            {
                if (_localRunner != null)
                {
                    await _taskSpellGotId.Task;
                    _localRunner.StopSpell(Id, finishReason);
                }

                await _taskSpellCasted.Task;

                State = CastPipelineState.Stopping;
                try
                {
                    using (var w = await repository.Get(_localWizard))
                    {
                        if (w == null)
                        {
                            _stopped = 0;
                            _brokeOnTimeout();
                        }
                        var wiz = w.Get<IWizardEntity>(_localWizard);
                        if (wiz != null)
                        {
                            if (!await wiz.StopCastSpell(_id, finishReason))
                            {
                                _stopped = 0;
                            }
                            else
                            {
                                State = CastPipelineState.Stopped;
                            }
                        }
                        else
                        {
                            _stopped = 0;
                        }

                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message($"FAIL_STOP:{Id} {SpellDef.____GetDebugAddress()}: {e}").Write();
                    _stopped = 0;
                }
                finally
                {
                    if (_stopped == 0)
                    {
                        _attemptsToStop++;
                        if (_attemptsToStop == 4)
                        {
                            Logger.IfError()?.Message($"Can't stop spell {Id} {SpellDef.____GetDebugAddress()}").Write();
                        }
                        _taskRunner(async _ =>
                        {
                            await Task.Delay(500);
                            FinishReasonType finishReasonType = FinishReasonType.None;
                            if (finishReason == SpellFinishReason.FailOnDemand)
                                finishReasonType = FinishReasonType.Fail;
                            else
                                finishReasonType = FinishReasonType.Success;
                            StopCast(finishReasonType);
                        });
                    }
                }
            });
        }

        private void OnSpellFinished(SpellFinishReason reason)
        {
            if (_localRunner != null && Id.IsValid)
                _localRunner.StopSpell(Id, reason);
            if (_awaiter != null)
            {
                _awaiter.FinishReason = reason;
                _awaiter.Finished?.Invoke(Id, reason);
            }
            _spellFinished(this);
        }

        public SpellDoerCastPipeline MakeFinished(SpellId id, FinishReasonType reason = FinishReasonType.Success)
        {
            Id = id;
            FinishReason = reason;
            State = FinishReason == FinishReasonType.Fail ? CastPipelineState.Failed : CastPipelineState.Succeded;
            _taskSpellGotId.SetResult(Id);
            _taskSpellCasted.SetResult(Id);
            _taskSpellFinished.SetResult((Id, SpellFinishReason.FailOnStart));
            _stopped = 1;
            return this;
        }

        public override string ToString()
        {
            return $"{Id} {State} {FinishReason}";
        }
    }


    public static class SpellDoerCastPipelineExtension
    {
        public static bool IsNotFinished(this ISpellDoerCastPipeline ppl)
        {
            return ppl != null && (ppl.State == CastPipelineState.None || ppl.State == CastPipelineState.Casted || ppl.State == CastPipelineState.Casting);
        }

        public static bool IsFinished(this ISpellDoerCastPipeline ppl)
        {
            return ppl == null || (ppl.State != CastPipelineState.None && ppl.State != CastPipelineState.Casted && ppl.State != CastPipelineState.Casting);
        }
    }
}