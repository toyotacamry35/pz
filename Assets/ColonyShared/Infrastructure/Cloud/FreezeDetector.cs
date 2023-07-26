using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using Prometheus;
using SharedCode.EntitySystem;

namespace GeneratedCode.Infrastructure.Cloud
{
    public static class FreezeDetectorFactory
    {
        public static FreezeDetector Start(FreezeDetectorSettings settings)
        {
            return new FreezeDetector(settings);
        }

        public class FreezeDetector : IDisposable
        {
            private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
            private readonly FreezeDetectorSettings _settings;
            private volatile bool _stopped;

            public FreezeDetector(FreezeDetectorSettings settings)
            {
                _settings = settings;
                StartThreadDetector();
                StartThreadPoolDetector();
            }

            private void StartThreadDetector()
            {
                new Thread(s =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    var sw = new Stopwatch();
                    sw.Start();
                    while (!_stopped)
                    {
                        Thread.Sleep(_settings.ThreadSleepTime);
                        
                        var elapsed = sw.Elapsed;
                        sw.Restart();
                        
                        if (ServerCoreRuntimeParameters.WriteToLogFreeze)
                        {
                            if (elapsed > _settings.ThreadSleepWarningTime)
                            {
                                _logger.IfWarn()?.Message("Thread sleep took {time}ms", elapsed.TotalMilliseconds).Write();
                            }
                        }

                        _settings.ThreadSleepHistogram.Observe(elapsed.TotalMilliseconds);
                    }
                }).Start();
            }

            private void StartThreadPoolDetector()
            {
                Task.Run(async () =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    while (!_stopped)
                    {
                        await Task.Delay(_settings.ThreadPoolSleepTime);
                        var elapsed = sw.Elapsed;
                        sw.Restart();
                        
                        if (ServerCoreRuntimeParameters.WriteToLogFreeze)
                        {
                            if (elapsed > _settings.ThreadPoolWarningTime)
                            {
                                _logger.IfWarn()?.Message("ThreadPool sleep took {time}ms", elapsed.TotalMilliseconds).Write();
                            }
                        }
                        
                        _settings.ThreadPoolSleepHistogram.Observe(elapsed.TotalMilliseconds);
                    }
                });
            }

            public void Dispose()
            {
                _stopped = true;
            }
        }
    }

    public class FreezeDetectorSettings
    {
        public FreezeDetectorSettings(TimeSpan threadSleepTime,
            Histogram threadSleepHistogram,
            TimeSpan threadSleepWarningTime,
            TimeSpan threadPoolSleepTime,
            Histogram threadPoolSleepHistogram,
            TimeSpan threadPoolWarningTime)
        {
            ThreadSleepTime = threadSleepTime;
            ThreadSleepHistogram = threadSleepHistogram;
            ThreadSleepWarningTime = threadSleepWarningTime;
            ThreadPoolSleepTime = threadPoolSleepTime;
            ThreadPoolSleepHistogram = threadPoolSleepHistogram;
            ThreadPoolWarningTime = threadPoolWarningTime;
        }

        public TimeSpan ThreadSleepTime { get; }
        public Histogram ThreadSleepHistogram { get; }
        public TimeSpan ThreadSleepWarningTime { get; }
        public TimeSpan ThreadPoolSleepTime { get; }
        public Histogram ThreadPoolSleepHistogram { get; }
        public TimeSpan ThreadPoolWarningTime { get; }
    }
}