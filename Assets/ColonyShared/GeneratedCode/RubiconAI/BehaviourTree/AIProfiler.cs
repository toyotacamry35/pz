using SharedCode.Aspects.Item.Templates;
using SharedCode.MovementSync;

namespace Assets.Src.RubiconAI.BehaviourTree
{
    public static class AIProfiler
    {
        public static bool EnableProfile = false;
        static AIProfiler()
        {
            EnableProfile = Constants.WorldConstants.EnableAIProfiler;
        }

        public static void BeginSample(object obj1, object obj2) {
            if (EnableProfile) ProfilerProxy.Profiler?.BeginSample($"{obj1}-{obj2}");
        }
        public static void BeginSample(object obj1) {
            if (EnableProfile) ProfilerProxy.Profiler?.BeginSample($"{obj1}");
        }
        public static void EndSample() {
            if (EnableProfile) ProfilerProxy.Profiler?.EndSample();
        }
    }
}
