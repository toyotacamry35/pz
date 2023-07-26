using ColonyShared.SharedCode.Utils;
using NLog;
using System;
using Core.Environment.Logging.Extension;

namespace SharedCode.Wizardry
{
    public static class TimelineHelpers
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public const long MinPeriod = 10; // ms

        public delegate ExecutionMask WordExecutionMaskDelegate(SpellWordDef word, SpellId spellId);
            
        [Flags]
        public enum ExecutionMask { None = 0, Start = 0x1, Finish = 0x2, All = Start | Finish };
        
        // make data, the spell with its state could be fully restored from 
        public static void CreateSpellTimeLineData(ITimelineSpell spell, WordExecutionMaskDelegate execMaskFn)
        {
            spell.Status.TimeLineData = new SpellTimeLineData
            (
                spellDef: spell.SpellDef,
                timeRange: TimeRange.FromDuration(0, !spell.CastData.IsInfinite ? spell.CastData.Duration : Int64.MaxValue),
                mustNotFail: true,
                period: Int64.MaxValue,
                executionMask: ExecutionMask.All,
                activationsLimit: 1
            );
            CreateSubSpellsTimeLineData(spell.Status, spell.Status.TimeLineData, spell.SpellId, execMaskFn);
        }

        private static void CreateSubSpellsTimeLineData(ITimelineSpellStatus status,  in SpellTimeLineData parent, SpellId spellId, WordExecutionMaskDelegate execMaskFn)
        {
            if (status.SubSpells != null)
                foreach (var subSpell in status.SubSpells)
                    CreateSubSpellTimeLineData(subSpell, parent, spellId, execMaskFn);
        }
        
        private static void CreateSubSpellTimeLineData(ITimelineSpellStatus status, in SpellTimeLineData parent, SpellId spellId, WordExecutionMaskDelegate execMaskFn)
        {
            SubSpell subSpell = status.SubSpell;
            TimeRange timeRange = TimeRange.FromDuration(CalcStartOffsetFromSubSpellAndParent(parent, subSpell), CalcDurationFromSubSpellAndParent(parent, subSpell));
            
            if (timeRange.Start < 0)
            {
                Logger.IfWarn()?.Message($"Offset for {subSpell} is {timeRange.Start}").Write();
                timeRange = new TimeRange(0, timeRange.Finish);
            }

            if (!parent.IsInfinite && timeRange.Start > parent.Duration)
            {
                Logger.IfWarn()?.Message($"Offset {timeRange.Start} for {subSpell} is great than parent duration {parent.Duration}").Write();
                timeRange = new TimeRange(parent.Duration, parent.Duration);
            }

            long period;
            int activationsLimit;
            
            if (timeRange.Duration != Int64.MaxValue)
            {
                period = Math.Max(timeRange.Duration + SyncTime.FromSeconds(Math.Max(subSpell.PeriodDelay, 0)), MinPeriod);
                activationsLimit = subSpell.Periodic ? CalcActivationsLimitFromSubSpellAndParent(parent.Duration, timeRange.Start, period) : 1;
            }
            else
            {
                period = Int64.MaxValue;
                activationsLimit = 1;
            }
            
            ExecutionMask execMask = 0;
            if (subSpell.Periodic)
                execMask = ExecutionMask.Finish;
            foreach (var word in status.SpellDef.Words)
                execMask |= execMaskFn(word, spellId);

            status.TimeLineData = new SpellTimeLineData(
                spellDef: subSpell.Spell,
                timeRange: timeRange,
                mustNotFail: subSpell.MustNotFail,
                executionMask: execMask,
                activationsLimit: activationsLimit,
                period: period
            );

            CreateSubSpellsTimeLineData(status, status.TimeLineData, spellId, execMaskFn);
        }

        // вычисляет смещение относительно parent'а
        public static long CalcStartOffsetFromSubSpellAndParent(in SpellTimeLineData parent, SubSpell subSpell)
        {
            if (subSpell.OffsetIsFromParentEnd && parent.IsInfinite) throw new ArgumentException($"You can't use OffsetIsFromParentEnd in IsInfinite spell | Spell:{parent.SpellDef}");
            return CalcStartOffsetFromSubSpellAndParent(parent.Duration, parent.IsInfinite, subSpell);
        }

        public static long CalcStartOffsetFromSubSpellAndParent(long parentDuration, bool parentIsInfinite, SubSpell subSpell)
        {
            return !subSpell.OffsetIsFromParentEnd ?
                SyncTime.FromSeconds(subSpell.OffsetStart) + (!parentIsInfinite ? (long) (parentDuration * subSpell.RelativeOffsetStart) : 0) :
                parentDuration - (SyncTime.FromSeconds(subSpell.OffsetStart) + (long) (parentDuration * subSpell.RelativeOffsetStart));
        }
        
        private static long CalcDurationFromSubSpellAndParent(in SpellTimeLineData parent, SubSpell subSpell)
        {
            if (subSpell.OverrideDuration)
                return SyncTime.FromSeconds(subSpell.OverridenDuration);
            if (subSpell.OverrideDurationPercent)
                return !parent.IsInfinite ? (long) (parent.Duration * subSpell.OverridenDurationPercent) : Int64.MaxValue;
            return !subSpell.Spell.Target.IsInfinite ? SyncTime.FromSeconds(subSpell.Spell.Target.Duration) : Int64.MaxValue;
        }

        private static int CalcActivationsLimitFromSubSpellAndParent(long parentDuration, long subSpellOffset, long subSpellPeriod)
        {
            if (parentDuration == long.MaxValue)
                return int.MaxValue;
            if (subSpellPeriod == long.MaxValue)
                return 1;
            parentDuration -= subSpellOffset;
            int count = (int)(parentDuration / subSpellPeriod);
            return count * subSpellPeriod < parentDuration ? count + 1 : count;
        }

        public static TimeRange SpellTimeRange(ITimelineSpellStatus exec, int activationIdx, long parentStartTime)
        {
            if(activationIdx >= exec.TimeLineData.ActivationsLimit) throw new Exception($"activationIdx:{activationIdx} >= activationsLimit:{exec.TimeLineData.ActivationsLimit} | {exec.ToString(true,true)}");
            if(exec.TimeLineData.Period == Int64.MaxValue && activationIdx > 0) throw new Exception($"activationIdx:{activationIdx} > 0 for not periodical spell | {exec.ToString(true,true)}");
            var start = parentStartTime + exec.TimeLineData.StartOffset + activationIdx * exec.TimeLineData.Period;
            var finish = !exec.TimeLineData.IsInfinite ? start + exec.TimeLineData.Duration : Int64.MaxValue;
            return new TimeRange(start, finish);
        }
    }
}