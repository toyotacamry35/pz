using ColonyShared.SharedCode.Utils;
using Src.Aspects.Impl.Stats.Proxy;

namespace Src.Aspects.Impl.Stats
{
    public static class TimeStatHelpers
    {
        /// <summary>
        /// Вычисляет время когда значение time stat'а перестанет меняться.
        /// Если значение и так не меняется (change rate равен 0), возвращает время последнего изменения (LastBreakPointTime)
        /// Если предел не задан, возвращяет long.MaxValue
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static long CalculateChangingStopTime(ITimeStat stat)
        {
            var state = stat.State;

            float diff = 0, changeRate = 0;
            if (state.ChangeRateCache > float.Epsilon)
            {
                if (stat.LimitMaxCache == float.MaxValue)
                    return long.MaxValue;
                diff = stat.LimitMaxCache - state.LastBreakPointValue;
                changeRate = state.ChangeRateCache;
            }
            else 
            if (state.ChangeRateCache < float.Epsilon)
            {
                if (stat.LimitMinCache == float.MinValue)
                    return long.MaxValue;
                diff = state.LastBreakPointValue - stat.LimitMinCache;
                changeRate = -state.ChangeRateCache;
            }

            if (diff < 0)
                diff = 0;
            
            return state.LastBreakPointTime + (changeRate != 0 ? SyncTime.FromSeconds(diff / changeRate) : 0);
        }
    }
}