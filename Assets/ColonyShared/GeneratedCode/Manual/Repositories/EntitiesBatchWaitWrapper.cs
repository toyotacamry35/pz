using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.AsyncStack;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Extensions;
using SharedCode.Repositories;

namespace GeneratedCode.Manual.Repositories
{
    public class EntitiesBatchWaitWrapper
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static long LastId = 0;
        public long Id { get; }

        public EntityBatchOperationWrapper[] Queues;

        private int _waitQueuesCount;

        public EntitiesContainer EntitiesContainer { get; private set; }

        private TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Task => CompletionSource.Task;

        private readonly DateTime _createdTime = DateTime.UtcNow;

        public EntitiesBatchWaitWrapper(EntitiesContainer entitiesContainer)
        {
            Id = Interlocked.Increment(ref LastId);
            EntitiesContainer = entitiesContainer;
        }

        public bool Complete()
        {
            try
            {
                CompletionSource.SetResult(true);
                return true;
            }
            catch(InvalidOperationException e)
            {
                if (CompletionSource.Task.IsFaulted)
                {
                    Logger.IfError()?.Message(e, "EntitiesBatchWaitWrapper CompletionSource IsFaulted").Write();;
                    return false;
                }

                if (CompletionSource.Task.IsCanceled)
                {
                    Logger.IfError()?.Message(e, "EntitiesBatchWaitWrapper CompletionSource IsCanceled").Write();;
                    return false;
                }

                if (CompletionSource.Task.IsCompleted)
                {
                    Logger.IfError()?.Message(e, "EntitiesBatchWaitWrapper CompletionSource IsCompleted").Write();;
                    return false;
                }

                Logger.IfError()?.Message("EntitiesBatchWaitWrapper CompletionSource is in completely invalid state").Write();
                return false;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "EntitiesBatchWaitWrapper CompletionSource exception").Write();;
                return false;
            }
        }

        public void SetTimeout()
        {
            CompletionSource.SetException(new RepositoryTimeoutException());
        }

        public void SetQueues(EntityBatchOperationWrapper[] queues, int waitCount)
        {
            Queues = queues;
            _waitQueuesCount = waitCount;
        }

        public void RemoveFromQueues()
        {
            for (int i = 0; i < Queues.Length; i++)
            {
                ref var item = ref Queues[i];
                item.EntityQueue.RemoveBatch(Id);
            }

            Queues = null;
            _waitQueuesCount = 0;
        }

        public void RemoveFromQueuesByTimeout(HashSet<EntityQueue> queuesToRun)
        {
            for (int i = 0; i < Queues.Length; i++)
            {
                ref var item = ref Queues[i];
                item.EntityQueue.RemoveTimeoutedBatches(Id, queuesToRun);
            }

            Queues = null;
            _waitQueuesCount = 0;
        }

        public void DecrementWaitQueuesCount()
        {
            _waitQueuesCount--;
        }

        public bool IsReadyToUse()
        {
            return _waitQueuesCount == 0;
        }

        public void DumpToStringBuilder(StringBuilder sb)
        {
            sb.AppendFormat("EntitiesBatchWaitWrapper {0} created {1} seconds ago containers:", Id, (DateTime.UtcNow - _createdTime).TotalSeconds);
            sb.AppendLine();
            foreach (var currentContainer in AsyncStackEnumerable.ToChildren(EntitiesContainer))
            {
                sb.AppendFormat("      <EntitiesContainer tag {0}", currentContainer.Tag ?? "null");
                if (currentContainer.Batch != null)
                    currentContainer.Batch.DumpToStringBuilder(sb);
                else
                    sb.Append("null");
                sb.Append(">");

            }
        }
    }

    public class EntityRestoredException : InvalidOperationException
    {
        public EntityRestoredException() : base()
        {
        }

        public EntityRestoredException(string message) : base(message)
        {
        }

        public EntityRestoredException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
