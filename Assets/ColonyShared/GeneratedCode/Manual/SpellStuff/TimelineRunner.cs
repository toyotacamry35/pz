using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using GeneratedCode.Manual.Repositories;
using NLog;
using ResourceSystem.Aspects;
using Scripting;
using SharedCode.EntitySystem;

namespace SharedCode.Wizardry
{
    public class TimelineRunner
    {
        public delegate void LoggerDelegate(string log, params object[] format);

        public enum WordResult { Successful, Failed }
        
        public const long RESULT_INFINITE = long.MaxValue;
        public const long RESULT_IMMEDIATELY = 0;
        public const long RESULT_FINISHED = -1;
        public const long RESULT_ALREADY_UPDATING = -2;
        public const long RESULT_ALREADY_FINISHED = -3;
        public const long RESULT_FATAL_FAILURE = -10;
        
        public const int MaxActionsPerUpdate = 32;

        private int _updateId; // for debug purposes only
        
        public void PrepareSpell(ITimelineSpell spell, TimelineHelpers.WordExecutionMaskDelegate execMaskFn)
        {
            TimelineHelpers.CreateSpellTimeLineData(spell, execMaskFn); // "0" means "before spell start"
        }

        public async Task<long> UpdateSpell(ITimelineSpell spell, long currentTime, IWizard wizard)
        {
            if (spell == null)
                return RESULT_FATAL_FAILURE;

            // всех, кто стучится во время обновления отфутболиваем, но запоминаем что кто-то стучался
            if (!spell.EnterToUpdate())
                return RESULT_ALREADY_UPDATING;

            var nextUpdateTime = RESULT_INFINITE;

            try
            {
                if (spell.IsFinished)
                    return RESULT_ALREADY_FINISHED;

                if (spell.StopCast != 0)
                {
                    wizard.Logger(LogLevel.Debug)?.Invoke("Ask To Finish | {0} StopCast:{1} Reason:{2}", spell, spell.StopCast, spell.StopCastWithReason);
                    spell.AskToFinish(spell.StopCast, spell.StopCastWithReason);
                }

                if (!spell.IsAskedToFinish)
                {
                    IEntitiesContainer importants = null;
                    try
                    {
                        importants = await wizard.AwaitImportantEntitiesIfNecessary(spell.CastData);

                        var updateId = ++_updateId;

                        if (spell.StartAt == 0)
                            throw new Exception($"Spell start time is not set! Spell:{spell}");

                        if (spell.Status.DeactivationsCount > 0)
                            throw new Exception($"Updating finished spell | {spell.ToString(true)}");

                        //FailOnEnd does not count as we've already processed all subspells and their finish
                        bool failed = spell.IsAskedToFinish && spell.AskedToBeFinishedWithReason.IsFail();
                        bool finished = spell.IsAskedToFinish && spell.AskedToBeFinishedWithReason.IsSuccess(); // по идее, при SucessOnTime, завершающие action'ы должны сгенериться естественным путём и без флага finished, но, иногда происходит так, что завершение спелла с SucessOnTime вызывается раньше, чем было запланировано (например спелл завершился на хосте и слейвам приходит эвент о его завершении, но на самом слейве завершение было запланировано на более познее время (почему так происходит отдельная тема для разбирательства) ), и в резульатет нужные action'ы не генерятся       
                        var finishAt = !spell.Status.TimeLineData.IsInfinite ? spell.StartAt + spell.Status.TimeLineData.Duration : long.MaxValue;
                        var forceFinish = finished || failed;

                        wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId} | {spell} | StartAt:{spell.StartAt} FinishAt:{finishAt.RelTimeToString(spell.StartAt)} Current:{currentTime.RelTimeToString(spell.StartAt)} Finished:{finished} Failed:{failed} ForceFinish:{forceFinish}");

                        if (!forceFinish)
                        {
                            nextUpdateTime = (await UpdateTimeline(
                                depth: 0,
                                currentTime: currentTime,
                                rootStartTime: spell.StartAt,
                                parentRange: new TimeRange(spell.StartAt, finishAt),
                                parentActivationIdx: 0,
                                exec: spell.Status,
                                rootSpell: spell,
                                nextUpdateTime: finishAt,
                                wizard: wizard,
                                actionsCount: 0,
                                updateId: updateId)).NextTime;
                        }
                        else
                        {
                            await FinishTimeline(
                                depth: 0,
                                currentTime: currentTime,
                                rootStartTime: spell.StartAt,
                                parentRange: new TimeRange(spell.StartAt, currentTime),
                                parentActivationIdx: 0,
                                exec: spell.Status,
                                rootSpell: spell,
                                updateId: updateId,
                                wizard: wizard);
                            nextUpdateTime = long.MaxValue;
                        }
                    }
                    catch (SpellBreakException)
                    {
                        wizard.Logger(LogLevel.Info)?.Invoke($"Spell execution broke due MaxActionsPerUpdate:{MaxActionsPerUpdate} reached | {spell}");
                        nextUpdateTime = currentTime + 10;
                    }
                    catch (SpellFailedException e)
                    {
                        spell.AskToFinish(currentTime, e.Reason);
                    }
                    catch (Exception e)
                    {
                        wizard.Logger(LogLevel.Error)?.Invoke($"{e.Message}\n{spell}\n{e.StackTrace}");
                        spell.AskToFinish(currentTime, SpellFinishReason.FailOnDemand);
                    }
                    finally
                    {
                        importants?.Dispose();
                    }
                }

//                if (spell.IsAskedToFinish && (!wizard.CanFinishSpells || spell.StopCast != 0)) // в оригинале было так, но spell.StopCast != 0, по сути, не работало, поэтому при !spell.EnterToUpdate() возвращаем не -1 (см. выше), как было изначально  
                if (spell.IsAskedToFinish)
                {
                    await wizard.SpellFinished(spell, currentTime);
                    return RESULT_FINISHED;
                }
            }
            finally
            {
                // если кто-то стучался во время обновления, то нужно немедленно обновиться ещё раз, вместо того, кто стучался 
                if (spell.ExitFromUpdate())
                    nextUpdateTime = RESULT_IMMEDIATELY;
            }

            return nextUpdateTime;
        }

        public async Task FinishSpell(ITimelineSpell spell, long currentTime, IWizard wizard)
        {
            wizard.Logger(LogLevel.Trace)?.Invoke("Finish spell | {0} Time:{1} Reason:{2}", spell, currentTime, spell.AskedToBeFinishedWithReason);
            spell.Finish(currentTime, spell.AskedToBeFinishedWithReason);

            if (spell.Status.DeactivationsCount == 0)
            {
                IEntitiesContainer importants = null;
                try
                {
                    importants = await wizard.AwaitImportantEntitiesIfNecessary(spell.CastData);

                    await FinishTimeline(
                        depth: 0,
                        currentTime: currentTime,
                        rootStartTime: spell.StartAt,
                        parentRange: new TimeRange(spell.StartAt, currentTime),
                        parentActivationIdx: 0,
                        exec: spell.Status,
                        wizard: wizard,
                        rootSpell: spell,
                        updateId: ++_updateId);
                }
                catch (Exception e)
                {
                    wizard.Logger(LogLevel.Error)?.Invoke($"{e.Message}\n{spell}\n{e.StackTrace}");
                }
                finally
                {
                    importants?.Dispose();
                }
            }

            // этот вызов нужен для подстраховки. по идее, если всё работает как задумывалось, к этому моменту всё уже должно завершиться 
            await FinishUnfinishedWords(spell.Status, spell, currentTime, spell.AskedToBeFinishedWithReason.IsFail(), wizard);
        }

        private async Task<bool> StartSubSpell(ITimelineSpellStatus spellStatus, ITimelineSpell rootSpell, TimeRange timeRange, TimeRange parentTimeRange, long currentTime, int activationIdx, IWizard wizard)
        {
            bool success = true;

            if (spellStatus == rootSpell.Status && spellStatus.ActivationsCount > 0)
                wizard.Logger(LogLevel.Error)?.Invoke("Spell already activated | {0}", rootSpell.ToString(true));

            var needToCheckPredicates = wizard.NeedToCheckPredicates;
            bool predicatesTrue = true;

            if (needToCheckPredicates)
            {
                predicatesTrue = await wizard.CheckSpellPredicates(currentTime, spellStatus.SpellDef, rootSpell.CastData, rootSpell.SpellId, rootSpell.Modifiers);
                if (predicatesTrue)
                    spellStatus.SuccesfullPredicatesCheckCount++;
                else
                {
                    spellStatus.FailedPredicatesCheckCount++;
                    wizard.Logger(LogLevel.Debug)?.Invoke("Check predicates failed | {0} SpellId:{1}", spellStatus, rootSpell.SpellId);
                }
            }

            if (predicatesTrue)
            {
                if (spellStatus?.SpellDef?.Words != null)
                {
                    var wordCastData = wizard.CreateWordCastData(currentTime, rootSpell.StartAt, parentTimeRange.Start, timeRange, rootSpell.SpellId, activationIdx, rootSpell.CastData, rootSpell.Modifiers);
                    for (var i = 0; success && i < spellStatus.SpellDef.Words.Length; i++)
                    {
                        var w = spellStatus.SpellDef.Words[i];
                        try
                        {
                            var word = w.ResolveWordRef();
                            if (word is SpellPredicateDef) continue;
                            var stopwatch = wizard.StartStopwatch();
                            await wizard.StartWord(word, wordCastData);
                            AsyncStackHolder.AssertNoChildren();
                            wizard.StopStopwatch(ref stopwatch, word, "StartWord");
                            spellStatus.IncrementWordActivations(word, activationIdx);
                        }
                        catch (Exception e)
                        {
                            wizard.Logger(LogLevel.Error).Invoke($"{spellStatus} | {e.ToString()}");
                            success = false;
                        }
                    }
                }
                else
                    wizard.Logger(LogLevel.Warn)?.Invoke("Spell without words | Status:{0} Spell:{1}", spellStatus, spellStatus?.SpellDef?.____GetDebugAddress());
            }
            else
                success = false;

            return success;
        }
        
        private async Task<bool> FinishSubSpell(ITimelineSpellStatus spellStatus, ITimelineSpell rootSpell, TimeRange timeRange, TimeRange parentTimeRange, long currentTime, int activationIdx, IWizard wizard)
        {
            bool success = true;
            
            var failed = rootSpell.AskedToBeFinishedWithReason.IsFail();

            if (spellStatus.SuccesfullActivationsCount > spellStatus.DeactivationsCount)
            {
                var wordCastData = wizard.CreateWordCastData(currentTime, rootSpell.StartAt, parentTimeRange.Start, timeRange, rootSpell.SpellId, activationIdx, rootSpell.CastData, rootSpell.Modifiers);
                for (int i = spellStatus.SpellDef.Words.Length - 1; i >= 0; --i)
                {
                    var w = spellStatus.SpellDef.Words[i];
                    try
                    {
                        var word = w.ResolveWordRef();
                        if (word is SpellPredicateDef) continue;
                        if (spellStatus.DecrementWordActivations(word))
                        {
                            await wizard.FinishWord(word, wordCastData, failed);
                            AsyncStackHolder.AssertNoChildren();
                        }
                    }
                    catch (Exception e)
                    {
                        wizard.Logger(LogLevel.Error)?.Invoke($"{spellStatus} | {e.ToString()}");
                        success = false;
                    }
                }
            }

            return success;
        }

        private static void CheckSubSpellsIsNotActive(ITimelineSpellStatus spellStatus)
        {
            foreach (var subSpell in spellStatus.SubSpells)
                if (subSpell.IsActive())
                    throw new ActionsOrderException($"SubSpell {subSpell.SpellDef.____GetDebugAddress()} has not been finished, although parent {spellStatus.SpellDef.____GetDebugAddress()} is finished");
        }

        private static async Task FinishUnfinishedWords(ITimelineSpellStatus status, ITimelineSpell rootSpell, long currentTime, bool failed, IWizard wizard)
        {
            if (status.WordActivations != null)
            {
                foreach (var unfinishedWord in status.WordActivations)
                {
                    wizard.Logger(LogLevel.Warn) ?.Invoke($"Unfinished word found during spell FinishUp | Word:{unfinishedWord.Item1.____GetDebugShortName()}");
                    try
                    {
                        var word = unfinishedWord.Item1.ResolveWordRef();
                        var wordCastData = wizard.CreateWordCastData(currentTime, rootSpell.StartAt, currentTime, new TimeRange(currentTime, currentTime), rootSpell.SpellId, unfinishedWord.Item2, rootSpell.CastData, rootSpell.Modifiers);
                        await wizard.FinishWord(word, wordCastData, failed);
                    }
                    catch (Exception e)
                    {
                        wizard.Logger(LogLevel.Error)?.Invoke("Exception during finishing unfinished words. This may cause invalid state in case of bad effects. | {0}", e);
                    }
                }
            }

            foreach (var subSpell in status.SubSpells)
                await FinishUnfinishedWords(subSpell, rootSpell, currentTime, failed, wizard);
        }

        private async Task<(long NextTime, int ActionsCount, TimelineHelpers.ExecutionMask ExecutionMask)> UpdateTimeline(
            long nextUpdateTime,
            short depth,
            TimeRange parentRange,
            int parentActivationIdx,
            long currentTime,
            long rootStartTime,
            ITimelineSpellStatus exec,
            ITimelineSpell rootSpell,
            int actionsCount,
            int updateId,
            IWizard wizard)
        {
            if(exec.TimeLineData.ActivationsLimit == int.MaxValue && parentActivationIdx > 0) throw new Exception($"Incorrect TimeLineData for {exec}");
            int activationIdxOrigin = parentActivationIdx * exec.TimeLineData.ActivationsLimit;
      
            int localActivationsCount = exec.ActivationsCount - activationIdxOrigin;
            int localDeactivationsCount = exec.DeactivationsCount - activationIdxOrigin;
         
            wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | UpdateTimeLineData | {exec} | depth:{depth} parentRange:{parentRange.ToString(rootStartTime)} parentActivationIdx:{parentActivationIdx} currentTime:{currentTime.RelTimeToString(rootStartTime)} activationIdxOrigin:{activationIdxOrigin} localActivationsCount:{localActivationsCount} localDeactivationsCount:{localDeactivationsCount} activationsLimit:{exec.TimeLineData.ActivationsLimit} executionMask:{exec.TimeLineData.ExecutionMask}");
            
            if (localActivationsCount < 0) wizard.Logger(LogLevel.Error)?.Invoke($"activationsCount:{localActivationsCount} < 0 | activationIdxOrigin:{activationIdxOrigin} parentActivationIdx:{parentActivationIdx} | {exec.ToString(true, true)}");
            if (localActivationsCount > exec.TimeLineData.ActivationsLimit) wizard.Logger(LogLevel.Error)?.Invoke($"activationsCount:{localActivationsCount} > activationsLimit:{exec.TimeLineData.ActivationsLimit} | activationIdxOrigin:{activationIdxOrigin} parentActivationIdx:{parentActivationIdx} | {exec.ToString(true, true)}");

            TimelineHelpers.ExecutionMask executionMask = exec.TimeLineData.ExecutionMask;
            
            float prevFinish = 0;
            for (int localActivationIdx = localDeactivationsCount; localActivationIdx < exec.TimeLineData.ActivationsLimit; ++localActivationIdx)
            {
                var range = TimelineHelpers.SpellTimeRange(exec, localActivationIdx, parentRange.Start);
                range = new TimeRange(range.Start, Math.Min(range.Finish, parentRange.Finish));

                wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Range#{localActivationIdx}:{range.ToString(rootStartTime)} | {exec}");
                
                if (range.Start < prevFinish)
                    throw new Exception($"range.Start:{range.Start} < prevFinish:{prevFinish} | activationIdxOrigin:{activationIdxOrigin} parentActivationIdx:{parentActivationIdx} | {exec.ToString(true, true)}");
                prevFinish = range.Finish;

                if (SyncTime.InTheFuture(range.Start, parentRange.Finish))
                {
                    wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Range In the Future | {exec}");
                    break;
                }

                if (localActivationIdx >= localActivationsCount)
                {
                    if ((exec.TimeLineData.ExecutionMask & TimelineHelpers.ExecutionMask.Start) != 0)
                    {
                        if (SyncTime.InThePast(range.Start, currentTime))
                        {
                            if (++actionsCount > MaxActionsPerUpdate)
                                throw new SpellBreakException();

                            wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Do Start | {exec} A:{exec.ActivationsCount}/{exec.FailedActivationsCount()} D:{exec.DeactivationsCount}");

                            var success = await StartSubSpell(spellStatus: exec, rootSpell: rootSpell, currentTime: currentTime, timeRange: range, parentTimeRange: parentRange, activationIdx: activationIdxOrigin + localActivationIdx, wizard: wizard);
                            IncrementSubSpellActivations(exec, success);

                            if (!success && exec.TimeLineData.MustNotFail)
                            {
                                var reason = exec == rootSpell.Status ? SpellFinishReason.FailOnStart : SpellFinishReason.FailOnEnd;
                                wizard.Logger(LogLevel.Debug)?.Invoke("FAIL_CAST: {0} Reason: {1}", exec, reason);
                                throw new SpellFailedException(reason);
                            }
                        }
                        else
                        {
                            if (nextUpdateTime >= range.Start)
                            {
                                nextUpdateTime = range.Start;
                                wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Schedule Start | {exec} Next:{nextUpdateTime.RelTimeToString(rootStartTime)} A:{exec.ActivationsCount}/{exec.FailedActivationsCount()} D:{exec.DeactivationsCount}");
                            }

                            break;
                        }
                    }
                    else
                    {
                        if (SyncTime.InThePast(range.Start, currentTime))
                        {
                            wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Fake activation | {exec}");
                            foreach (var word in exec.SpellDef.Words)
                                exec.IncrementWordActivations(word, activationIdxOrigin + localActivationIdx);
                            IncrementSubSpellActivations(exec, true);
                        }
                        else
                        {
                            wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Skip scheduling start | {exec}");
                        }
                    }
                }

                foreach (var subSpell in exec.SubSpells)
                {
                    var res = await UpdateTimeline(
                        nextUpdateTime: nextUpdateTime,
                        depth: (short) (depth + 1),
                        currentTime: currentTime,
                        rootStartTime: rootStartTime,
                        parentRange: range,
                        parentActivationIdx: activationIdxOrigin + localActivationIdx,
                        exec: subSpell,
                        rootSpell: rootSpell,
                        updateId: updateId,
                        actionsCount: actionsCount,
                        wizard: wizard);
                    nextUpdateTime = Math.Min(res.NextTime, nextUpdateTime);
                    actionsCount = res.ActionsCount;
                    executionMask |= res.ExecutionMask;
                }

                if ((exec.TimeLineData.ExecutionMask & TimelineHelpers.ExecutionMask.Finish) != 0)
                {
                    if (SyncTime.InThePast(range.Finish, currentTime))
                    {
                        if (++actionsCount > MaxActionsPerUpdate)
                            throw new SpellBreakException();

                        wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Do Finish | {exec} A:{exec.ActivationsCount}/{exec.FailedActivationsCount()} D:{exec.DeactivationsCount}");

                        var success = await FinishSubSpell(spellStatus: exec, rootSpell: rootSpell, currentTime: currentTime, timeRange: range, parentTimeRange: parentRange, activationIdx: activationIdxOrigin + localActivationIdx, wizard: wizard);
                        IncrementSubSpellDeactivations(exec);

                        if (success)
                        {
                            if (exec == rootSpell.Status)
                                rootSpell.AskToFinish(currentTime, SpellFinishReason.SucessOnTime);
                        }
                        else if (exec.TimeLineData.MustNotFail)
                        {
                            var reason = SpellFinishReason.FailOnEnd;
                            wizard.Logger(LogLevel.Debug)?.Invoke("FAIL_CAST: {0} Reason: {1}", exec, reason);
                            throw new SpellFailedException(reason);
                        }
                    }
                    else
                    {
                        if (nextUpdateTime >= range.Finish)
                        {
                            nextUpdateTime = range.Finish;
                            wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Schedule Finish | {exec} Next:{nextUpdateTime.RelTimeToString(rootStartTime)} A:{exec.ActivationsCount}/{exec.FailedActivationsCount()} D:{exec.DeactivationsCount}");
                        }

                        break;
                    }
                }
                else
                {
                    if (SyncTime.InThePast(range.Finish, currentTime))
                    {
                        wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Fake deactivation | {exec}");
                        foreach (var word in exec.SpellDef.Words)
                            exec.DecrementWordActivations(word);
                        IncrementSubSpellDeactivations(exec);
                    }
                    else
                    {
                        wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Skip scheduling finish | {exec}");
                        
                        if (executionMask == TimelineHelpers.ExecutionMask.None)
                            break; // если ни этот спелл, ни его сабспеллы, не содержат выполняемых слов, то дальше проверять нет смысла, всёравно ничего не зашедулится 
                    }
                }
            }

            return (nextUpdateTime, actionsCount, executionMask);
        }

        private static void IncrementSubSpellDeactivations(ITimelineSpellStatus exec)
        {
            if (exec.ActivationsCount > exec.DeactivationsCount)
                exec.DeactivationsCount++;

            CheckSubSpellsIsNotActive(exec);
        }

        private static void IncrementSubSpellActivations(ITimelineSpellStatus exec, bool success)
        {
            exec.ActivationsCount++;
            if (success)
                exec.SuccesfullActivationsCount++;
        }

        private async Task FinishTimeline(
            IWizard wizard,
            short depth,
            TimeRange parentRange,
            int parentActivationIdx,
            long currentTime,
            long rootStartTime,
            ITimelineSpellStatus exec,
            ITimelineSpell rootSpell,
            int updateId)
        {
            if(exec.TimeLineData.ActivationsLimit == int.MaxValue && parentActivationIdx > 0) throw new Exception($"Incorrect TimeLineData for {exec}");
            int activationIdxOrigin = parentActivationIdx * exec.TimeLineData.ActivationsLimit;
      
            int localActivationsCount = exec.ActivationsCount - activationIdxOrigin;
            int localDeactivationsCount = exec.DeactivationsCount - activationIdxOrigin;
         
            wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | FinishTimeLineData | {exec} | depth:{depth} parentRange:{parentRange.ToString(rootStartTime)} parentActivationIdx:{parentActivationIdx} currentTime:{currentTime.RelTimeToString(rootStartTime)} activationIdxOrigin:{activationIdxOrigin} localActivationsCount:{localActivationsCount} localDeactivationsCount:{localDeactivationsCount} activationsLimit:{exec.TimeLineData.ActivationsLimit}");
            
            if (localActivationsCount < 0) wizard.Logger(LogLevel.Error)?.Invoke($"activationsCount:{localActivationsCount} < 0 | activationIdxOrigin:{activationIdxOrigin} parentActivationIdx:{parentActivationIdx} | {exec.ToString(true, true)}");
            if (localActivationsCount > exec.TimeLineData.ActivationsLimit) wizard.Logger(LogLevel.Error)?.Invoke($"activationsCount:{localActivationsCount} > activationsLimit:{exec.TimeLineData.ActivationsLimit} | activationIdxOrigin:{activationIdxOrigin} parentActivationIdx:{parentActivationIdx} | {exec.ToString(true, true)}");
            
            for (int localActivationIdx = localDeactivationsCount; localActivationIdx < localActivationsCount; ++localActivationIdx)
            {
                var range = TimelineHelpers.SpellTimeRange(exec, localActivationIdx, parentRange.Start);
                range = new TimeRange(range.Start, Math.Min(range.Finish, parentRange.Finish));

                wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} | Range#{localActivationIdx}:{range.ToString(rootStartTime)} | {exec.SpellDef.____GetDebugAddress()}");

                foreach (var subSpell in exec.SubSpells)
                {
                    await FinishTimeline(
                            depth: (short) (depth + 1),
                            currentTime: currentTime,
                            rootStartTime: rootStartTime,
                            parentRange: range,
                            parentActivationIdx: activationIdxOrigin + localActivationIdx,
                            exec: subSpell,
                            rootSpell: rootSpell,
                            updateId: updateId,
                            wizard: wizard);
                }

                await FinishSubSpell(timeRange: range, parentTimeRange: parentRange, spellStatus: exec, activationIdx: activationIdxOrigin + localActivationIdx, rootSpell: rootSpell, currentTime: currentTime, wizard: wizard);
                IncrementSubSpellDeactivations(exec);
    
                wizard.Logger(LogLevel.Trace)?.Invoke($"Timeline #{updateId:####} |_Do_Finish_______| {exec.SpellDef.____GetDebugAddress()} A:{exec.ActivationsCount}/{exec.FailedActivationsCount()} D:{exec.DeactivationsCount}");
            }
        }
   
        public interface IWizard
        {
            bool NeedToCheckPredicates { get; }
            LoggerDelegate Logger(LogLevel level);
            ValueTask<IEntitiesContainer> AwaitImportantEntitiesIfNecessary(ISpellCast spellCast);
            ValueTask<bool> CheckSpellPredicates(long currentTime, SpellDef spell, ISpellCast castData, SpellId spellId, IReadOnlyList<SpellModifierDef> modifiers);
            Task SpellFinished(ITimelineSpell spell, long now);
            ValueTask StartWord(SpellWordDef word, SpellWordCastData castData);
            ValueTask FinishWord(SpellWordDef word, SpellWordCastData castData, bool failed);
            SpellWordCastData CreateWordCastData(long currentTime, long spellStartTime, long parentSubSpellStartTime, TimeRange wordTimeRange, SpellId spellId, int subSpellCount, ISpellCast castData, IReadOnlyList<SpellModifierDef> modifiers);
            Stopwatch StartStopwatch();
            void StopStopwatch(ref Stopwatch sw, SpellWordDef word, string operation);
        }

        private class ActionsOrderException : Exception
        {
            public ActionsOrderException(string message) : base(message) {}
        }

        private class SpellFailedException : Exception
        {
            public readonly SpellFinishReason Reason;

            public SpellFailedException(SpellFinishReason reason)
            {
                Reason = reason;
            }
        }
        
        private class SpellBreakException : Exception
        {
        }
    }
}