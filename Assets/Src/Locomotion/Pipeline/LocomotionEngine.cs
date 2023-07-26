using System;
using System.Diagnostics;
using Assets.ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
 
namespace Src.Locomotion
{
    public class LocomotionEngine : ILocomotionEngine
    {
        // ReSharper disable once UnusedMember.Local
        static readonly LocomotionLogger Logger = LocomotionLogger.GetLogger(nameof(LocomotionEngine));

        private UpdateablesCollection _updateables;
        private ILocomotionDebugable[] _debugables;
        private ILocomotionPipelineFetchNode _fetchNode;
        private ILocomotionPipelineCommitNode _commitNode;
        private ILocomotionComposablePipeline _composablePipeline;
        private readonly LocomotionDebug.Context _debug;
        private bool _ready;

        public LocomotionEngine(LocomotionDebug.Context dbg)
        {
            _debug = dbg;
        }

        public LocomotionEngine Updateables([NotNull] params ILocomotionUpdateable[] updateables)
        {
            if (updateables == null) throw new ArgumentNullException(nameof(updateables));
            if (_updateables != null) throw new InvalidOperationException($"Method {nameof(Updateables)} was called twice");
            _updateables = new UpdateablesCollection(updateables);
            return this;
        }

        public LocomotionEngine Debugables([NotNull] params ILocomotionDebugable[] debugables)
        {
            _debugables = debugables;
            return this;
        }
 
        public ILocomotionEngine ComposePipeline([NotNull] ILocomotionPipelineFetchNode node, Action<ILocomotionComposablePipeline> builder)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (_composablePipeline != null) throw new InvalidOperationException($"Method {nameof(ComposePipeline)} was called twice");
            _fetchNode = node;
            var pipeline = new LocomotionPipeline();
            builder.Invoke(pipeline);
            _commitNode = pipeline;
            _composablePipeline = pipeline;
            return this;
        }

        public long DbgUpdateCounter { get; private set; }
        public void Execute(float dt)
        {
            if (dt <= 0)
                throw new ArgumentException($"Delta time can't be less or equal to zero\n{(new StackTrace()).ToString()}");

            ++DbgUpdateCounter;
            _debug?.BeginOfFrame();

            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## LocomotionEngine.Execute: 1) updateables.Update");
            _updateables.Update(dt);
            LocomotionProfiler.EndSample();

            if (_fetchNode.IsReady && _commitNode.IsReady)
            {
                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## LocomotionEngine.Execute: 2) _fetchNode.Fetch");
                var vars = _fetchNode.Fetch(dt);
                LocomotionProfiler.EndSample();

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## LocomotionEngine.Execute: 3) _commitNode.Commit");
                _commitNode.Commit(ref vars, dt);
                LocomotionProfiler.EndSample();
            }

            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## DebugAgent.Gather");

            if (LocomotionDebug.DebugAgent != null && _debugables != null)
                foreach (var debugable in _debugables)
                    LocomotionDebug.DebugAgent.Gather(debugable);

            LocomotionProfiler.EndSample();

            _debug?.EndOfFrame();
        }

        public void Dispose()
        {
            _updateables?.Dispose();
            _composablePipeline?.Dispose();
        }
    }

    //Namely Locomotion
    public static class LocomotionProfiler
    {
        public static bool EnableProfile = false;
        static LocomotionProfiler()
        {
            EnableProfile = GlobalConstsDef.DebugFlagsGetter.IsEnableLocomotionProfiler(GlobalConstsHolder.GlobalConstsDef);
        }

        //public static void BeginSample(object obj1, object obj2) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(string.Concat(obj1, obj2)); }
        public static void BeginSample(object obj1) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(obj1.ToString()); }
        public static void EndSample() { if (EnableProfile) UnityEngine.Profiling.Profiler.EndSample(); }
    }

    //Other: (PawnWatchDog & Char.)
    public static class LocomotionProfiler2
    {
        public static bool EnableProfile = false;
        static LocomotionProfiler2()
        {
            EnableProfile = GlobalConstsDef.DebugFlagsGetter.IsEnableLocomotionProfiler2(GlobalConstsHolder.GlobalConstsDef);
        }

        //public static void BeginSample(object obj1, object obj2) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(string.Concat(obj1, obj2)); }
        public static void BeginSample(object obj1) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(obj1.ToString()); }
        public static void EndSample() { if (EnableProfile) UnityEngine.Profiling.Profiler.EndSample(); }
    }

    //SpatialHash
    public static class LocomotionProfiler3
    {
        public static bool EnableProfile = false;
        static LocomotionProfiler3()
        {
            EnableProfile = GlobalConstsDef.DebugFlagsGetter.IsEnableLocomotionProfiler3(GlobalConstsHolder.GlobalConstsDef);
        }

        //public static void BeginSample(object obj1, object obj2) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(string.Concat(obj1, obj2)); }
        public static void BeginSample(object obj1) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(obj1.ToString()); }
        public static void EndSample() { if (EnableProfile) UnityEngine.Profiling.Profiler.EndSample(); }
    }

    //SpatialLegionary
    public static class LocomotionProfiler4
    {
        public static bool EnableProfile = false;
        static LocomotionProfiler4()
        {
            EnableProfile = GlobalConstsDef.DebugFlagsGetter.IsEnableLocomotionProfiler4(GlobalConstsHolder.GlobalConstsDef);
        }

        //public static void BeginSample(object obj1, object obj2) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(string.Concat(obj1, obj2)); }
        public static void BeginSample(object obj1) { if (EnableProfile) UnityEngine.Profiling.Profiler.BeginSample(obj1.ToString()); }
        public static void EndSample() { if (EnableProfile) UnityEngine.Profiling.Profiler.EndSample(); }
    }

}