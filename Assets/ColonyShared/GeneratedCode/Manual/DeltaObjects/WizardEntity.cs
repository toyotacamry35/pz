using System;
using System.Linq;
using System.Threading.Tasks;
using SharedCode.Wizardry;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Utils.Extensions;
using SharedCode.EntitySystem;
using System.Diagnostics;
using Assets.ColonyShared.SharedCode.Utils;
using NLog;
using System.Collections.Generic;
using SharedCode.EntitySystem.Delta;
using JetBrains.Annotations;
using GeneratedCode.EntitySystem;
using ResourcesSystem.Loader;
using GeneratedCode.Network.Statistic;
using SharedCode.Serializers;
using System.Collections.Concurrent;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.Modifiers;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;
using SharedCode.Utils.DebugCollector;
using Shared.ManualDefsForSpells;
using SharedCode.Aspects.Item.Templates;
using Core.Cheats;
using GeneratedCode.Manual.Repositories;
using ResourceSystem.Aspects;
using Assets.Src.Arithmetic;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class WizardEntity : IWizardEntityImplementRemoteMethods, TimelineRunner.IWizard, IHookOnInit, IHookOnStart, IHookOnDestroy
    {
        public const string WatchdogConfig = "/Config/WizardWatchdog";

        protected bool IsSlaveEntity => HostWizard.IsValid;

        [NotNull] internal static readonly NLog.Logger CommonLogger = LogManager.GetCurrentClassLogger();
        [NotNull] internal static readonly NLog.Logger _Logger = LogManager.GetCurrentClassLogger();

        private TimelineRunner.LoggerDelegate[] _loggers;
        private readonly ConcurrentQueue<string> _loggedStuff = new ConcurrentQueue<string>();
        static WizardWatchdogDef Watchdog;
        //        ChainCancellationToken _watchdogToken;

        public string WhoAmI { get; private set; }

        private TimelineRunner.LoggerDelegate Logger() => Logger(LogLevel.Debug);

        private TimelineRunner.LoggerDelegate Logger(LogLevel level)
            => IsLoggerEnabled(level) ? _loggers[level.Ordinal] : null;

        private bool IsLoggerEnabled(LogLevel level)
            => _Logger.IsEnabled(level) && (level >= LogLevel.Warn || IsInterestingEnoughToLog);

        public Task OnInit()
        {
            WhoAmI = "Wizard Host";
            Counter = SpellId.FirstMasterValid;
            SlaveCounter = SpellId.FirstSlaveValid;
            Spells = new DeltaDictionary<SpellId, ISpell>();
            _spellWordFilter = GetSpellWordExecutionMask;
            if (Watchdog == null)
                Watchdog = GameResourcesHolder.Instance.LoadResource<WizardWatchdogDef>(WatchdogConfig);
            //_watchdogToken = this.Chain().Delay(10, true, true).WatchdogUpdate().Run();
            _loggers = new TimelineRunner.LoggerDelegate[LogLevel.AllLoggingLevels.Max(x => x.Ordinal) + 1];
            foreach (var level in LogLevel.AllLoggingLevels)
            {
                var l = level;
                _loggers[level.Ordinal] = (m, p) => _PutInLog(l, m, p);
            }
            return Task.CompletedTask;
        }

        public Task OnStart()
        {
            if (SlaveWizardMark == null)
                return SubscribeToMortal();
            return Task.CompletedTask;
        }


        private readonly TimelineRunner _timeLine = new TimelineRunner();
        private TimelineHelpers.WordExecutionMaskDelegate _spellWordFilter;

        //----------------------------------------------------CAST SPELL BEGIN-------------------------------------------------------------------------------------------

        ISpellStatus CreateNewStatus([NotNull] SpellDef spell)
        {
            var status = new SpellStatus();
            status.Spell = spell ?? throw new ArgumentNullException(nameof(spell));
            if (spell.SubSpells.Length > 0)
                status.SubSpells = new DeltaList<ISpellStatus>();
            foreach (var subSpell in spell.SubSpells)
            {
                var subStatus = CreateNewStatus(subSpell.Spell);
                subStatus.SubSpell = subSpell;
                status.SubSpells.Add(subStatus);
            }
            return status;
        }
        //        void PutInLog(string log) => PutInLog(LogLevel.Debug, log, null);
        //
        //        void PutInLog(string log, params object[] format) => PutInLog(LogLevel.Debug, log, format);
        //
        //        void PutInLog(LogLevel level, string log) => PutInLog(level, log, null);

        private void _PutInLog(LogLevel level, string log, params object[] prms)
        {
            if (_Logger.IsTraceEnabled || level >= LogLevel.Error)
            {
                if (prms != null && prms.Length > 0)
                    log = string.Format(log, prms);
                _Logger.If(level)?.Message(new LogEventInfo { Message = StringBuildersPool.Get.Append(WhoAmI).Append(" | ").Append(log).ToStringAndReturn(), Level = level, LoggerName = _Logger.Name })
                    .Write();
                if (IsInterestingEnoughToLog)
                {

                    _loggedStuff.Enqueue(log);
                    if (_loggedStuff.Count > Watchdog.LogHistoryCount)
                        _loggedStuff.TryDequeue(out _);
                    //    _loggedStuff.TryDequeue(out _);
                }
            }
        }

        public Task<SpellId> NewSpellIdImpl()
        {
            return Task.FromResult(NewSpellIdInternal());
        }

        private SpellId NewSpellIdInternal()
        {
            var id = Counter.Next();
            Counter = id;
            return id;
        }

        public Task<SpellId> CastSpellImpl(SpellCast spellCast)
        {
            return CastSpellImpl(spellCast, SpellId.Invalid);
        }

        public Task<SpellId> CastSpellImpl(SpellCast spellCast, SpellId clientSpellId)
        {
            return CastSpellImpl(spellCast, clientSpellId, SpellId.Invalid);
        }
        public async Task TryPlaceCooldown(bool begin, SpellDef def)
        {
            if (def.Cooldown == null || IsSlaveEntity)
                return;
            if (def.Cooldown.Target.FromEnd && !begin)
            {
                var cds = await def.Cooldown.Target.Cooldown.Target.CalcAsync(Owner, EntitiesRepository);
                CooldownsUntil[def.Cooldown.Target.Group] = SyncTime.Now +
                    SyncTime.FromSeconds(cds);
                DelayRemoveCooldown(cds, CooldownsUntil[def.Cooldown.Target.Group], def.Cooldown.Target.Group);
            }
            else if (!def.Cooldown.Target.FromEnd && begin)
            {
                var cds = await def.Cooldown.Target.Cooldown.Target.CalcAsync(Owner, EntitiesRepository);
                CooldownsUntil[def.Cooldown.Target.Group] = SyncTime.Now +
                    SyncTime.FromSeconds(cds);
                DelayRemoveCooldown(cds, CooldownsUntil[def.Cooldown.Target.Group], def.Cooldown.Target.Group);
            }
        }

        public void DelayRemoveCooldown(float delay, long cooldownShouldEndAt, CooldownGroupDef groupDef)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                if (!((IEntityExt)this).IsMaster())
                    return;
                if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(TypeId, Id) == RepositoryEntityContainsStatus.Master)
                    using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(TypeId, Id))
                    {
                        if (CooldownsUntil.TryGetValue(groupDef, out var val))
                        {
                            if (val == cooldownShouldEndAt)
                                CooldownsUntil.Remove(groupDef);
                        }
                    }
            }, EntitiesRepository);

        }
        public async Task<bool> IsOnCooldown(SpellDef def)
        {
            if (def.Cooldown == null)
                return false;
            var now = SyncTime.Now;
            if (CooldownsUntil.TryGetValue(def.Cooldown.Target.Group, out var cooldown))
            {
                if (cooldown > now)
                    return true;
            }
            return false;
        }
        public async Task<SpellId> CastSpellImpl(SpellCast spellCast, SpellId clientSpellId, SpellId prevSpellId)
        {
            if (spellCast.Def == null) throw new ArgumentNullException(nameof(spellCast.Def));

            Logger()?.Invoke("CastSpell | {0}{1}{2}", spellCast, clientSpellId.IsValid ? $" clientSpellId:{clientSpellId}" : string.Empty, prevSpellId.IsValid ? $" prevSpellId:{prevSpellId}" : string.Empty);

            Statistics<SpellCastStatistics>.Instance.Casted(spellCast.Def);

            var stopwatch = StartStopwatch();
            try
            {

                if (IsDead && !spellCast.Def.IgnoresDeath)
                {
                    Logger()?.Invoke("FAIL_CAST: I'm dead, , {0}", spellCast);
                    return SpellId.Invalid;
                }

                if (IsSlaveEntity)
                {
                    return await CastSpellFromSlaveToHost(spellCast, clientSpellId, prevSpellId);
                }

                Logger(LogLevel.Trace)?.Invoke("Spells on cast: [{0}]", spellCast, string.Join(", ", Spells.Select(x => x.Value.CastData.Def.____GetDebugAddress())));

                if (spellCast.Def.ClearsSlot)
                    foreach (var spell in Spells)
                        if ((spell.Value.CastData.Def.Slot != null && spell.Value.CastData.Def.Slot == spellCast.Def.Slot) || (spell.Value.CastData.Def == spellCast.Def))
                            await StopCastSpellImpl(spell.Value.Id);

                if (prevSpellId.IsValid)
                    foreach (var spell in Spells)
                        if (spell.Value.Id == prevSpellId)
                            await StopCastSpellImpl(prevSpellId);

                if (HasSpellsPreventingThisFromStartInternal(spellCast, prevSpellId))
                {
                    Logger()?.Invoke("FAIL_CAST: has spells preventing {0} to start: [{1}]", spellCast, string.Join(", ", GetSpellsPreventingThisFromStartInternal(spellCast, prevSpellId).Select(x => $"{x.CastData.Def.____GetDebugRootName()}")));
                    return SpellId.Invalid;
                }

                // if (!await SpellSelectionMatchesSelector(spellCast))
                // {
                //     Logger()?.Invoke("FAIL_CAST: spell {0} was blocked by selection match", spellCast);
                //     return SpellId.Invalid;
                // }

                var currentTime = SyncTime.Now;

                Statistics<SpellCastStatistics>.Instance.Used(spellCast.Def);
                SpellId id;
                if (!clientSpellId.IsValid)
                {
                    id = NewSpellIdInternal();
                    if (id.IsGeneratedBySlave)
                    {
                        Logger(LogLevel.Error)?.Invoke("Spell id:{0} is not a master generated spell id", id);
                        return SpellId.Invalid;
                    }

                    spellCast.StartAt = currentTime;
                }
                else
                {
                    id = clientSpellId;
                    if (!id.IsGeneratedBySlave)
                    {
                        Logger(LogLevel.Error)?.Invoke("Provided spell id:{0} is not a slave generated spell id", id);
                        return SpellId.Invalid;
                    }

                    if (id == SlaveCounter)
                        Logger(LogLevel.Error)?.Invoke("Provided spell id:{0} is already in use", id);
                    if (id.PureCounter > SlaveCounter.PureCounter)
                        SlaveCounter = id; // запоминаем, последний сгенерённый клиентом идентификатор, чтобы отдать его при следующей инициализации клиента
                    if (SyncTime.InTheFuture(spellCast.StartAt, currentTime))
                    {
                        if (SyncTime.InTheFuture(spellCast.StartAt, currentTime + 1000))
                            Logger(LogLevel.Warn)?.Invoke("Spell from future | SpellId:{0} StartAt:{1} TimeDelta:{2}", id, spellCast.StartAt, spellCast.StartAt - currentTime);
                        spellCast.StartAt = currentTime;
                    }
                }

                var newSpell = (ISpell)new Spell
                {
                    CastData = spellCast,
                    Id = id,
                    Status = CreateNewStatus(spellCast.Def),
                };

                if (spellCast.TryGetParameter<SpellCastParameterCauser>(out var causerParam))
                    newSpell.Causer = causerParam.Causer;
                
                await SpellModifiers.GetModifiersForSpell(spellCast, Owner, EntitiesRepository, newSpell.Modifiers);

                _timeLine.PrepareSpell(newSpell, _spellWordFilter);
                var ents = newSpell.CastData.GetAllImportantEntities();

                List<SpellPredicateDef> failedPredicates = Logger() != null ? new List<SpellPredicateDef>() : null;
                var hasEntities = SubscribeOnSpellImportantEntities(ents);
                var predicatesTrue = await CheckPredicatesByData(SyncTime.Now, spellCast.Def, spellCast, id, failedPredicates, newSpell.Modifiers);
                if (!hasEntities || !predicatesTrue)
                {
                    Logger()?.Invoke("FAIL_CAST: spellCast:{2}, spellId:{3}, noEntities:{0}, predicatesFailed:[{1}]", !hasEntities, failedPredicates != null ? string.Join(", ", failedPredicates.Select(x => x.____GetDebugShortName())) : predicatesTrue.ToString(), spellCast, id);
                    return SpellId.Invalid;
                }

                bool started = TryFinallyStartSpell(newSpell, prevSpellId);
                if (!started)
                {
                    Logger()?.Invoke("FAIL_CAST: failed to finally start {0} {1}", spellCast, id);
                    return SpellId.Invalid;
                }
                else
                    EventsForSauron.PostEvent(Owner, null, newSpell.CastData.Def);
                await TryPlaceCooldown(true, newSpell.SpellDef);

                if (clientSpellId.IsValid)
                {
                    for (var i = SpellsThatMustBeStoppedAtStart.Count - 1; i >= 0; --i)
                    {
                        var tuple = SpellsThatMustBeStoppedAtStart[i];
                        if (tuple.SpellId == clientSpellId)
                        {
                            Logger()?.Invoke("FAIL_CAST: Spell was stopped before start {0}", clientSpellId);
                            SpellsThatMustBeStoppedAtStart.RemoveAt(i);
                            StopCastSpellInternal(newSpell, tuple.Reason);
                        }
                        else if (SyncTime.InThePast(tuple.ExpiredAt))
                        {
                            SpellsThatMustBeStoppedAtStart.RemoveAt(i);
                        }
                    }
                }

                CastExtraSpells(newSpell, currentTime);

                return id;
            }
            finally
            {
                StopStopwatchStart(ref stopwatch, spellCast);
            }
        }

        private void CastExtraSpells(ISpell newSpell, long startTime)
        {
            var extraSpells = newSpell.Modifiers.GetCastExtraSpellModifier().GetEnumerator();
            if (extraSpells.MoveNext())
                AsyncUtils.RunAsyncTask(
                    async () =>
                    {
                        using (await this.GetThis())
                        {
                            do
                            {
                                var sc = new SpellCastWithParameters
                                {
                                    Def = extraSpells.Current,
                                    StartAt = startTime,
                                    Parameters = newSpell.CastData.GetParameters(),
                                    Context = newSpell.CastData.Context
                                };
                                await this.CastSpell(sc);
                            } while (extraSpells.MoveNext());
                        }
                    });
        }

        private bool IsStopwatchEnabled => IsLoggerEnabled(LogLevel.Warn);

        private bool IsStopwatchWordEnabled => IsLoggerEnabled(LogLevel.Error);

        Stopwatch StartStopwatchWord()
        {
            if (IsStopwatchWordEnabled)
                return StopwatchPool.GetStarted;
            return null;
        }

        Stopwatch StartStopwatch()
        {
            if (IsStopwatchEnabled)
                return StopwatchPool.GetStarted;
            return null;
        }

        void StopStopwatchStart(ref Stopwatch stopwatch, ISpellCast cast)
        {
            if (stopwatch != null)
            {
                var elapsed = stopwatch.StopAndRelease();
                stopwatch = null;
                if (elapsed > Watchdog.MillisecondsTooMuchForStart)
                    Logger(LogLevel.Warn)?.Invoke("Start took too long {0} {1} ms", cast.Def.____GetDebugAddress(), elapsed);
            }
        }

        void StopStopwatchUpdate(ref Stopwatch stopwatch, ISpellCast cast)
        {
            if (stopwatch != null)
            {
                var elapsed = stopwatch.StopAndRelease();
                stopwatch = null;
                if (elapsed > Watchdog.MillisecondsTooMuchForStart)
                    Logger(LogLevel.Warn)?.Invoke("Update took too long {0} {1} ms", cast.Def.____GetDebugAddress(), elapsed);
            }
        }

        void StopStopwatchPredicate(ref Stopwatch stopwatch, SpellPredicateDef predicate)
        {
            if (stopwatch != null)
            {
                var elapsed = stopwatch.StopAndRelease();
                stopwatch = null;
                if (elapsed > Watchdog.MillisecondsCriticalForStart)
                    Logger(LogLevel.Error)?.Invoke($"Predicate checks takes critical long | WhoAmI:{WhoAmI} Word:{predicate.____GetDebugAddress()} Elapsed:{elapsed}");
                else
                if (elapsed > Watchdog.MillisecondsTooMuchForStart)
                    Logger(LogLevel.Warn)?.Invoke($"Predicate check takes too long | WhoAmI:{WhoAmI} Word:{predicate.____GetDebugAddress()} Elapsed:{elapsed}");
            }
        }

        void StopStopwatchWord(ref Stopwatch stopwatch, SpellWordDef word, string operation)
        {
            if (stopwatch != null)
            {
                var elapsed = stopwatch.StopAndRelease();
                stopwatch = null;
                if (elapsed > Watchdog.MillisecondsCriticalForStart)
                    Logger(LogLevel.Error)?.Invoke($"Spell word takes critical long | WhoAmI:{WhoAmI} Word:{word.____GetDebugAddress()} Elapsed:{elapsed} Operation:{operation}");
                else
                if (elapsed > Watchdog.MillisecondsTooMuchForStart)
                    Logger(LogLevel.Warn)?.Invoke($"Spell word takes too long | WhoAmI:{WhoAmI} Word:{word.____GetDebugAddress()} Elapsed:{elapsed} Operation:{operation}");
            }
        }

        async Task<SpellId> CastSpellFromSlaveToHost(SpellCast spellCast, SpellId clientSpellId, SpellId prevSpellId)
        {
            Logger(LogLevel.Trace)?.Invoke("Spells on cast: [{0}]", spellCast, string.Join(", ", Spells.Select(x => x.Value.CastData.Def.____GetDebugAddress())));

            if (HasSpellsPreventingThisFromStartInternal(spellCast, prevSpellId))
            {
                Logger()?.Invoke("FAIL_CAST: has spells preventing {0} to start: [{1}]", spellCast, string.Join(", ", GetSpellsPreventingThisFromStartInternal(spellCast, prevSpellId).Select(x => $"{x.CastData.Def.____GetDebugRootName()}")));
                return SpellId.Invalid;
            }
            using (var host = await EntitiesRepository.Get<IWizardEntityClientFull>(HostWizard.Guid, spellCast))
            {
                //Logger.IfDebug()?.Message($"CastSpellFromSlaveToHost: {EntitiesRepository.CloudNodeType}").Write();
                var hostWizard = host?.Get<IWizardEntityClientFull>(HostWizard.Guid);
                if (hostWizard == null)
                {
                    Logger()?.Invoke("FAIL_CAST: no host wizard");
                    return SpellId.Invalid;
                }
                return await hostWizard.CastSpell(spellCast, clientSpellId, prevSpellId);
            }
        }

        ValueTask<bool> SpellSelectionMatchesSelector(ISpellCast spellCast)
        {
            //            var withSelector = spellCast as IWithSelector;
            //            if (withSelector == null)
            //                return true;
            //            if (withSelector.SpellSelector == SpellSelectorType.None)
            //                return true;
            //            var spellSelector = SpellSelectorFactory.Create(withSelector.SpellSelector);
            //            var checkDef = await spellSelector.Select(Owner.Guid, EntitiesRepository);
            //            return checkDef == spellCast.Def;
            return new ValueTask<bool>(true);
        }

        bool TryFinallyStartSpell(ISpell newSpell, SpellId prevSpellId)
        {
            if (!CheckSpellId(newSpell.Id, newSpell.CastData))
                return false;

            if (HasSpellsPreventingThisFromStartInternal(newSpell.CastData, prevSpellId))
            {
                Logger()?.Invoke("FAIL_CAST: has spells preventing {0} to start finally: [{1}]", newSpell.CastData, string.Join(", ", GetSpellsPreventingThisFromStartInternal(newSpell.CastData, prevSpellId).Select(x => $"{x.CastData.Def.____GetDebugRootName()}")));
                return false;
            }

            //            if (_watchdogToken == null)
            //            {
            //                //_watchdogToken = this.Chain().Delay(10, true, true).WatchdogUpdate().Run();
            //            }

            Logger(LogLevel.Trace)?.Invoke("StartSpell | SpellId:{1} {0}", newSpell.CastData, newSpell.Id);
            Collect.IfActive?.EventBgn($"{WhoAmI}.{newSpell.CastData.Def.____GetDebugRootName()}", Owner, (this, newSpell.Id));

            Spells.Add(newSpell.Id, newSpell);
            newSpell.Started = SyncTime.Now;
            DelayUpdate(0, newSpell.Id, newSpell.CastData);
            return true;
        }

        public Task<bool> HasSpellsPreventingThisFromStartImpl(SpellCast spell)
        {
            return Task.FromResult(HasSpellsPreventingThisFromStartInternal(spell, SpellId.Invalid));
        }


        private bool HasSpellsPreventingThisFromStartInternal(ISpellCast spell, SpellId ignoreSpell)
        {
            return Spells.HelperAny(x => x.Value.ItBlocksCastOf(spell.Def, ignoreSpell));
        }

        private IEnumerable<ISpell> GetSpellsPreventingThisFromStartInternal(ISpellCast spell, SpellId ignoreSpell)
        {
            return Spells.Where(x => x.Value.ItBlocksCastOf(spell.Def, ignoreSpell)).Select(x => x.Value);
        }

        private bool SubscribeOnSpellImportantEntities(IEnumerable<OuterRef<IEntity>> ents)
        {
            return EntitiesRepository.SubscribeOnDestroyOrUnload(ents.Select(v => new EntityDesc(v.Guid, v.TypeId)), false, EntitiesRepository_EntityDestroy);
        }

        private void UnsubscribeOnSpellImportantEntities(IEnumerable<OuterRef<IEntity>> ents)
        {
            foreach (var ent in ents)
            {
                EntitiesRepository.UnsubscribeOnDestroyOrUnload(ent.TypeId, ent.Guid, EntitiesRepository_EntityDestroy);
            }
        }
        //----------------------------------------------------CAST SPELL END-------------------------------------------------------------------------------------------

        //----------------------------------------------------STOP CAST SPELL BEGIN-------------------------------------------------------------------------------------------
        public async Task<bool> StopCastSpellImpl(SpellId spell)
        {
            return await StopCastSpellImpl(spell, SpellFinishReason.FailOnDemand);
        }
        async Task<bool> StopCallFromSlaveToHost(SpellId spell, SpellFinishReason reason)
        {
            if (IsSlaveEntity && SlaveWizardMark.OnServer)//this is dangerous as any vasyastalker666 can hack this, but there is no way to make this work on host for now
            {
                using (var host = await EntitiesRepository.Get<IWizardEntityClientFull>(HostWizard.Guid, spell))
                {
                    var entity = host?.Get<IWizardEntityClientFull>(HostWizard.Guid);
                    if (entity == null)
                    {
                        Logger()?.Invoke("IWizardEntity StopCastSpellImpl null entity id {0}", HostWizard.Guid);
                        return true;
                    }

                    return await entity.StopCastSpell(spell, reason);
                }
            }
            else if (IsSlaveEntity)
            {
                using (var host = await EntitiesRepository.Get<IWizardEntityClientFull>(HostWizard.Guid, spell))
                {
                    var entity = host.Get<IWizardEntityClientFull>(HostWizard.Guid);
                    if (entity != null)
                        return await entity.StopCastSpell(spell, reason);
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> StopCastSpellImpl(SpellId spell, SpellFinishReason reason)
        {
            Logger()?.Invoke("StopCast {0} Reason:{1}", spell, reason);
            if (IsSlaveEntity)
                return await StopCallFromSlaveToHost(spell, reason);

            Spells.TryGetValue(spell, out var s);
            if (s != null)
            {
                StopCastSpellInternal(s, reason);
            }
            else if (spell.IsGeneratedBySlave)
            {
                if (SpellsThatMustBeStoppedAtStart.All(x => x.SpellId != spell))
                    SpellsThatMustBeStoppedAtStart.Add(new SpellThatMustBeStoppedAtStart { SpellId = spell, ExpiredAt = SyncTime.Now + 10000, Reason = reason });
            }

            return true;
        }

        private void StopCastSpellInternal(ISpell s, SpellFinishReason reason)
        {
            if (s != null && !s.IsFinished() && s.CastData.Def.CanBeFinished)
            {
                Logger()?.Invoke("StopCast internal {0} Reason:{1}", s, reason);
                s.StopCast = SyncTime.Now;
                s.StopCastWithReason = reason;
                DelayUpdate(0, s.Id, s.CastData);
                //this.Chain().Update(spell).Run();
            }
        }
        //----------------------------------------------------STOP CAST SPELL END-------------------------------------------------------------------------------------------

        public async Task CancelSpellImpl(SpellId spellId)
        {
            Logger()?.Invoke("[{1}] CancelSpell {0}", spellId, WhoAmI);
            if (!IsSlaveEntity)
            {
                CanceledSpells.TryAdd(spellId, true);
            }
            else
            {
                using (var host = await EntitiesRepository.Get<IWizardEntityClientFull>(HostWizard.Guid))
                {
                    var hostWizard = host?.Get<IWizardEntityClientFull>(HostWizard.Guid);
                    await hostWizard.CancelSpell(spellId);
                }
            }
        }

        private bool IsSpellCanceled(SpellId spellId)
        {
            if (!spellId.IsValid)
                return false;
            return CanceledSpells.ContainsKey(spellId);
        }

        public async Task<bool> UpdateImpl(SpellId spellId)
        {
            Spells.TryGetValue(spellId, out var spell);
            if (spell == null)
                return false;
            Logger(LogLevel.Trace)?.Invoke("Update | {0}", spell);

            var stopwatch = StartStopwatch();

            long nextUpdateTime = await _timeLine.UpdateSpell(spell, SyncTime.Now, this);
            if (spell.Status.Activations == 0)
                spell.Status.AccumulatedDelta = 0;
            else
                spell.Status.AccumulatedDelta += SyncTime.NowUnsynced - spell.Status.LastTimeUpdated;
            spell.Status.LastTimeUpdated = SyncTime.NowUnsynced;
            spell.Status.Activations++;
            if (spell.Status.Activations == 5)
            {
                var averageDelta = spell.Status.AccumulatedDelta / (spell.Status.Activations - 1);
                if (averageDelta < 50)
                    Logger(LogLevel.Warn)?.Invoke($"Spell is activated too often {spell.CastData.Def.____GetDebugShortName()} Average delta: {averageDelta:F3}");
                spell.Status.Activations = 0;
            }
            StopStopwatchUpdate(ref stopwatch, spell.CastData);

            Logger(LogLevel.Trace)?.Invoke("Updated | {0} Result:{1}", spell, nextUpdateTime);

            switch (nextUpdateTime)
            {
                case TimelineRunner.RESULT_INFINITE:
                case TimelineRunner.RESULT_FINISHED:
                    return true;
                case TimelineRunner.RESULT_FATAL_FAILURE:
                case TimelineRunner.RESULT_ALREADY_UPDATING:
                case TimelineRunner.RESULT_ALREADY_FINISHED:
                    return false;
                default:
                    DelayUpdate(SyncTime.ToSeconds(Math.Max(10, nextUpdateTime - SyncTime.Now)), spellId, spell.CastData);
                    return true;
            }
        }



        private async Task FinishUpSpell(ISpell spell, long now)
        {
            Logger()?.Invoke("FinishUpSpell | {0}", spell);

            //            spell.FinishReason = spell.AskedToBeFinishedWithReason;
            //            spell.Finished = now;
            if (!IsSlaveEntity)
            {
                var ents = spell.CastData.GetAllImportantEntities();
                UnsubscribeOnSpellImportantEntities(ents);
            }

            await _timeLine.FinishSpell(spell, now, this);

            if (!IsSlaveEntity)
            {
                DelayRemove(0, spell.Id);
            }
            //this.Chain().SpellFinishedDelay(now, spell.Id, spell.FinishReason).Run();
        }

        bool ShouldDoImpactOnFinish(bool failed, SpellImpactTiming timing)
        {
            if (timing == SpellImpactTiming.OnFinish)
                return true;
            if (!failed && timing == SpellImpactTiming.OnSuccess)
                return true;
            if (failed && timing == SpellImpactTiming.OnFail)
                return true;

            return false;
        }

        private ValueTask<IEntitiesContainer> AwaitImportantEntitiesIfNecessary(ISpellCast cast)
        {
            ValueTask<IEntitiesContainer> container = default;
            if (!IsSlaveEntity)
            {
                var batch = EntityBatch.Create();
                foreach (var importantEntity in cast.GetAllImportantEntities())
                {
                    if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(importantEntity.RepTypeId(ReplicationLevel.Server), importantEntity.Guid) == RepositoryEntityContainsStatus.Master)
                        ((IEntityBatchExt)batch).AddExclusive(importantEntity.RepTypeId(ReplicationLevel.Server), importantEntity.Guid);
                    else
                        batch.Add(importantEntity.RepTypeId(ReplicationLevel.Server), importantEntity.Guid);
                }

                if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(Owner.RepTypeId(ReplicationLevel.Server), Owner.Guid) == RepositoryEntityContainsStatus.Master)
                    ((IEntityBatchExt)batch).AddExclusive(Owner.RepTypeId(ReplicationLevel.Server), Owner.Guid);
                else
                    batch.Add(Owner.RepTypeId(ReplicationLevel.Server), Owner.Guid);

                container = EntitiesRepository.Get(batch, cast.Def);
            }
            return container;
        }

        private async ValueTask<bool> CheckPredicatesByData(long currentTime, SpellDef def, ISpellCast cast, SpellId spellId, List<SpellPredicateDef> failedPredicates, IReadOnlyList<SpellModifierDef> modifiers, PredicateIgnoreGroupDef predicateIgnoreGroupDef = null)
        {
            if (def.Predicates == null || def.Predicates.Length == 0)
                return true;

            bool allTrue = true;
            var castData = new SpellPredCastData(wizard: new OuterRef<IWizardEntity>(this),
                castData: cast, caster: Owner, 
                currentTime: currentTime, slaveMark: SlaveWizardMark, 
                canceled: IsSpellCanceled(spellId), modifiers: modifiers, repo:EntitiesRepository);
            IEntitiesContainer importants = null;
            try
            {
                if (await IsOnCooldown(def))
                    return false;
                importants = await AwaitImportantEntitiesIfNecessary(cast);
                foreach (var pred in def.Predicates)
                {
                    bool res;
                    try
                    {
                        if (predicateIgnoreGroupDef != null && predicateIgnoreGroupDef.IgnorePredicatesOfType.Contains(pred.GetType()))
                            continue;

                        var stopwatch = StartStopwatchWord();
                        res = await SpellPredicates.CheckPredicate(castData, pred, EntitiesRepository);
                        AsyncStackHolder.AssertNoChildren();
                        StopStopwatchPredicate(ref stopwatch, pred);
                        if (!res)
                        {
                            //                                if (IsInterestingEnoughToLog)
                            //                                    PutInLog($"FAIL_CAST: predicate failed {def.____GetDebugShortName()} {word.____GetDebugShortName()}");
                            allTrue = false;
                            failedPredicates?.Add(pred);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger(LogLevel.Error)?.Invoke("Exception during predicates check: {0}", e);
                        allTrue = false;
                        res = false;
                    }

                    if (!res)
                        break;
                }
            }
            catch (Exception e)
            {
                Logger(LogLevel.Error)?.Invoke("Exception during predicates check: {0}", e);
            }
            finally
            {
                importants?.Dispose();
            }

            return allTrue;
        }

        public async Task OnDestroy()
        {
            if (IsSlaveEntity)
            {
                using (var wizardC = await EntitiesRepository.Get<IWizardEntityClientBroadcast>(HostWizard.Guid))
                {
                    if (wizardC.TryGet<IWizardEntityClientBroadcast>(HostWizard.Guid, out var wizHost))
                    {
                        wizHost.Spells.OnItemAddedOrUpdated -= Spells_OnItemAdded;
                        wizHost.Spells.OnItemRemoved -= Spell_Finished;
                        wizHost.UnsubscribePropertyChanged(nameof(IWizardEntityClientFull.IsInterestingEnoughToLog), OnIsInterestingEnoughToLog);
                    }
                }

                foreach (var spell in Spells.ToArray())
                    await StopSpellFromHostImpl(spell.Value.Id, SpellFinishReason.FailOnDemand, SyncTime.Now);
            }
            else
            {
                var now = SyncTime.Now;
                foreach (var spell in Spells.ToArray()) // FinishUpSpell удаляет спелл из Spells
                {
                    spell.Value.AskedToFinish = now;
                    spell.Value.AskedToBeFinishedWithReason = SpellFinishReason.FailOnDemand;
                    await FinishUpSpell(spell.Value, SyncTime.Now);
                }
            }

            if (SlaveWizardMark == null)
                await UnsubscribeFromMortal();
        }
        public async Task<bool> CheckSpellCastPredicatesImpl(long currentTime, SpellCast spell, List<SpellPredicateDef> failedPredicates, PredicateIgnoreGroupDef predicateIgnoreGroupDef = null)
        {
            return await CheckPredicatesByData(currentTime, spell.Def, spell, SpellId.Invalid, failedPredicates, await SpellModifiers.GetModifiersForSpell(spell, Owner, EntitiesRepository), predicateIgnoreGroupDef);
        }

        public async Task<bool> ConnectToHostAsReplicaImpl(OuterRef<IWizardEntity> host)
        {

            Logger()?.Invoke("Connecting to host as replica {0}", this.Id);
            using (var wizardC = await EntitiesRepository.Get<IWizardEntityClientBroadcast>(host.Guid))
            {
                var wizHost = wizardC.Get<IWizardEntityClientBroadcast>(host.Guid);
                HostWizard = host;
                WhoAmI = "Wizard Slave";
                foreach (var spell in wizHost.Spells)
                {
                    var newSpell = new Spell { CastData = spell.Value.CastData, Id = spell.Value.Id };
                    Logger()?.Invoke($"#Dbg: CastSpellFromHostImpl id: {newSpell.Id}, {newSpell.CastData.Def.____GetDebugShortName()}");
                    if (spell.Value.FinishReason == SpellFinishReason.None && CheckSpellId(newSpell.Id, newSpell.CastData))
                        await CastSpellFromHostImpl(spell.Value.Id, spell.Value.CastData);
                }
                wizHost.Spells.OnItemAddedOrUpdated += Spells_OnItemAdded;
                wizHost.Spells.OnItemRemoved += Spell_Finished;
                wizHost.SubscribePropertyChanged(nameof(IWizardEntityClientFull.IsInterestingEnoughToLog), OnIsInterestingEnoughToLog);
                Counter = SpellId.FirstSlaveValid;
                //Counter = await wizHost.WizardHasRisen(); // got remembered SlaveCounter from host
            }

            Logger()?.Invoke("Connected to host as replica {0}", this.Id);
            return true;
        }

        private async Task Spell_Finished(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientBroadcast> eventArgs)
        {
            var spell = eventArgs.Value;
            //Logger.IfInfo()?.Message($"SpellFinishedBeforeGet {id}").Write();
            using (var selfE = await EntitiesRepository.Get<IWizardEntity>(Id, spell?.CastData))
            {
                var selfWiz = selfE.Get<IWizardEntity>(Id);
                if (selfWiz == null)
                    return;
                //Log($"SpellFinishedAfterGet {id}");
                await selfWiz.StopSpellFromHost(spell.Id, spell.FinishReason, spell.Finished);
                //Log($"SpellFinishedAfterStopFromHost {id}");
            };
        }
        //        static int _maxFailCount = 2;
        //        ConcurrentDictionary<SpellId, int> _failCounts = new ConcurrentDictionary<SpellId, int>();
        public void DelayUpdate(float delay, SharedCode.Wizardry.SpellId spellId, ISpellCast castData)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                // Handling case of entty was teleported while delay:
                if (!((IEntityExt)this).IsMaster())
                    return;
                try
                {
                    if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(TypeId, Id) == RepositoryEntityContainsStatus.Master)
                        using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(TypeId, Id, castData))
                        {
                            await ((IWizardEntity)this).Update(spellId);
                        }
                }
                catch (Exception e)
                {
                    //                    int failCount = 0;
                    //                    if (_failCounts.TryGetValue(spellId, out failCount))
                    //                        _failCounts[spellId] = failCount + 1;
                    //                    else
                    //                        _failCounts[spellId] = 1;
                    //                    //if (failCount < _maxFailCount)
                    //                    //    DelayUpdate(1, spellId, castData);

                    Logger(LogLevel.Error)?.Invoke($"DelayUpdate spell exception | {spellId} {castData} Wizard:{Id} | {e}");
                    if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(TypeId, Id) == RepositoryEntityContainsStatus.Master)
                        using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(TypeId, Id, castData))
                        {
                            await ((IWizardEntity)this).StopCastSpell(spellId, SpellFinishReason.FailOnDemand);
                        }
                }
            }, EntitiesRepository);
        }


        public async Task<bool> WatchdogUpdateImpl()
        {
            //            if (Spells.Count == 0 && _watchdogToken != null && _watchdogToken.CanBeCancelled)
            //            {
            //                _watchdogToken.Cancel(EntitiesRepository);
            //                _watchdogToken = null;
            //            }
            //            foreach (var spell in Spells)
            //            {
            //                int failCount = 0;
            //                if (_failCounts.TryGetValue(spell.Id, out failCount))
            //                {
            //                    Logger.IfError()?.Message($"SPELLS_WATCHDOG: Spell is not finished correctly {Owner} {spell.Id} {spell.CastData.Def.____GetDebugShortName()}", LogLevel.Error).Write();
            //                    await StopCastSpell(spell.Id, SpellFinishReason.FailOnDemand);
            //                    int val;
            //                    _failCounts.TryRemove(spell.Id, out val);
            //                    DelayUpdate(1, spell.Id, spell.CastData);
            //                }
            //
            //
            //            }
            return true;
        }


        public void DelayRemove(float delay, SharedCode.Wizardry.SpellId spellId)
        {

            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                if (!((IEntityExt)this).IsMaster())
                    return;
                if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(TypeId, Id) == RepositoryEntityContainsStatus.Master)
                    using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(TypeId, Id))
                    {
                        await SpellFinishedDelayImpl(spellId);
                    }
            }, EntitiesRepository);

        }

        private async Task Spells_OnItemAdded(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientBroadcast> eventArgs)
        {
            var spell = eventArgs.Value;
            if (spell == null)
            {
                Logger(LogLevel.Error)?.Invoke($"Null spell in Spells_OnItemAdded | Wizard:{Id}");
                return;
            }
            using (var selfE = await EntitiesRepository.Get<IWizardEntity>(Id, spell.CastData))
            {
                var selfWiz = selfE.Get<IWizardEntity>(Id);
                if (selfWiz == null)
                    return;
                await selfWiz.CastSpellFromHost(spell.Id, spell.CastData);
            }
        }

        public Task<bool> CastSpellFromHostImpl(SpellId id, SpellCast spell)
        {
            Logger()?.Invoke("SpellFromHost | SpellId:{0} {1}", id, spell);
            if (!CheckSpellId(id, spell))
                return Task.FromResult(false);
            Collect.IfActive?.EventBgn($"{WhoAmI}.{spell.Def.____GetDebugRootName()}", Owner, (this, id));
            var newSpell = new Spell { CastData = spell, Id = id };
            Spells.Add(id, newSpell);
            newSpell.Status = CreateNewStatus(spell.Def);
            _timeLine.PrepareSpell(newSpell, _spellWordFilter);
            //this.Chain().Update(id).Run();
            DelayUpdate(0, id, spell);
            return Task.FromResult(true);
        }

        public Task<bool> StopSpellFromHostImpl(SpellId id, SpellFinishReason reason, long timeStamp)
        {
            Logger()?.Invoke("Stop spell from host {0}", id);
            Collect.IfActive?.EventEnd((this, id));
            Spells.TryGetValue(id, out var spellToFinish);
            spellToFinish.StopCastWithReason = reason;
            spellToFinish.StopCast = timeStamp;
            //this.Chain().Update(id).Run();
            DelayUpdate(0, id, spellToFinish.CastData);
            //this.Chain().Delay(0.05f).Update();
            return Task.FromResult(true);
        }

        public async Task<bool> StopAllSpellsOfGroupImpl(SpellGroupDef group, SpellId except, SpellFinishReason reason)
        {
            foreach (var spell in Spells.Where(x => x.Value.CastData.Def.Group == group && x.Value.Id != except).ToList())
                await StopCastSpellImpl(spell.Value.Id, reason);
            return true;
        }

        public async Task<bool> StopSpellByDefImpl(SpellDef spellDef, SpellId except, SpellFinishReason reason)
        {
            foreach (var spell in Spells.Where(x => x.Value.CastData.Def == spellDef && x.Value.Id != except).ToList())
                await StopCastSpellImpl(spell.Value.Id, reason);
            return true;
        }

        public async Task<bool> StopSpellByCauserImpl(SpellPartCastId causer, SpellFinishReason reason)
        {
            bool any = false;
            foreach (var spell in Spells.Where(x => x.Value.Causer == causer).ToList())
                any |= await StopCastSpellImpl(spell.Value.Id, reason);
            return any;
        }

        public ValueTask<bool> HasActiveSpellImpl(SpellDef spell)
        {
            foreach (var x in Spells)
                if (x.Value.CastData.Def == spell)
                    return new ValueTask<bool>(true);
            return new ValueTask<bool>(false);
        }

        public ValueTask<bool> HasActiveSpellGroupImpl(SpellGroupDef group)
        {
            foreach (var x in Spells)
                if (x.Value.CastData.Def.Group == group)
                    return new ValueTask<bool>(true);
            return new ValueTask<bool>(false);
        }

        public async Task<WizardDebugData> GetDebugDataImpl()
        {
            if (IsSlaveEntity)
            {
                using (var host = await EntitiesRepository.Get<IWizardEntityClientFull>(HostWizard.Guid))
                {
                    var masterStatus = await host.Get<IWizardEntityClientFull>(HostWizard.Guid).GetDebugData();
                    masterStatus.SlaveCurrentSpellStatuses = Spells.Select(x => x.Value.Status.TimeLineData).ToList();
                    return masterStatus;
                }

            }
            return new WizardDebugData() { CurrentSpellStatuses = Spells.Select(x => x.Value.Status.TimeLineData).ToList() };
        }

        public async Task<bool> SpellFinishedDelayImpl(SpellId spellId)
        {

            if (!spellId.IsValid)
            {
                CommonLogger.IfError()?.Message("SpellFinishedDelayImpl spellId is not valid").Write();
                return false;
            }
            Logger()?.Invoke("Finished delay | SpellId:{0}", spellId);
            Spells.TryGetValue(spellId, out var spell);
            if (spell != null)
            {
                Spells.Remove(spell.Id);
                CanceledSpells.Remove(spell.Id);
                await TryPlaceCooldown(false, spell.SpellDef);
            }
            Logger(LogLevel.Trace)?.Invoke("Spells after finish: [{0}]", string.Join(", ", Spells.Select(x => x.Value.CastData.Def.____GetDebugAddress())));
            Collect.IfActive?.EventEnd((this, spellId));
            return true;
        }

        private async Task EntitiesRepository_EntityDestroy(int arg1, Guid arg2, IEntity arg3)
        {
            using (var self = await EntitiesRepository.Get<IWizardEntity>(Id))
            {
                self.TryGet<IWizardEntity>(Id, out var entity);
                if (entity != null)
                    await entity.OnLostPossiblyImportantEntity(new OuterRef<IEntity>(arg2, arg1));
            }
        }

        public async Task<bool> OnLostPossiblyImportantEntityImpl(OuterRef<IEntity> ent)
        {
            if (!IsSlaveEntity)
                foreach (var spell in Spells)
                {
                    var validator = spell.Value.Validator ?? (spell.Value.Validator = new SpellStateValidator(spell.Value));
                    if (validator.IsNowInvalid(ent))
                        await StopCastSpell(spell.Value.Id);
                }
            return true;
        }

        public async Task<bool> WizardHasDiedImpl()
        {

            IsDead = true;
            if (IsSlaveEntity)
                return true;
            foreach (var spell in Spells)
                if (!spell.Value.CastData.Def.IgnoresDeath)
                    await StopCastSpell(spell.Value.Id, SpellFinishReason.FailOnDemand);

            return true;
        }

        public Task<SpellId> WizardHasRisenImpl()
        {
            IsDead = false;
            if (IsSlaveEntity)//функционально не имеет смысла, но потом хоть вспомним, что тут хуйня происходит с этими  вызовами на слейв-энтити
                return Task.FromResult(SlaveCounter);
            return Task.FromResult(SlaveCounter);
        }
        public Task<bool> LocalUpdateTimeLineDataImpl()
        {
            return Task.FromResult(true);
        }
        public async Task<long> UpdateImpl()
        {
            return 0;
        }

        public Task<string> DumpEventsImpl()
        {
            var sb = StringBuildersPool.Get;
            foreach (var entry in _loggedStuff)
            {
                sb.Append(entry);
                sb.Append("---\n");
            }
            return Task.FromResult(sb.ToStringAndReturn());
        }

        private bool CheckSpellId(SpellId id, ISpellCast data)
        {
            foreach (var spell in Spells)
            {
                if (spell.Value.Id == id)
                {
                    Logger(LogLevel.Error)?.Invoke($"Duplicated spell id | SpellId:{id} NewSpell:{data.Def.____GetDebugAddress()} ExistingSpell:{spell.Value.CastData.Def.____GetDebugAddress()} Wizard:{Id}");
                    return false;
                }
            }
            return true;
        }

        public Task GetBackFromIdleModeImpl() => Task.CompletedTask;

        public Task GoIntoIdleModeImpl() => Task.CompletedTask;

        private TimelineHelpers.ExecutionMask GetSpellWordExecutionMask(SpellWordDef word, SpellId spellId)
        {
            switch (word)
            {
                case SpellPredicateDef predicate:
                    return NeedToCheckPredicates ? TimelineHelpers.ExecutionMask.Start : TimelineHelpers.ExecutionMask.None;

                case SpellImpactDef impact:

                    if (SlaveWizardMark.IsSlave())
                    {
                        if (!(SlaveWizardMark.OnServer && impact.UnityAuthorityServerImpact))
                            return TimelineHelpers.ExecutionMask.None;
                    }
                    else if (impact.UnityAuthorityServerImpact)
                        return TimelineHelpers.ExecutionMask.None;

                    switch (impact.WhenToApply)
                    {
                        case SpellImpactTiming.OnStart:
                            return TimelineHelpers.ExecutionMask.Start;
                        case SpellImpactTiming.OnFinish:
                        case SpellImpactTiming.OnSuccess:
                        case SpellImpactTiming.OnFail:
                            return TimelineHelpers.ExecutionMask.Finish;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                case SpellEffectDef effect:
                    if (!SlaveWizardMark.OnClient() && SpellEffects.IsClientOnlyEffect(effect))
                        return TimelineHelpers.ExecutionMask.None;
                    if (IsSlaveEntity && SlaveWizardMark.HasClientAuthority && spellId.IsGeneratedBySlave && SpellEffects.IsPredictableEffect(effect))
                        return TimelineHelpers.ExecutionMask.None;
                    else
                        return TimelineHelpers.ExecutionMask.All;

                default:
                    return TimelineHelpers.ExecutionMask.None;
            }
        }

        private bool NeedToCheckPredicates => !IsSlaveEntity || (SlaveWizardMark != null && SlaveWizardMark.HasClientAuthority);

        ValueTask TimelineRunner.IWizard.StartWord(SpellWordDef word, SpellWordCastData castData)
        {
            var executionMask = GetSpellWordExecutionMask(word, castData.SpellId);

            Logger(LogLevel.Trace)?.Invoke($"Start Word | Word:{word} ExecutionMask:{executionMask} {castData}");

            if ((executionMask & TimelineHelpers.ExecutionMask.Start) == 0)
                return new ValueTask();

            switch (word)
            {
                case SpellImpactDef impact:
                    return SpellImpacts.CastImpact(castData, impact, EntitiesRepository);
                case SpellEffectDef effect:
                    return SpellEffects.StartEffect(castData, effect, EntitiesRepository);
                default:
                    return new ValueTask();
            }
        }

        ValueTask TimelineRunner.IWizard.FinishWord(SpellWordDef word, SpellWordCastData castData, bool failed)
        {
            var executionMask = GetSpellWordExecutionMask(word, castData.SpellId);

            Logger(LogLevel.Trace)?.Invoke($"Finish Word | Word:{word} ExecutionMask:{executionMask} Failed:{failed} {castData}");

            if ((executionMask & TimelineHelpers.ExecutionMask.Finish) == 0)
                return new ValueTask();

            switch (word)
            {
                case SpellImpactDef impact when ShouldDoImpactOnFinish(failed, impact.WhenToApply):
                    return SpellImpacts.CastImpact(castData, impact, EntitiesRepository);
                case SpellEffectDef effect:
                    return SpellEffects.EndEffect(castData, effect, EntitiesRepository);
                default:
                    return new ValueTask();
            }
        }

        async Task TimelineRunner.IWizard.SpellFinished(ITimelineSpell spell, long now)
            => await FinishUpSpell((ISpell)spell, now);

        bool TimelineRunner.IWizard.NeedToCheckPredicates => NeedToCheckPredicates;

        ValueTask<IEntitiesContainer> TimelineRunner.IWizard.AwaitImportantEntitiesIfNecessary(ISpellCast cast)
            => AwaitImportantEntitiesIfNecessary(cast);

        ValueTask<bool> TimelineRunner.IWizard.CheckSpellPredicates(long currentTime, SpellDef spell, ISpellCast castData, SpellId spellId, IReadOnlyList<SpellModifierDef> modifiers)
            => CheckPredicatesByData(currentTime, spell, castData, spellId, null, modifiers);

        TimelineRunner.LoggerDelegate TimelineRunner.IWizard.Logger(LogLevel level) => Logger(level);

        SpellWordCastData TimelineRunner.IWizard.CreateWordCastData(long currentTime, long spellStartTime, long parentSubSpellStartTime, TimeRange wordTimeRange, SpellId spellId, int subSpellCount, ISpellCast castData, IReadOnlyList<SpellModifierDef> modifiers)
        {
            return new SpellWordCastData
            (
                wizard: new OuterRef<IWizardEntity>(this),
                castData: castData,
                caster: Owner,
                spellId: spellId,
                subSpellCount: subSpellCount,
                currentTime: currentTime,
                wordTimeRange: wordTimeRange,
                spellStartTime: spellStartTime,
                parentSubSpellStartTime: parentSubSpellStartTime,
                slaveMark: SlaveWizardMark,
                canceled: IsSpellCanceled(spellId),
                firstOrLast: true,
                context: null,
                modifiers: modifiers,
                repo: EntitiesRepository
            );
        }

        Stopwatch TimelineRunner.IWizard.StartStopwatch() => StartStopwatchWord();

        void TimelineRunner.IWizard.StopStopwatch(ref Stopwatch sw, SpellWordDef word, string operation) => StopStopwatchWord(ref sw, word, operation);


        private async Task SubscribeToMortal()
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(Owner.TypeId, Owner.Guid))
            {
                var mortal = wrapper.Get<IHasMortalClientBroadcast>(Owner.TypeId, Owner.Guid, ReplicationLevel.ClientBroadcast);
                if (mortal != null)
                {
                    mortal.Mortal.DieEvent += OnDie;
                    mortal.Mortal.ResurrectEvent += OnResurrect;
                }
            }
        }

        private async Task UnsubscribeFromMortal()
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(Owner.TypeId, Owner.Guid))
            {
                wrapper.TryGet<IHasMortalClientBroadcast>(Owner.TypeId, Owner.Guid, ReplicationLevel.ClientBroadcast, out var mortal);
                if (mortal != null)
                {
                    mortal.Mortal.DieEvent -= OnDie;
                    mortal.Mortal.ResurrectEvent -= OnResurrect;
                }
            }
        }

        private async Task OnDie(Guid entityId, int typeId, PositionRotation corpsePlace)
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(TypeId, Id))
            {
                if (wrapper == null)
                {
                    CommonLogger.IfError()?.Message($"Can't get wrapper by typeId: {TypeId} & Id: {Id}.").Write();
                    return;
                }

                await WizardHasDied();
            }
        }

        private async Task OnResurrect(Guid entityId, int typeId, PositionRotation at)
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(TypeId, Id))
            {
                if (wrapper == null)
                {
                    CommonLogger.IfError()?.Message($"Can't get wrapper by typeId: {TypeId} & Id: {Id}.").Write();
                    return;
                }

                if (IsDead)
                    await WizardHasRisen();
            }
        }

        public Task SetIsInterestingEnoughToLogImpl(bool enable)
        {
            IsInterestingEnoughToLog = enable;
            CommonLogger.IfInfo()?.Message($"Logger for {Id} is {(enable ? "enabled" : "disabled")}").Write();
            return Task.CompletedTask;
        }

        private async Task OnIsInterestingEnoughToLog(EntityEventArgs args)
        {
            bool enable = (bool)args.NewValue;
            using (var cnt = await EntitiesRepository.Get(TypeId, Id))
                await cnt.Get<IWizardEntity>(TypeId, Id).SetIsInterestingEnoughToLog(enable);
        }
    }

    public static class CheatVariables
    {
        public static ConcurrentDictionary<BaseResource, object> Values = new ConcurrentDictionary<BaseResource, object>();
        [Cheat]
        public static void SetCheatVariable(BaseResource varName, string value)
        {
            if (bool.TryParse(value, out var result))
                Values[varName] = result;
            else
                Values[varName] = value;
        }

        public static bool CheckValue(BaseResource varName, string value)
        {
            object val = null;

            if (Constants.WorldConstants.DefaultCheatVariableValues.TryGetValue(varName, out var constValue))
                val = constValue;

            if (Values.TryGetValue(varName, out var dynValue))
                val = dynValue;


            if (bool.TryParse(value, out var result))
            {
                var res = val != null ? (bool)val : false;
                return res == result;
            }
            else
            {
                return ((string)val) == value;
            }
        }
    }
}
