using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace ColonyShared.SharedCode.Utils
{
    public static class StopwatchPool
    {
        private const int MaxPoolSize = 30;
        private static readonly ConcurrentStack<Stopwatch> _localSB = new ConcurrentStack<Stopwatch>(Enumerable.Range(0, 10).Select(x => new Stopwatch()));
        
        public static Stopwatch Get => _localSB.TryPop(out var result) ? result : new Stopwatch();

        public static Stopwatch GetStarted
        {
            get
            {
                var sb = Get;
                sb.Start();
                return sb;
            }
        }

        public static long StopAndRelease(this Stopwatch sw)
        {
            sw.Stop();
            var res = sw.ElapsedMilliseconds;
            sw.Reset();
            if (_localSB.Count < MaxPoolSize)
                _localSB.Push(sw);
            return res;
        }
    }
}