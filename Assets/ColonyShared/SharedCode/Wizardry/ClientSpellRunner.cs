using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.Lib.Extensions;
using ColonyShared.SharedCode.Utils;
using SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using NLog;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils.DebugCollector;
using SharedCode.Wizardry;
using static SharedCode.Wizardry.TimelineRunner.WordResult;

namespace SharedCode.Wizardry
{
    public class ClientSpellRunner : TimelineRunner.IWizard
    {
        private static readonly NLog.Logger _Logger = LogManager.GetLogger(nameof(ClientSpellRunner));
        private static WizardWatchdogDef Watchdog;
        private const string WhoAmI = nameof(ClientSpellRunner);


        private const int MinUpdateDelay = 10;
        private const int StopRepeatDelay = 20;       
        
        private readonly OuterRef<IEntity> _ownerRef;
        private readonly IEntitiesRepository _repository;
        private readonly IUnityEnvironmentMark _slaveWizardMark;
        private readonly Dictionary<SpellId, LocalSpell> _spells = new Dictionary<SpellId, LocalSpell>();
        private readonly TimelineRunner _timeline = new TimelineRunner();
        private readonly OuterRef<IWizardEntity> _wizardRef;
        private readonly TimelineRunner.LoggerDelegate[] _loggers;
        private readonly TimelineHelpers.WordExecutionMaskDelegate _spellWordFilter;

        public ClientSpellRunner(OuterRef<IWizardEntity> wizardRef, OuterRef<IEntity> ownerRef, IUnityEnvironmentMark slaveMark, IEntitiesRepository repository)
        {            
            if (!wizardRef.IsValid) throw new ArgumentException(nameof(wizardRef));
            _wizardRef = wizardRef;
            if (!ownerRef.IsValid) throw new ArgumentException(nameof(ownerRef));
            _ownerRef = ownerRef;
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            _repository = repository;
            _slaveWizardMark = slaveMark;
            _spellWordFilter = GetSpellWordExecutionMask;
            _loggers = new TimelineRunner.LoggerDelegate[LogLevel.AllLoggingLevels.Max(x => x.Ordinal) + 1];
            foreach (var level in LogLevel.AllLoggingLevels)
            {
                var l = level; 
                _loggers[level.Ordinal] = (m, p) => 
                    _Logger.Log(new LogEventInfo {Message = StringBuildersPool.Get.Append(WhoAmI).Append(" | ").Append(m).ToStringAndReturn(), Parameters = p, Level = l, LoggerName = _Logger.Name});
            }
            if (Watchdog == null)
                Watchdog = GameResourcesHolder.Instance.LoadResource<WizardWatchdogDef>(WizardEntity.WatchdogConfig);
            Logger(LogLevel.Debug)?.Invoke($"Created | Owner:{ownerRef} Wizard:{wizardRef} Repository:{repository}");
        }

        public bool StartSpell(SpellId castId, ISpellCast castData, SpellId prevSpell, IReadOnlyList<SpellModifierDef> modifiers)
        {
            if(!castId.IsValid) throw new ArgumentException("SpellId is invalid", nameof(castId));
            
            Collect.IfActive?.EventBgn($"ClientSpellRunner.{castData.Def.____GetDebugRootName()}", _ownerRef, new SpellGlobalId(_wizardRef.Guid, castId));

            var spell = LocalSpell.Create(castId, castData, modifiers);
            _timeline.PrepareSpell(spell, _spellWordFilter);
            lock (_spells)
            {
                if (castData.Def.ClearsSlot)
                    foreach (var s in _spells.Fetch(x => castData.Def.Slot != null && x.Value.SpellDef.Slot == castData.Def.Slot || x.Value.SpellDef == castData.Def))
                        StopSpellImpl(s.Value, SpellFinishReason.SucessOnDemand);

                if (prevSpell.IsValid)
                {
                    LocalSpell s;
                    if (_spells.TryGetValue(prevSpell, out s))
                        StopSpellImpl(s, SpellFinishReason.SucessOnDemand);
                }

                if (HasBlockersOfCast(castData, prevSpell))
                    return false;
                
                _spells.Add(castId, spell);
            }
            Logger(LogLevel.Debug)?.Invoke($"Start spell | {spell} CurrentTime:{SyncTime.Now.TimeToString(spell.StartAt)}");
            AsyncUtils.RunAsyncTask(() => SpellWorker(spell), _repository);
            return true;
        }

        public void StopSpell(SpellId castId, SpellFinishReason reason)
        {
            LocalSpell spell;
            lock (_spells)
                _spells.TryGetValue(castId, out spell);
            if (spell != null)
                StopSpellImpl(spell, reason);
        }
        
        public bool HasBlockersOfCast(ISpellCast spell, SpellId ignoreSpell)
        {
            lock(_spells)
                return _spells.Values.Any(x => x.ItBlocksCastOf(spell.Def, ignoreSpell));
        }

        private async Task SpellWorker(LocalSpell spell)
        {            
            Logger(LogLevel.Trace)?.Invoke($"{nameof(SpellWorker)} started | {spell} CurrentTime:{SyncTime.Now.TimeToString(spell.StartAt)}");

            long nextUpdateAt = 0;
            while (true)
            {
                var delay = 0;
                try
                {
                    var curTime = SyncTime.Now;
                    if(nextUpdateAt != 0) Logger(LogLevel.Trace)?.Invoke($"{nameof(SpellWorker)} update | {spell} CurrentTime:{curTime.TimeToString(spell.StartAt)} Lateness:{curTime - nextUpdateAt}");
                    nextUpdateAt = await _timeline.UpdateSpell(spell, curTime, this);
                    switch (nextUpdateAt)
                    {
                        case TimelineRunner.RESULT_INFINITE:
                        case TimelineRunner.RESULT_FINISHED:
                        case TimelineRunner.RESULT_FATAL_FAILURE:
                        case TimelineRunner.RESULT_ALREADY_UPDATING:
                        case TimelineRunner.RESULT_ALREADY_FINISHED:
                            Logger(LogLevel.Trace)?.Invoke($"{nameof(SpellWorker)} done | {spell} NextUpdateAt:{nextUpdateAt.TimeToString()}");
/* Exit point ------> */    return; 
                        default:
                            curTime = SyncTime.Now;
                            delay = Math.Max((int)(nextUpdateAt - curTime), MinUpdateDelay);
                            Logger(LogLevel.Trace)?.Invoke($"{nameof(SpellWorker)} updated | {spell} CurrentTime:{curTime.TimeToString(spell.StartAt)} NextUpdateAt:{nextUpdateAt.TimeToString(spell.StartAt)} Delay:{delay}ms");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger(LogLevel.Error)?.Invoke($"{spell} | Exception: {e}");
                    if (spell.Stop(SyncTime.Now, SpellFinishReason.FailOnDemand))
                        await SpellStopper(spell);
                    break;
                }

                await Task.Delay(delay);
            }
        }

        private async Task SpellStopper(LocalSpell spell)
        {            
            Logger(LogLevel.Trace)?.Invoke($"{nameof(SpellStopper)} started | {spell} CurrentTime:{SyncTime.Now}");

            while (true)
            {
                try
                {
                    await _timeline.UpdateSpell(spell, SyncTime.Now, this);
                    if (spell.IsFinished)
                    {
                        Logger(LogLevel.Trace)?.Invoke($"{nameof(SpellStopper)} done | {spell} CurrentTime:{SyncTime.Now}");
/* Exit point ---> */   return;
                    }

                    Logger(LogLevel.Trace)?.Invoke($"{nameof(SpellStopper)} did not finished | {spell} CurrentTime:{SyncTime.Now}");
                }
                catch (Exception e)
                {
                    Logger(LogLevel.Error)?.Invoke($"{spell} | Exception: {e}");
                }

                await Task.Delay(StopRepeatDelay);
            }
        }
        
        private void StopSpellImpl(LocalSpell spell, SpellFinishReason reason)
        {
            Logger(LogLevel.Debug)?.Invoke($"Stop spell | {spell} Reason:{reason}");
            if (spell == null) throw new ArgumentNullException(nameof(spell));
            if (spell.Stop(SyncTime.Now, reason))
                AsyncUtils.RunAsyncTask(() => SpellStopper(spell), _repository);
        }

        private void RemoveSpell(ITimelineSpell spell)
        {
            lock (_spells)
            {
                var kvp = _spells.FirstOrDefault(x => x.Value == spell);
                if (kvp.Key.IsValid)
                    _spells.Remove(kvp.Key);
            }
        }

        TimelineHelpers.ExecutionMask GetSpellWordExecutionMask(SpellWordDef word, SpellId spellId)
        {
            switch (word)
            {
                case SpellImpactDef impact:
                    return TimelineHelpers.ExecutionMask.None;
                case SpellPredicateDef predicate:
                    return TimelineHelpers.ExecutionMask.None;
                case SpellEffectDef effect:
                    if (SpellEffects.IsPredictableEffect(effect))
                        return TimelineHelpers.ExecutionMask.All;
                    else
                        return TimelineHelpers.ExecutionMask.None;
                default:
                    return TimelineHelpers.ExecutionMask.None;
            }
        }
        
        TimelineRunner.LoggerDelegate Logger(LogLevel level)
        {
            return _Logger.IsEnabled(level) ? _loggers[level.Ordinal] : null;
        }

        public void Dispose()
        {
        }


        #region TimelineRunner.IWizard

        bool TimelineRunner.IWizard.NeedToCheckPredicates => false;

        TimelineRunner.LoggerDelegate TimelineRunner.IWizard.Logger(LogLevel level) => Logger(level);

        ValueTask<IEntitiesContainer> TimelineRunner.IWizard.AwaitImportantEntitiesIfNecessary(ISpellCast spellCast)
        {
            return default;
        }

        ValueTask<bool> TimelineRunner.IWizard.CheckSpellPredicates(long currentTime, SpellDef spell, ISpellCast castData, SpellId spellId, IReadOnlyList<SpellModifierDef> modifiers)
        {
            return new ValueTask<bool>(true);
        }

        async Task TimelineRunner.IWizard.SpellFinished(ITimelineSpell spell, long now)
        {
            if(!(spell is LocalSpell)) throw new ArgumentException($"Wrong type of spell:{spell.GetType()}");
            await _timeline.FinishSpell(spell, now, this);
            RemoveSpell(spell);
            Collect.IfActive?.EventEnd(new SpellGlobalId(_wizardRef.Guid, spell.SpellId));
        }

        ValueTask TimelineRunner.IWizard.StartWord(SpellWordDef word, SpellWordCastData castData)
        {
            if ((GetSpellWordExecutionMask(word, castData.SpellId) & TimelineHelpers.ExecutionMask.Start) == 0)
                return new ValueTask();
            return SpellEffects.StartEffect(castData, (SpellEffectDef)word, _repository);
        }

        ValueTask TimelineRunner.IWizard.FinishWord(SpellWordDef word, SpellWordCastData castData, bool failed)
        {
            if ((GetSpellWordExecutionMask(word, castData.SpellId) & TimelineHelpers.ExecutionMask.Finish) == 0)
                return new ValueTask();
            return SpellEffects.EndEffect(castData, (SpellEffectDef)word, _repository);
        }

        SpellWordCastData TimelineRunner.IWizard.CreateWordCastData(long currentTime, long spellStartTime, long parentSubSpellStartTime, TimeRange wordTimeRange, SpellId spellId, int subSpellCount, ISpellCast castData, IReadOnlyList<SpellModifierDef> modifiers)
        {
            return new SpellWordCastData
            (
                wizard: _wizardRef,
                castData: castData,
                caster: _ownerRef,
                spellId: spellId,
                subSpellCount: subSpellCount,
                currentTime: currentTime,
                wordTimeRange: wordTimeRange,
                spellStartTime: spellStartTime,
                parentSubSpellStartTime: parentSubSpellStartTime,
                slaveMark: _slaveWizardMark,
                firstOrLast: false,
                canceled: false,
                context: null,
                modifiers: modifiers,
                repo:_repository
            );
        }

        Stopwatch TimelineRunner.IWizard.StartStopwatch()
        {
            if (_Logger.IsEnabled(LogLevel.Error))
                return StopwatchPool.GetStarted;
            return null;
        }

        void TimelineRunner.IWizard.StopStopwatch(ref Stopwatch stopwatch, SpellWordDef word, string operation)
        {
            if (stopwatch != null)
            {
                var elapsed = stopwatch.StopAndRelease();
                stopwatch = null;
                if (elapsed > Watchdog.MillisecondsCriticalForStart)
                    Logger(LogLevel.Error)?.Invoke("Spell word takes critical long | {@}", new { WhoAmI, Word = word.____GetDebugAddress(), elapsed, operation} );
                else
                if (elapsed > Watchdog.MillisecondsTooMuchForStart)
                    Logger(LogLevel.Warn)?.Invoke("Spell word takes too long | {@}", new { WhoAmI, Word = word.____GetDebugAddress(), elapsed, operation} );
            }  
        }
        
        #endregion


        #region ITimelineSpell

        private class LocalSpell : ITimelineSpell
        {
            private int _updating;
            private long _stopCast;
            private SpellFinishReason _stopCastWithReason;
            private SpellFinishReason _finishReason;
            private IReadOnlyList<SpellModifierDef> _modifiers;
            
            public static LocalSpell Create(SpellId spellId, ISpellCast castData, IReadOnlyList<SpellModifierDef> modifiers)
            {
                if (!spellId.IsValid) throw new ArgumentNullException(nameof(spellId));
                if (castData == null) throw new ArgumentNullException(nameof(castData));
                return new LocalSpell
                {
                    SpellId = spellId,
                    CastData = castData,
                    Status = LocalSpellStatus.Create(castData.Def, null),
                    _modifiers = modifiers
                };
            }

            public long StartAt => CastData.StartAt;
            public SpellId SpellId { get; private set; }
            public SpellDef SpellDef => CastData.Def;
            public ISpellCast CastData { get; private set; }
            public IReadOnlyList<SpellModifierDef> Modifiers => _modifiers;
            public ITimelineSpellStatus Status { get; private set; }
            public long StopCast => _stopCast;
            public SpellFinishReason StopCastWithReason => _stopCastWithReason;
            public bool IsAskedToFinish { get; private set; }
            public SpellFinishReason AskedToBeFinishedWithReason { get; private set; }
            public bool IsFinished { get; private set; }
        
            bool ITimelineSpell.EnterToUpdate()
            {
                if (Interlocked.Increment(ref _updating) > 1)
                {
                    if(_updating % 20 == 0)
                        throw new Exception("Can't Enter To Update too many time");
                    return false;
                }
                return true;
            }

            bool ITimelineSpell.ExitFromUpdate()
            {
                return Interlocked.Exchange(ref _updating, 0) > 1;
            }

            public void AskToFinish(long at, SpellFinishReason reason)
            {
                IsAskedToFinish = true;
                AskedToBeFinishedWithReason = reason;
            }

            public void Finish(long at, SpellFinishReason reason)
            {
                IsFinished = true;
                _finishReason = reason;
            }

            public bool Stop(long at, SpellFinishReason reason)
            {
                if (Interlocked.CompareExchange(ref _stopCast, at, 0) == 0)
                {
                    _stopCastWithReason = reason;
                    return true;
                }
                return false;
            }
            
            public override string ToString()
            {
                return ToString(false);
            }

            public string ToString(bool withSubSpells)
            {
                var sb = StringBuildersPool.Get;
                sb
                    .Append("[")
                    .Append("Id:").Append(SpellId)
                    .Append(" ").Append(CastData.Def.____GetDebugAddress())
                    .Append(" Status:").Append(Status.ToString(false, false));
                if(IsFinished)
                    sb.Append(" FIN:").Append(_finishReason);
                else
                if(IsAskedToFinish)
                    sb.Append(" ATF:").Append(AskedToBeFinishedWithReason);
                else
                if(StopCast != 0)
                    sb.Append(" STP:").Append(StopCastWithReason).Append(":").Append(StopCast);
                if (withSubSpells)
                {
                    sb.Append(" SubSpells:[");
                    if(Status.SubSpells != null)
                        foreach (var subSpell in Status.SubSpells)
                            sb.Append(subSpell.ToString(true, true));
                    sb.Append("] ");
                }
                sb.Append("]");
                return sb.ToStringAndReturn();
            }
        }

        private class LocalSpellStatus : ITimelineSpellStatus
        {
            private List<(SpellWordDef,int)> _wordActivations;

            public int SuccesfullPredicatesCheckCount { get; set; }
            public int FailedPredicatesCheckCount { get; set; }
            public int SuccesfullActivationsCount { get; set; }
            public int ActivationsCount { get; set; }
            public int DeactivationsCount { get; set; }
            public SpellDef SpellDef { get; private set; }
            public SubSpell SubSpell { get; private set; }

            public IEnumerable<ITimelineSpellStatus> SubSpells { get; private set; }

            public SpellTimeLineData TimeLineData { get; set; }

            public IEnumerable<(SpellWordDef,int)> WordActivations => _wordActivations;

            public void IncrementWordActivations(SpellWordDef word, int activationIdx)
            {
                (_wordActivations ?? (_wordActivations = CreateWordActivations(SpellDef))).Add((word, activationIdx));
            }

            public bool DecrementWordActivations(SpellWordDef word)
            {
                if (_wordActivations != null)
                    for(int idx = _wordActivations.Count - 1; idx >= 0; --idx)
                        if (_wordActivations[idx].Item1 == word)
                        {
                            _wordActivations.RemoveAt(idx);
                            return true;
                        }
                return false;
            }

            public static LocalSpellStatus Create(SpellDef spellDef, SubSpell subSpell)
            {
                return new LocalSpellStatus
                {
                    SpellDef = spellDef,
                    SubSpell = subSpell,
                    SubSpells = CreateSubSpells(spellDef.SubSpells)
                    //SubSpells = spellDef.SubSpells.Select(x => Create(x.Spell, x)).ToArray(),
                };
            }

            private static LocalSpellStatus[] CreateSubSpells(SubSpell[] subSpells)
            {
                var rv = new LocalSpellStatus[subSpells.Length];
                for (var i = 0; i < subSpells.Length; i++)
                    rv[i] = Create(subSpells[i].Spell, subSpells[i]);
                return rv;
            }

            private static List<(SpellWordDef, int)> CreateWordActivations(SpellDef spellDef)
            {
                var cnt = 0;
                foreach (var word in spellDef.Words)
                    if (word is SpellEffectDef && SpellEffects.IsPredictableEffect((SpellEffectDef) word))
                        ++cnt;
                return new List<(SpellWordDef, int)>(cnt);
            }
            
            public override string ToString()
            {
                return ToString(true, false);
            }

            public string ToString(bool withDef, bool withSubSpells)
            {
                var sb = StringBuildersPool.Get.Append("[");
                if (withDef)
                    sb.Append(SubSpell != null ? SubSpell.____GetDebugAddress() : SpellDef.____GetDebugAddress()).Append(" ");
                sb.Append("A:").Append(ActivationsCount).Append("/").Append(this.FailedActivationsCount());
                sb.Append(" D:").Append(DeactivationsCount);
                if (withSubSpells)
                {
                    sb.Append(" SubSpells:[");
                    if(SubSpells != null)
                        foreach (var subSpell in SubSpells)
                            sb.Append(subSpell.ToString(true, true));
                    sb.Append("] ");
                }
                return sb.Append("]").ToStringAndReturn();
            }
        }

        #endregion
    }
}