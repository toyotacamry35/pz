using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using Harmony;
using System;
using System.Diagnostics;
using NLog.Fluent;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.Repositories;

namespace UnityAsyncAwaitUtil
{
    internal sealed class UnitySynchronizationContext2 : SynchronizationContext
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("UnitySynchronizationContext");

        private readonly ConcurrentQueue<WorkRequest> m_AsyncWorkQueue = new ConcurrentQueue<WorkRequest>();
        private readonly int m_MainThreadID = Thread.CurrentThread.ManagedThreadId;

        private readonly Stopwatch sw = new Stopwatch();

        private static readonly TimeSpan WarnThreshold = TimeSpan.FromMilliseconds(500);
        private static readonly TimeSpan ErrorThreshold = TimeSpan.FromMilliseconds(1000);

        private static UnitySynchronizationContext2 Instance;

        public override void Send(SendOrPostCallback callback, object state)
        {
            if (m_MainThreadID == Thread.CurrentThread.ManagedThreadId)
                callback(state);
            else
            {
                using (var waitHandle = new ManualResetEventSlim(false))
                {
                    m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state, ExecutionContext.Capture(), waitHandle));
                    waitHandle.Wait();
                }
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state, ExecutionContext.Capture(), null));
        }

        public void SendUnsafe(SendOrPostCallback callback, object state)
        {
            if (m_MainThreadID == Thread.CurrentThread.ManagedThreadId)
                callback(state);
            else
            {
                using (var waitHandle = new ManualResetEventSlim(false))
                {
                    m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state, null, waitHandle));
                    waitHandle.Wait();
                }
            }
        }

        public void PostUnsafe(SendOrPostCallback callback, object state)
        {
            m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state, null, null));
        }

        private void Exec()
        {
            sw.Stop();

            NLog.LogLevel level;
            if (sw.Elapsed < WarnThreshold)
                level = NLog.LogLevel.Debug;
            else if (sw.Elapsed < ErrorThreshold)
                level = NLog.LogLevel.Warn;
            else
                level = NLog.LogLevel.Error;

            if (Logger.IsEnabled(level)) Logger.Log(level, "Called with interval of {0}", sw.Elapsed);
              
            var workCount = m_AsyncWorkQueue.Count;
            for (int i = 0; i < workCount; i++)
            {
                WorkRequest work;
                if (m_AsyncWorkQueue.TryDequeue(out work))
                    work.Invoke();
                else
                    break;
            }
            sw.Reset();
            sw.Start();
        }

        private static void ExecuteTasks()
        {
            var context = Instance as UnitySynchronizationContext2;
            if (context != null)
                context.Exec();
        }

        private readonly struct WorkRequest
        {
            private readonly SendOrPostCallback m_DelegateCallback;
            private readonly object m_DelegateState;
            private readonly ManualResetEventSlim m_WaitHandle;
            private readonly ExecutionContext m_ExecutionContext;

            private static readonly ContextCallback s_ContextCallback = InvokeImpl;

            public WorkRequest(SendOrPostCallback callback, object state, ExecutionContext executionContext, ManualResetEventSlim waitHandle)
            {
                m_DelegateCallback = callback;
                m_DelegateState = state;
                m_WaitHandle = waitHandle;
                m_ExecutionContext = executionContext;
            }

            private static void InvokeImpl(object obj)
            {
                WorkRequest request = (WorkRequest)obj;
                request.m_DelegateCallback(request.m_DelegateState);
            }

            public void Invoke()
            {
                try
                {
                    if(m_ExecutionContext != null)
                        ExecutionContext.Run(m_ExecutionContext, s_ContextCallback, this);
                    else
                        m_DelegateCallback(m_DelegateState);
                }
                catch(Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
                finally
                {
                    m_WaitHandle?.Set();

                }
            }
        }

        private static HarmonyInstance _harmony = HarmonyInstance.Create("UnitySyncContextReplacer");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Replace()
        {
            if (Current.GetType() == typeof(UnitySynchronizationContext2))
                return;

            var unitySyncContextType = typeof(Application).Assembly.GetType("UnityEngine.UnitySynchronizationContext");
            var orig = unitySyncContextType.GetMethod(nameof(ExecuteTasks), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            if (orig == null)
            {
                UnityEngine.Debug.LogError("Cant find method Exec of UnitySynchronizationContext");
                return;
            }

            var replacement = new HarmonyMethod(typeof(UnitySynchronizationContext2).GetMethod(nameof(ExecuteTasks), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic));

            if( _harmony.Patch(orig, replacement, null) == null)
            {
                UnityEngine.Debug.LogError("Failed to patch method Exec of UnitySynchronizationContext");
                return;
            }

            var oldSyncContext = Current;

            Instance = new UnitySynchronizationContext2();
            SetSynchronizationContext(Instance);

            SyncContextUtil.UnitySynchronizationContextReplaced = Instance;
            AsyncStackHolder.UnityThreadId = Thread.CurrentThread.ManagedThreadId;

            if (oldSyncContext != null && unitySyncContextType.IsInstanceOfType(oldSyncContext))
            {                
                var execMethod = unitySyncContextType.GetMethod("Exec", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                var del = (Action)Delegate.CreateDelegate(typeof(Action), oldSyncContext, execMethod);
                Current.Post((_) => del(), null);
            }
        }
    }
}