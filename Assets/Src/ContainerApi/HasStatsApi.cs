using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects.Impl.Stats;
using SharedCode.EntitySystem;
using Uins;

namespace Assets.Src.ContainerApis
{
    public delegate void StatChangedDelegate(StatResource statResource, AnyStatState anyStatState);

    public abstract class HasStatsApi<T> : EntityApi where T : StatListenerBroadcast, new()
    {
        protected ConcurrentDictionary<StatResource, T> Listeners = new ConcurrentDictionary<StatResource, T>();


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientBroadcast;


        //=== Public ==========================================================

        /// <summary>
        /// Список всех имеющихся статов типа statKind
        /// </summary>
        /// <param name="statKind">Фильтр. Если None - то все</param>
        public IEnumerable<StatResource> GetStatResourcesOfStatKind(StatKind statKind)
        {
            return GetStatResourcesOfStatKindRequest(statKind);
        }

        public void SubscribeToStats(StatResource statResource, StatChangedDelegate onStatChanged)
        {
            if (onStatChanged.AssertIfNull(nameof(onStatChanged)) ||
                statResource.AssertIfNull(nameof(statResource)))
                return;

            SubscribeToStatsRequest(statResource, onStatChanged);
        }

        public void UnsubscribeFromStats(StatResource statResource, StatChangedDelegate onStatChanged)
        {
            if (onStatChanged.AssertIfNull(nameof(onStatChanged)) ||
                statResource.AssertIfNull(nameof(statResource)))
                return;

            UnsubscribeFromStatsRequest(statResource, onStatChanged);
        }


        //=== Protected =======================================================

        protected void SubscribeToStatsRequest(StatResource statResource, StatChangedDelegate onStatChanged)
        {
            var statListener = GetOrCreateStatListener(statResource);
            if (statListener == null)
            {
                Logger.Error(
                    $"Subscribe: Not found {nameof(statListener)} for {nameof(statResource)}={statResource.____GetDebugRootName()}");
                return;
            }

            statListener.StatValueChanged += onStatChanged;
            if (IsSubscribedSuccessfully)
                onStatChanged.Invoke(statResource, statListener.Current);
        }

        protected void UnsubscribeFromStatsRequest(StatResource statResource, StatChangedDelegate onStatChanged)
        {
            var statListener = SearchForExistsStatListener(statResource);
            if (statListener == null)
            {
                Logger.Error(
                    $"Unsubscribe: Not found {nameof(statListener)} for {nameof(statResource)}={statResource.____GetDebugRootName()}");
                return;
            }

            statListener.StatValueChanged -= onStatChanged;
        }

        protected IEnumerable<StatResource> GetStatResourcesOfStatKindRequest(StatKind statKind)
        {
            if (!IsSubscribedSuccessfully)
                return Enumerable.Empty<StatResource>();

            return statKind == StatKind.None
                ? Listeners.Keys
                : Listeners.Where(kvp => kvp.Value.Changes.Kind == statKind).Select(kvp => kvp.Key);
        }

        protected void OnSomeStatAddUpdInternal(StatResource statResource, IDeltaObject deltaObject, StatKind statKind,
            bool doEventCall = true)
        {
            var listener = GetOrCreateStatListener(statResource);
            if (listener.AssertIfNull(nameof(listener)))
                return;

            listener.SetAndSubscribeOnly(deltaObject, statKind);

            if (doEventCall && listener.HasChanges)
                listener.FireOnSlotItemChanged();
        }

        protected void OnSomeStatRemoveInternal(StatResource statResource, IDeltaObject deltaObject, StatKind statKind)
        {
            T listener = null;
            lock (Listeners)
            {
                if (!Listeners.TryGetValue(statResource, out listener))
                {
                    UI.Logger.Error(
                        $"{nameof(Listeners.TryGetValue)} {typeof(T).NiceName()}[{statResource.____GetDebugRootName()}] FAIL");
                    return;
                }
            }

            listener.UnsubscribeAndReset(deltaObject, statKind);
            if (listener.HasChanges)
                listener.FireOnSlotItemChanged();
        }

        public T SearchForExistsStatListener(StatResource statResource)
        {
            return GetOrCreateStatListener(statResource, false);
        }


        //=== Private =========================================================

        private T GetOrCreateStatListener(StatResource statResource, bool createIfNotExists = true)
        {
            T statListener;
            lock (Listeners)
            {
                if (!Listeners.TryGetValue(statResource, out statListener))
                {
                    if (createIfNotExists)
                    {
                        statListener = new T();
                        statListener.SetStatResource(statResource);
                        if (!Listeners.TryAdd(statResource, statListener))
                            UI.Logger.Error(
                                $"{nameof(Listeners.TryAdd)} new {typeof(T).NiceName()}[{statResource.____GetDebugRootName()}] FAIL");
                    }
                }
            }

            return statListener;
        }
    }
}