using System.Collections.Generic;
using SharedCode.EntitySystem;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.EntitySystem
{
    public static class EventHelper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static string SubscribersToString(Delegate del)
        {
            var __sb__ = new System.Text.StringBuilder();
            if (del != null)
            {
                var __subscribers__ = del.GetInvocationList();
                __sb__.AppendFormat("{1}subscribers count: {0}", __subscribers__.Length, __subscribers__.Length >= ServerCoreRuntimeParameters.EventSubscribersWarnCount ? "WARN " : "").AppendLine();
                foreach (var subscriber in __subscribers__.Cast<PropertyChangedDelegate>())
                    __sb__.AppendFormat("<obj:{0} method:{1} hashcode:{2}>", subscriber.Target.GetType().Name, subscriber.Method.Name, subscriber.Target.GetHashCode());
            }

            if (__sb__.Length == 0)
                return null;
            return __sb__.ToString();
        }

        private static async Task RunSubscriber(PropertyChangedDelegate subscriber, EntityEventArgs args)
        {
            try
            {
                using (var cts = new CancellationTokenSource())
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntityEventTimeoutSeconds), cts.Token);
                    var subscriberTask = subscriber(args);
                    await Task.WhenAny(subscriberTask, timeoutTask);
                    if (!subscriberTask.IsCompleted)
                    {
                        Logger.IfError()?.Message("RunSubscriber process event obj {0} method {1} timeout: {2}", subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds).Write();
                        var __sw__ = new System.Diagnostics.Stopwatch();
                        __sw__.Start();
                        await subscriberTask;
                        __sw__.Stop();
                        Logger.IfError()?.Message("RunSubscriber process event obj {0} method {1} executing too long: {2}", subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds + __sw__.Elapsed.TotalSeconds).Write();
                    }
                    else
                    {
                        cts.Cancel();
                        if (subscriberTask.IsFaulted)
                            Logger.IfError()?.Message(subscriberTask.Exception, "RunSubscriber faulted process event obj {0} method {1}", subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception event obj {0} method {1}", subscriber?.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
            }
        }

        public static void FireEvent(List<Func<Task>> container, EntityEventArgs args, PropertyChangedDelegate ev)
        {
            foreach (var subscriber in ev.GetInvocationList())
            {
                container.Add(() => RunSubscriber((PropertyChangedDelegate)subscriber, args));
            }

        }

    }
}
