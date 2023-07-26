using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Src.Telemetry
{
    public class TelemetrySystem : ITelemetryInterface
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private static float UpdateInterval = 0.5f;
        private static float LagThreshold = 0.1f;

        private float _realFrameTime = 0.0f;
        private float _frameTime = 0.0f;
        private float _averageFrameTime = 0.0f;
        private float _fps = 0.0f;
        private float _averageFps = 0.0f;
        private long _allocatedMemory = 0;
        private long _allocatedMemoryOnFrame = 0;
        private int _frame;

        private float _accumulatedFpsOnInterval = 0.0f;
        private float _accumulatedFrameTimeOnInterval = 0.0f;
        private float _framesOnInterval = 0.0f;
        private float _intervalTimeLeft = 0.0f;
        private float _timeOfLastGC;
        private float _allocatedAfterLastGC;
        private float _timeOfLastLag;
        private float _realtimeSinceStartup;

        public static ITelemetryInterface Telemetry => GameState.Instance ? GameState.Instance.TelemetrySystem : null;

        public float AverageFrameTime { get { return _averageFrameTime; } }
        public float AverageFPS { get { return _averageFps; } }

        public float FrameTime { get { return _frameTime; } }
        public float FPS { get { return _fps; } }

        public long AllocatedMemoryTotal => _allocatedMemory;

        public long AllocatedMemoryOnFrame => _allocatedMemoryOnFrame;

        public bool IsUpdateRequired => IsGCStatisticsEnabled;
        public bool IsGCStatisticsEnabled  => Logger.IsDebugEnabled;
        
        public void Init()
        {
            _intervalTimeLeft = UpdateInterval;
        }

        public void Update()
        {
            _frame = Time.frameCount;
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            _realFrameTime = realtimeSinceStartup - _realtimeSinceStartup;
            _frameTime = Time.unscaledDeltaTime;
            _realtimeSinceStartup = realtimeSinceStartup;
            _fps = 1 / _frameTime;
            _intervalTimeLeft -= _frameTime;
            _accumulatedFrameTimeOnInterval += (_frameTime * 1000.0f);
            _accumulatedFpsOnInterval += _fps;
            _framesOnInterval += 1.0f;
            var prevFrameAllocatedMemory = _allocatedMemory;
            _allocatedMemory = Profiler.GetMonoUsedSizeLong(); //GetTotalAllocatedMemoryLong();
            _allocatedMemoryOnFrame = _allocatedMemory - prevFrameAllocatedMemory;

            if (_intervalTimeLeft <= 0)
            {
                _averageFrameTime = _accumulatedFrameTimeOnInterval / _framesOnInterval;
                _averageFps = _accumulatedFpsOnInterval / _framesOnInterval;
                _intervalTimeLeft = UpdateInterval;
                _accumulatedFrameTimeOnInterval = 0.0f;
                _accumulatedFpsOnInterval = 0.0f;
                _framesOnInterval = 0.0f;
            }

            if (IsGCStatisticsEnabled)
            {
                if (_allocatedMemoryOnFrame < 0)
                {
                    var period = _realtimeSinceStartup - _timeOfLastGC;
                    var growth = (prevFrameAllocatedMemory - _allocatedAfterLastGC);
                    Logger.Info("{GC}", new
                    {
                        Time = _realtimeSinceStartup,
                        Frame = _frame,
                        TimeSincePrevGC = period, 
                        AllocatedBefore = prevFrameAllocatedMemory / 1000,
                        AllocatedAfter = _allocatedMemory / 1000,
                        Freed = -_allocatedMemoryOnFrame / 1000,
                        GrowthForPeriod = growth / 1000,
                        GrowthPerSecond = period > 0 ? ((float)growth / 1000) / period : float.NaN  
                    });
                    _timeOfLastGC = _realtimeSinceStartup;
                    _allocatedAfterLastGC = _allocatedMemory;
                }

                if (_realFrameTime > LagThreshold)
                {
                    var period = _realtimeSinceStartup - _timeOfLastLag;
                    Logger.Info("{Lag}", new
                    {
                        Time = _realtimeSinceStartup,
                        Frame = _frame,
                        FrameTime = _realFrameTime,
                        TimeSincePrevLag = period, 
                    });
                    _timeOfLastLag = _realtimeSinceStartup;
                }
            }
        }

        //Assets\Telemetry\ElasticAccessor.cs
        //Assets\ColonyShared\Telemetry\WorldCharacterEvents.cs
        private void TestTelemetry()
        {
            //var repo = GameState.Instance.ClientClusterNode;
            //var telemetryEvent = new HardwareEvent()
            //{
            //    VideoCard = SystemInfo.graphicsDeviceName,
            //};

            //TODO
            //AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
            //{
            //    using (var wrapper = await repo.GetFirstService<ITelemetryServiceEntityClientFull>())
            //    {
            //        var telemetryServiceEntity = wrapper.GetFirstService<ITelemetryServiceEntityClientFull>();
            //        if (telemetryServiceEntity != null)
            //        {
            //            var result = await telemetryServiceEntity.IndexEvent(telemetryEvent);
            //        }
            //    }
            //}, repo).WrapErrors();
            //----
        }
    }
}
