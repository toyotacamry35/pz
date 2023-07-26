using System;
using Nest;

namespace ColonyShared.SharedCode.Utils
{
    public struct TimeRange
    {
        public readonly long Start;
        public readonly long Finish;
        
        public TimeRange(long start, long finish)
        {
            Start = start;
            Finish = finish;
        }

        public bool IsValid => Start > 0 && Finish >= Start;
        
        public static TimeRange FromDuration(long start, long duration)
        {
            return new TimeRange(start, start != TimeUnitsHelpers.Infinite && duration != TimeUnitsHelpers.Infinite ? start + duration : TimeUnitsHelpers.Infinite);
        }
        
        public long Duration => Finish != TimeUnitsHelpers.Infinite && Start != TimeUnitsHelpers.Infinite ? Finish - Start : TimeUnitsHelpers.Infinite;
        
        public static TimeRange operator-(TimeRange r1, long offset)
        {
            return new TimeRange(
                r1.Start != TimeUnitsHelpers.Infinite ? r1.Start - offset : TimeUnitsHelpers.Infinite,
                r1.Finish != TimeUnitsHelpers.Infinite ? r1.Finish - offset : TimeUnitsHelpers.Infinite
            );
        }

        public static TimeRange operator+(TimeRange r1, long offset)
        {
            return new TimeRange(
                r1.Start != TimeUnitsHelpers.Infinite ? r1.Start + offset : TimeUnitsHelpers.Infinite,
                r1.Finish != TimeUnitsHelpers.Infinite ? r1.Finish + offset : TimeUnitsHelpers.Infinite
            );
        }
        
        public string ToString(long rootStartTime)
        {
            return $"({Start.RelTimeToString(rootStartTime)}, {Finish.RelTimeToString(rootStartTime)})";
        }
        
        public override string ToString()
        {
            return $"({Start.TimeToString()}, {Finish.TimeToString()})";
        }
    }
}
