using GeneratedCode.Manual.Repositories;
using SharedCode.EntitySystem;
using System;
using System.Diagnostics;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.Manual.AsyncStack
{
    public struct AsyncStackIsolator : IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("AsyncStack");

        public StackTrace CallerStack { get; }

        public static AsyncStackIsolator IsolateContext(StackTrace callerStack = null)
        {
            return new AsyncStackIsolator(callerStack);
        }

        private AsyncStackIsolator(StackTrace _callerStack)
        {
            CallerStack = _callerStack;
            AsyncEntitiesRepositoryRequestContext.Head?.Context?.CheckValid();
            AsyncEntitiesRepositoryRequestContext.Head = null;
            disposedValue = false;
        }

        // Check that context was emptied correctly
        private static bool CheckAsyncEntitiesRepositoryRequestContext(AsyncEntitiesRepositoryRequestContext ctx)
        {
            var item = ctx.Tail;

            if (item == null)
                return true;

            Logger.IfError()?.Message("asyncEntitiesRepositoryRequestContext WrappersStack is not empty {0}", item.Context.Id).Write();
            LogAsyncEntitiesRepositoryRequestContextRecursive(null, item);
            return false;
        }

        private static void LogAsyncEntitiesRepositoryRequestContextRecursive(EntitiesContainer parent, EntitiesContainer item)
        {
            var container = item;
            Logger.Error("asyncEntitiesRepositoryRequestContext {0} parentBatchId {1} batchId {2} tag {3} created stackTrace {4} current --- stackTrace {5}",
                item.Context.Id, parent?.Batch?.Id.ToString() ?? "none",
                    ((IEntitiesContainerExtension)container).Batch?.Id.ToString() ?? "none", ((IEntitiesContainerExtension)container).Tag ?? "null",
                        ((IEntitiesContainerExtension)container).CreatedStackTrace?.ToString() ?? string.Empty, ((IEntitiesContainerExtension)container).CurrentStackTrace?.ToString() ?? string.Empty);

            if (item.Child != null)
                LogAsyncEntitiesRepositoryRequestContextRecursive(item, (EntitiesContainer)item.Child);
        }


        #region IDisposable Support
        private bool disposedValue;

        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var value = AsyncEntitiesRepositoryRequestContext.Head?.Context;
                    if (value != null)
                    {
                        Logger.IfFatal()?.Message("Async context is not cleared: {0}. Caller stack: {1}", value, CallerStack?.ToString() ?? "<not set>").Write();
                        if (!CheckAsyncEntitiesRepositoryRequestContext(value))
                            Logger.IfError()?.Message("{0} is tainted. Caller stack: {1}", this, CallerStack?.ToString() ?? "<not set>").Write();
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
        public override string ToString()
        {
            return $"[{typeof(AsyncStackIsolator).Name}]";
        }
    }
}
