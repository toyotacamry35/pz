using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Quests;
using SharedCode.Serializers;
using SharedCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class StatisticEngine : IHasStatisticsRouterInternal
    {
        IDictionary<Type, IStatisticRouter> IHasStatisticsRouterInternal.Routers => _routers;
        IDictionary<Type, IStatisticRouter> _routers;

        protected override void constructor()
        {
            base.constructor();
            _routers = new ConcurrentDictionary<Type, IStatisticRouter>();
            StatisticsEvent += StatisticEngine_StatisticsEvent;
        }

        public async Task PostStatisticsEventImpl(StatisticEventArgs arg)
        {
            await OnStatisticsEventInvoke(arg);
        }

        private async Task StatisticEngine_StatisticsEvent(StatisticEventArgs arg)
        {
            var keyType = arg.GetType();
            IStatisticRouter router;
            if (!_routers.TryGetValue(keyType, out router))
                return;
            if(router != null)
                await router.InvokeRoutedEvent(arg, EntitiesRepository);
        }
    }

    public static class StatisticEngineExtensions
    {
        public static Task<StatisticRouter<T>> GetStatisticRouter<T>(this IStatisticEngineServer engine) where T : StatisticEventArgs
        {
            var keyType = typeof(T);
            var internalEngine = ((IHasStatisticsRouterInternal)engine.GetBaseDeltaObject());
            IStatisticRouter router;

            if (!internalEngine.Routers.TryGetValue(keyType, out router))
            {
                router = new StatisticRouter<T>();
                internalEngine.Routers.Add(keyType, router);
            }
            return Task.FromResult((StatisticRouter<T>)router);
        }
    }

    public class StatisticRouter<T> : IStatisticRouter where T : StatisticEventArgs
    {
        public event Func<T, Task> RoutedEvent;

        private IDictionary<Func<T, Task>, object> _subscribers = new ConcurrentDictionary<Func<T, Task>, object>();

        public Task Subscribe(Func<T, Task> callback)
        {
            if (!_subscribers.ContainsKey(callback))
                _subscribers.Add(callback, null);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(Func<T, Task> callback)
        {
            if (_subscribers.ContainsKey(callback))
                _subscribers.Remove(callback);
            return Task.CompletedTask;
        }

        async Task IStatisticRouter.InvokeRoutedEvent(StatisticEventArgs arg, IEntitiesRepository repo)
        {
            foreach(var subscriber in _subscribers.Keys)
                AsyncUtils.RunAsyncTask(() => subscriber.InvokeAsync((T)arg), repo);

            await RoutedEvent.InvokeAsync((T)arg);
        }
    }
}
