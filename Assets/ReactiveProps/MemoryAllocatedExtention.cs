using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;

namespace ReactivePropsNs
{
    public static class MemoryAllocatedExtention
    {
        public static IStream<T> MemoryAllocated<T>(this IStream<T> source, ICollection<IDisposable> disposables, string prefix, NLog.Logger logger = null)
        {
            if (logger == null)
                logger = ReactiveLogs.Logger;
            var output = PooledReactiveProperty<T>.Create(reactOnChangesOnly: false, deepLogPrefix => $"{nameof(MemoryAllocated)}({deepLogPrefix}, {logger})\n{source.DeepLog(deepLogPrefix + '\n')}");
            disposables.Add(output);
            var subscribtion = source.Subscribe(disposables,
                value =>
                {
                    long allocated = GC.GetTotalMemory(false);
                    output.Value = value;
                    logger.IfError()?.Message("{prefix} Memory Allocated: {allocated}", prefix, GC.GetTotalMemory(false) - allocated).Write();
                },
                () => output.Dispose());
            disposables.Add(subscribtion);
            return output;
        }
        public static IStream<T> MemoryAllocatedWarn<T>(this IStream<T> source, ICollection<IDisposable> disposables, string prefix, NLog.Logger logger = null)
        {
            if (logger == null)
                logger = ReactiveLogs.Logger;
            var output = PooledReactiveProperty<T>.Create(reactOnChangesOnly: false, deepLogPrefix => $"{nameof(MemoryAllocated)}({deepLogPrefix}, {logger})\n{source.DeepLog(deepLogPrefix + '\n')}");
            disposables.Add(output);
            var subscribtion = source.Subscribe(disposables,
                value =>
                {
                    long allocated = GC.GetTotalMemory(false);
                    output.Value = value;
                    logger.IfWarn()?.Message("{prefix} Memory Allocated: {allocated}", prefix, GC.GetTotalMemory(false) - allocated).Write();
                },
                () => output.Dispose());
            disposables.Add(subscribtion);
            return output;
        }
        public static IStream<T> MemoryAllocatedDebug<T>(this IStream<T> source, ICollection<IDisposable> disposables, string prefix, NLog.Logger logger = null)
        {
            if (logger == null)
                logger = ReactiveLogs.Logger;
            var output = PooledReactiveProperty<T>.Create(reactOnChangesOnly: false, deepLogPrefix => $"{nameof(MemoryAllocated)}({deepLogPrefix}, {logger})\n{source.DeepLog(deepLogPrefix + '\n')}");
            disposables.Add(output);
            var subscribtion = source.Subscribe(disposables,
                value =>
                {
                    long allocated = GC.GetTotalMemory(false);
                    output.Value = value;
                    logger.IfDebug()?.Message("{prefix} Memory Allocated: {allocated}", prefix, GC.GetTotalMemory(false) - allocated).Write();
                },
                () => output.Dispose());
            disposables.Add(subscribtion);
            return output;
        }
    }
}