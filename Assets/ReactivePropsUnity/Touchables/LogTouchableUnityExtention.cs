using System;
using System.Collections.Generic;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables {
    public static class LogTouchableUnityExtention {
        public static ITouchable<T> Log<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor = new LogTouchableExtention.Processor<T>(prefix, toString, logger, LogTouchableExtention.Processor<T>.LogLevel.Error, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }
        public static ITouchable<T> LogInfo<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor = new LogTouchableExtention.Processor<T>(prefix, toString, logger, LogTouchableExtention.Processor<T>.LogLevel.Info, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }
        public static ITouchable<T> LogDebug<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor = new LogTouchableExtention.Processor<T>(prefix, toString, logger, LogTouchableExtention.Processor<T>.LogLevel.Debug, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }
        public static ITouchable<T> LogTrace<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor = new LogTouchableExtention.Processor<T>(prefix, toString, logger, LogTouchableExtention.Processor<T>.LogLevel.Trace, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }
    }
}
