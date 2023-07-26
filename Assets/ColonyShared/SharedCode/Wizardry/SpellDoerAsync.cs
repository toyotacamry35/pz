using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NLog;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using ColonyShared.SharedCode.Utils;
using System.Threading.Tasks;
using System.Linq;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem.Delta;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using ColonyShared.SharedCode.Modifiers;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.Repositories;
using ReactivePropsNs.ThreadSafe;
using SharedCode.Serializers;

namespace SharedCode.Wizardry
{
    public class SpellDoerAsync : ISpellDoer
    {
        [NotNull] public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Action<SpellDoerCastPipeline> _onSpellStartedCb;
        private readonly Action<SpellDoerCastPipeline> _onSpellFinishedCb;
        private readonly Action _notifyBrokeOnTimeoutCb;
        private readonly Action<Func<IEntitiesRepository, Task>> _taskRunnerFn;
        private readonly List<SpellDoerCastPipeline> _pipelines = new List<SpellDoerCastPipeline>();
        private readonly DisposableComposite _disposables = new DisposableComposite();
        private IUnityEnvironmentMark _clientMark;
        private OuterRef<IWizardEntity> _wizard;
        private IEntitiesRepository _repository;
        private OuterRef<IEntity> _ownerRef;
        private bool _isPlayer;
        private (OuterRef<IWizardEntityClientBroadcast> Ref, IEntitiesRepository Repo) _wizardSubscribedTo;
        private ClientSpellRunner _predictionRunner;

        public SpellDoerAsync()
        {
            _onSpellStartedCb = SpellStarted;
            _onSpellFinishedCb = SpellFinished;
            _notifyBrokeOnTimeoutCb = NotifyBrokeOnTimeout;
            _taskRunnerFn = RunAsyncTask;
        }

        public bool IsActive => _wizard.IsValid;
        public IEntitiesRepository Repository => _repository;

        public event SpellDoerStartDelegate OnOrderStarted;
        public event SpellDoerFinsihDelegate OnOrderFinished;
        public event Action BrokeOnTimeout;

        public void Activate(OuterRef<IWizardEntity> wizardRef, OuterRef<IEntity> ownerRef, IEntitiesRepository repository, IUnityEnvironmentMark clientMark, bool isPlayer)
        {
            Logger.IfDebug()
                ?.Message(ownerRef.Guid, $"Create Spell Doer | SlaveWizard{wizardRef} Repo:{repository} Mark:{clientMark} isPlayer:{isPlayer}")
                .Write();
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _ownerRef = ownerRef.IsValid ? ownerRef : throw new ArgumentException($"!{nameof(ownerRef)}.{nameof(ownerRef.IsValid)}");
            _clientMark = clientMark ?? throw new ArgumentNullException(nameof(clientMark));
            _isPlayer = isPlayer;
            _wizard = wizardRef.IsValid ? wizardRef : throw new ArgumentException($"!{nameof(wizardRef)}.{nameof(wizardRef.IsValid)}");
            _clientMark.HasClientAuthorityStream.Action(_disposables, hasAuthority => { if (hasAuthority) CreatePredictionRunner(); else DestroyPredictionRunner(); });
        }

        public void Deactivate()
        {
            Logger.IfDebug()
                ?.Message(_ownerRef.Guid, $"Destroy Spell Doer | SlaveWizard:{_wizard} Repo:{_repository} Mark:{_clientMark} isPlayer:{_isPlayer}")
                .Write();
            _disposables.Clear();
            _wizard = default;
            _repository = null;
            DestroyPredictionRunner();
        }

        public async Task SubscribeToWizard(OuterRef<IWizardEntityClientBroadcast> wizardRef, IEntitiesRepository repository)
        {
            _wizardSubscribedTo = (wizardRef, repository);
            using (var w = await repository.Get(wizardRef))
            {
                var wizard = w.Get(wizardRef, ReplicationLevel.ClientBroadcast);
                if (wizard == null)
                    throw new Exception($"Can't get wizard {wizardRef}");
                if (wizard.Spells == null)
                    throw new Exception($"Invalid wizard {wizardRef}");
                wizard.Spells.OnItemRemoved += Wizard_SpellFinished;
            }
        }

        public async Task UnsubscribeFromWizard()
        {
            if (_wizardSubscribedTo.Ref.IsValid)
                using (var w = await _wizardSubscribedTo.Repo.Get(_wizardSubscribedTo.Ref))
                {
                    var wizard = w.Get(_wizardSubscribedTo.Ref, ReplicationLevel.ClientBroadcast);
                    if (wizard != null)
                    {
                        foreach (var spell in wizard.Spells)
                            await Wizard_SpellFinishedInternal(spell.Value);
                        wizard.Spells.OnItemRemoved -= Wizard_SpellFinished;
                    }
                }
            _wizardSubscribedTo = default;
        }

        private void CreatePredictionRunner()
        {
            if (!IsActive) throw new InvalidOperationException("Not active");

            if (_predictionRunner == null)
                _predictionRunner = new ClientSpellRunner(_wizard, _ownerRef, _clientMark, _repository);
        }


        private void DestroyPredictionRunner()
        {
            _predictionRunner = null; // просто не запускаем новых спеллов, не останавливая текущие
        }

        private async Task Wizard_SpellFinished(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientBroadcast> eventArgs)
        {
            var removedSpell = eventArgs.Value;
            await Wizard_SpellFinishedInternal(removedSpell);
        }

        private Task Wizard_SpellFinishedInternal(ISpellClientBroadcast removedSpell)
        {
            var reason = removedSpell.FinishReason;
            var id = removedSpell.Id;
            lock (_pipelines)
                for (int i = _pipelines.Count - 1; i >= 0; --i)
                    _pipelines[i].SpellFinished(id, reason);
            return Task.CompletedTask;
        }

        public ISpellDoerCastPipeline DoCast(SpellCast order, AwaitableSpellDoerCast awaiter = default)
        {
            return DoCast(new SpellDoerCastPipeline(), order, awaiter);
        }

        public ISpellDoerCastPipeline DoCast(SpellCastBuilder spellBuilder, AwaitableSpellDoerCast awaiter = default)
        {
            return DoCast(new SpellDoerCastPipeline(), spellBuilder.Build(), awaiter);
        }

        private SpellDoerCastPipeline DoCast(SpellDoerCastPipeline pipeline, SpellCast order, AwaitableSpellDoerCast awaiter)
        {
            return DoCastInternal(pipeline, order, SpellId.Invalid, awaiter);
        }

        public ISpellDoerCastPipeline DoCastChain(SpellCast order, SpellId afterSpell, AwaitableSpellDoerCast awaiter = default)
        {
            return DoCastChain(new SpellDoerCastPipeline(), order, afterSpell, awaiter);
        }

        public ISpellDoerCastPipeline DoCastChain(SpellCastBuilder spellBuilder, SpellId afterSpell, AwaitableSpellDoerCast awaiter = default)
        {
            return DoCastChain(new SpellDoerCastPipeline(), spellBuilder.Build(), afterSpell, awaiter);
        }

        private ISpellDoerCastPipeline DoCastChain(SpellDoerCastPipeline pipeline, SpellCast order, SpellId prevSpell, AwaitableSpellDoerCast awaiter)
        {
            return DoCastInternal(pipeline, order, prevSpell, awaiter);
        }

        private SpellDoerCastPipeline DoCastInternal(SpellDoerCastPipeline cast, SpellCast order, SpellId afterSpell, AwaitableSpellDoerCast awaiter)
        {
            if (!IsActive)
                return cast.MakeFinished(SpellId.Invalid);

            //Logger.IfInfo()?.Message($"SpellDoer {order.Def.Name} {SyncTime.GlobalClockwatchNow}").Write();
            if (_isPlayer && (_clientMark == null || !_clientMark.HasClientAuthority))
            {
                Logger.IfError()?.Message($"Casting spell from client WITHOUT authority | Cast:{order}").Write();
                return cast.MakeFinished(SpellId.Invalid);
            }

            lock (_pipelines)
                _pipelines.Add(cast);
            cast.CastSpell(
                _taskRunnerFn,
                _isPlayer,
                _onSpellStartedCb,
                _onSpellFinishedCb,
                _notifyBrokeOnTimeoutCb,
                order,
                afterSpell,
                _wizard,
                _predictionRunner,
                awaiter);
            // We return here valid OrderId no matter, what happened inside `.RunAsynTask(..)` (e.g. spell couldn't be casted). And outer code should do all work on subscriptions of cluster-spell true casting
            return cast;
        }

        public ISpellDoerCastPipeline GetPipeline(SpellId spellId)
        {
            if (!spellId.IsValid)
                return null;
            lock (_pipelines)
                return _pipelines.FirstOrDefault(x => x.Id == spellId);
        }

        private void SpellStarted(ISpellDoerCastPipeline pipeline)
        {
            OnOrderStarted?.Invoke(pipeline);
        }

        private void SpellFinished(SpellDoerCastPipeline pipeline)
        {
            OnOrderFinished?.Invoke(pipeline);
            lock (_pipelines)
                _pipelines.Remove(pipeline);
        }

        public async Task<bool> CanStartCast(SpellCast cast, PredicateIgnoreGroupDef predicateIgnoreGroupDef = null, bool checkPredicatesOnly = false)
        {
            if (!IsActive)
                return false;
            using (var w = await _repository.Get(_wizard))
            {
                var wizard = w.Get(_wizard, ReplicationLevel.Master);
                if (!checkPredicatesOnly)
                {
                    if (_predictionRunner != null)
                    {
                        if (_predictionRunner.HasBlockersOfCast(cast, SpellId.Invalid))
                            return false;
                    }
                    else
                    {
                        if (await wizard.HasSpellsPreventingThisFromStart(cast))
                            return false;
                    }
                }

                return await wizard.CheckSpellCastPredicates(SyncTime.Now, cast, null);
            }
        }

        public bool StopCast(SpellId id, FinishReasonType reason)
        {
            if (!IsActive)
                return false;

            SpellFinishReason finishReason = SpellFinishReason.None;
            if (reason == FinishReasonType.Fail)
                finishReason = SpellFinishReason.FailOnDemand;
            else
                finishReason = SpellFinishReason.SucessOnDemand;
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var w = await _repository.Get(_wizard))
                {
                    if (w == null)
                        NotifyBrokeOnTimeout();
                    var ent = w?.Get(_wizard, ReplicationLevel.Master);
                    if (ent == null)
                    {
                        Logger.IfError()?.Message("SpellDoerAsync StopCast null entity id {0}", _wizard).Write();
                        return;
                    }
                    await ent.StopCastSpell(id, finishReason);
                }
            }, _repository);
            return true;
        }

        //это всё необходимо заменить на соответствующие вызовы WizardEntity из соответствующих эффектов/импактов
        public void StopAllSpellsOfGroup([NotNull] SpellGroupDef group, SpellId orderId, FinishReasonType reason)
        {
            if (!IsActive)
                return;

            if (_isPlayer)
                Logger.IfDebug()?.Message($"#Dbg: StopAllSpellsOfGroup {group}, {orderId}, {reason}").Write();

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cnt = await _repository.Get(_wizard))
                {
                    if (cnt != null)
                    {
                        var w = cnt.Get(_wizard, ReplicationLevel.Master);
                        if (w != null)
                            await w.StopAllSpellsOfGroup(group, orderId, reason == FinishReasonType.Success ? SpellFinishReason.SucessOnDemand : SpellFinishReason.FailOnDemand);
                        else
                            NotifyBrokeOnTimeout();
                    }
                    else
                        NotifyBrokeOnTimeout();
                }
            }, _repository);
        }

        private void NotifyBrokeOnTimeout()
        {
            if (_isPlayer)
                Logger.IfError()?.Message("FAIL_CAST: broke on timeout").Write();
            BrokeOnTimeout?.Invoke();
        }

        private void RunAsyncTask(Func<IEntitiesRepository, Task> task)
        {
            if (!IsActive)
                return;

            AsyncUtils.RunAsyncTask(() => task(_repository), _repository);
        }
    }


    public class SpellCastStatus
    {
        public SpellCastStatus() { }
        public SpellCastStatus(SpellCastStatus s)
        {
            StartTimeStamp = s.StartTimeStamp;
            SpellDesc = s.SpellDesc;
            FinishReason = s.FinishReason;
            EndTimeStamp = s.EndTimeStamp;
        }
        public long StartTimeStamp;
        public long EndTimeStamp = long.MaxValue;
        public ISpellCast SpellDesc;
        public SpellFinishReason FinishReason;
        public long RemoveAt = long.MaxValue;
    }
}
