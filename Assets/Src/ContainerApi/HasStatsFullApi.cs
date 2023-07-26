using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace Assets.Src.ContainerApis
{
    public class HasStatsFullApi : HasStatsApi<StatListenerFull>
    {
        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var hasStatsClientFull = wrapper as IHasStatsEngineClientFull;
            if (hasStatsClientFull.AssertIfNull(nameof(hasStatsClientFull)))
                return;

            var stats = hasStatsClientFull.Stats;
            if (stats.AssertIfNull(nameof(stats)))
                return;

            if (!stats.ProceduralStats.AssertIfNull(nameof(stats.ProceduralStats)))
            {
                stats.ProceduralStats.OnItemAddedOrUpdated += OnProceduralStatsAddedOrUpdated;
                stats.ProceduralStats.OnItemRemoved += OnProceduralStatsRemoved;
                foreach (var kvp in stats.ProceduralStats)
                    OnSomeStatAddUpdInternal(kvp.Key, kvp.Value, StatKind.Procedural, false);
            }


            if (!stats.TimeStats.AssertIfNull(nameof(stats.TimeStats)))
            {
                stats.TimeStats.OnItemAddedOrUpdated += OnTimeStatsAddedOrUpdated;
                stats.TimeStats.OnItemRemoved += OnTimeStatsRemoved;
                foreach (var kvp in stats.TimeStats)
                    OnSomeStatAddUpdInternal(kvp.Key, kvp.Value, StatKind.Time, false);
            }


            if (!stats.AccumulatedStats.AssertIfNull(nameof(stats.AccumulatedStats)))
            {
                stats.AccumulatedStats.OnItemAddedOrUpdated += OnAccumulatedStatsAddedOrUpdated;
                stats.AccumulatedStats.OnItemRemoved += OnAccumulatedStatsRemoved;
                foreach (var kvp in stats.AccumulatedStats)
                    OnSomeStatAddUpdInternal(kvp.Key, kvp.Value, StatKind.Accumulated, false);
            }

            if (!stats.ProxyStats.AssertIfNull(nameof(stats.ProxyStats)))
            {
                stats.ProxyStats.OnItemAddedOrUpdated += OnProxyStatsAddedOrUpdated;
                stats.ProxyStats.OnItemRemoved += OnProxyStatsRemoved;
                foreach (var kvp in stats.ProxyStats)
                    OnSomeStatAddUpdInternal(kvp.Key, kvp.Value, StatKind.Proxy, false);
            }

            //Безусловно все раздаем
            foreach (var listener in Listeners.Values)
            {
                listener.TakeChanges(); //Current еще не содержит никаких данных
                listener.FireOnSlotItemChanged(true);
            }
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var hasStatsClientFull = wrapper as IHasStatsEngineClientFull;
            if (hasStatsClientFull.AssertIfNull(nameof(hasStatsClientFull)))
                return;

            var stats = hasStatsClientFull.Stats;
            if (stats.AssertIfNull(nameof(stats)))
                return;

            if (stats.ProceduralStats != null)
            {
                stats.ProceduralStats.OnItemAddedOrUpdated -= OnProceduralStatsAddedOrUpdated;
                stats.ProceduralStats.OnItemRemoved -= OnProceduralStatsRemoved;

                foreach (var kvp in stats.ProceduralStats)
                    OnSomeStatRemoveInternal(kvp.Key, kvp.Value, StatKind.Procedural);
            }


            if (stats.TimeStats != null)
            {
                stats.TimeStats.OnItemAddedOrUpdated -= OnTimeStatsAddedOrUpdated;
                stats.TimeStats.OnItemRemoved -= OnTimeStatsRemoved;

                foreach (var kvp in stats.TimeStats)
                    OnSomeStatRemoveInternal(kvp.Key, kvp.Value, StatKind.Time);
            }


            if (stats.AccumulatedStats != null)
            {
                stats.AccumulatedStats.OnItemAddedOrUpdated -= OnAccumulatedStatsAddedOrUpdated;
                stats.AccumulatedStats.OnItemRemoved -= OnAccumulatedStatsRemoved;

                foreach (var kvp in stats.AccumulatedStats)
                    OnSomeStatRemoveInternal(kvp.Key, kvp.Value, StatKind.Accumulated);
            }

            if (stats.ProxyStats != null)
            {
                stats.ProxyStats.OnItemAddedOrUpdated -= OnProxyStatsAddedOrUpdated;
                stats.ProxyStats.OnItemRemoved -= OnProxyStatsRemoved;

                foreach (var kvp in stats.ProxyStats)
                    OnSomeStatRemoveInternal(kvp.Key, kvp.Value, StatKind.Proxy);
            }
        }


        //=== Private =========================================================

        private Task OnProceduralStatsAddedOrUpdated(DeltaDictionaryChangedEventArgs<StatResource, IProceduralStatClientFull> args)
        {
            OnSomeStatAddUpdInternal(args.Key, args.Value, StatKind.Procedural);
            return Task.CompletedTask;
        }

        private Task OnProceduralStatsRemoved(DeltaDictionaryChangedEventArgs<StatResource, IProceduralStatClientFull> args)
        {
            OnSomeStatRemoveInternal(args.Key, args.Value, StatKind.Procedural);
            return Task.CompletedTask;
        }

        private Task OnTimeStatsAddedOrUpdated(DeltaDictionaryChangedEventArgs<StatResource, ITimeStatClientFull> args)
        {
            OnSomeStatAddUpdInternal(args.Key, args.Value, StatKind.Time);
            return Task.CompletedTask;
        }

        private Task OnTimeStatsRemoved(DeltaDictionaryChangedEventArgs<StatResource, ITimeStatClientFull> args)
        {
            OnSomeStatRemoveInternal(args.Key, args.Value, StatKind.Time);
            return Task.CompletedTask;
        }

        private Task OnAccumulatedStatsAddedOrUpdated(DeltaDictionaryChangedEventArgs<StatResource, IAccumulatedStatClientFull> args)
        {
            OnSomeStatAddUpdInternal(args.Key, args.Value, StatKind.Accumulated);
            return Task.CompletedTask;
        }

        private Task OnAccumulatedStatsRemoved(DeltaDictionaryChangedEventArgs<StatResource, IAccumulatedStatClientFull> args)
        {
            OnSomeStatRemoveInternal(args.Key, args.Value, StatKind.Accumulated);
            return Task.CompletedTask;
        }

        private Task OnProxyStatsAddedOrUpdated(DeltaDictionaryChangedEventArgs<StatResource, IProxyStatClientFull> args)
        {
            OnSomeStatAddUpdInternal(args.Key, args.Value, StatKind.Proxy);
            return Task.CompletedTask;
        }

        private Task OnProxyStatsRemoved(DeltaDictionaryChangedEventArgs<StatResource, IProxyStatClientFull> args)
        {
            OnSomeStatRemoveInternal(args.Key, args.Value, StatKind.Proxy);
            return Task.CompletedTask;
        }
    }
}