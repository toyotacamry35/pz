using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace Assets.Src.ContainerApis
{
    public class HasStatsBroadcastApi : HasStatsApi<StatListenerBroadcast>
    {
        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var hasStatsClientBroadcast = wrapper as IHasStatsEngineClientBroadcast;
            if (hasStatsClientBroadcast.AssertIfNull(nameof(hasStatsClientBroadcast)))
                return;

            var stats = hasStatsClientBroadcast.Stats;
            if (stats.AssertIfNull(nameof(stats)))
                return;

            if (!stats.ProceduralStatsBroadcast.AssertIfNull(nameof(stats.ProceduralStatsBroadcast)))
            {
                stats.ProceduralStatsBroadcast.OnItemAddedOrUpdated += OnProceduralStatsAddedOrUpdated;
                stats.ProceduralStatsBroadcast.OnItemRemoved += OnProceduralStatsRemoved;
                foreach (var kvp in stats.ProceduralStatsBroadcast)
                    OnSomeStatAddUpdInternal(kvp.Key, kvp.Value, StatKind.Procedural, false);
            }


            if (!stats.TimeStatsBroadcast.AssertIfNull(nameof(stats.TimeStatsBroadcast)))
            {
                stats.TimeStatsBroadcast.OnItemAddedOrUpdated += OnTimeStatsAddedOrUpdated;
                stats.TimeStatsBroadcast.OnItemRemoved += OnTimeStatsRemoved;
                foreach (var kvp in stats.TimeStatsBroadcast)
                    OnSomeStatAddUpdInternal(kvp.Key, kvp.Value, StatKind.Time, false);
            }

            if (!stats.AccumulatedStatsBroadcast.AssertIfNull(nameof(stats.AccumulatedStatsBroadcast)))
            {
                stats.AccumulatedStatsBroadcast.OnItemAddedOrUpdated += OnAccumulatedStatsAddedOrUpdated;
                stats.AccumulatedStatsBroadcast.OnItemRemoved += OnAccumulatedStatsRemoved;
                foreach (var kvp in stats.AccumulatedStatsBroadcast)
                    OnSomeStatAddUpdInternal(kvp.Key, kvp.Value, StatKind.Accumulated, false);
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
            var hasStatsClientBroadcast = wrapper as IHasStatsEngineClientBroadcast;
            if (hasStatsClientBroadcast.AssertIfNull(nameof(hasStatsClientBroadcast)))
                return;

            var stats = hasStatsClientBroadcast.Stats;
            if (stats.AssertIfNull(nameof(stats)))
                return;

            if (stats.ProceduralStatsBroadcast != null)
            {
                stats.ProceduralStatsBroadcast.OnItemAddedOrUpdated -= OnProceduralStatsAddedOrUpdated;
                stats.ProceduralStatsBroadcast.OnItemRemoved -= OnProceduralStatsRemoved;

                foreach (var kvp in stats.ProceduralStatsBroadcast)
                    OnSomeStatRemoveInternal(kvp.Key, kvp.Value, StatKind.Procedural);
            }


            if (stats.TimeStatsBroadcast != null)
            {
                stats.TimeStatsBroadcast.OnItemAddedOrUpdated -= OnTimeStatsAddedOrUpdated;
                stats.TimeStatsBroadcast.OnItemRemoved -= OnTimeStatsRemoved;

                foreach (var kvp in stats.TimeStatsBroadcast)
                    OnSomeStatRemoveInternal(kvp.Key, kvp.Value, StatKind.Time);
            }

            if (stats.AccumulatedStatsBroadcast != null)
            {
                stats.AccumulatedStatsBroadcast.OnItemAddedOrUpdated -= OnAccumulatedStatsAddedOrUpdated;
                stats.AccumulatedStatsBroadcast.OnItemRemoved -= OnAccumulatedStatsRemoved;

                foreach (var kvp in stats.AccumulatedStatsBroadcast)
                    OnSomeStatRemoveInternal(kvp.Key, kvp.Value, StatKind.Time);
            }
        }


        //=== Private =========================================================

        private Task OnProceduralStatsAddedOrUpdated(DeltaDictionaryChangedEventArgs<StatResource, IProceduralStatClientBroadcast> args)
        {
            OnSomeStatAddUpdInternal(args.Key, args.Value, StatKind.Procedural);
            return Task.CompletedTask;
        }

        private Task OnProceduralStatsRemoved(DeltaDictionaryChangedEventArgs<StatResource, IProceduralStatClientBroadcast> args)
        {
            OnSomeStatRemoveInternal(args.Key, args.Value, StatKind.Procedural);
            return Task.CompletedTask;
        }

        private Task OnTimeStatsAddedOrUpdated(DeltaDictionaryChangedEventArgs<StatResource, ITimeStatClientBroadcast> args)
        {
            OnSomeStatAddUpdInternal(args.Key, args.Value, StatKind.Time);
            return Task.CompletedTask;
        }

        private Task OnTimeStatsRemoved(DeltaDictionaryChangedEventArgs<StatResource, ITimeStatClientBroadcast> args)
        {
            OnSomeStatRemoveInternal(args.Key, args.Value, StatKind.Time);
            return Task.CompletedTask;
        }

        private Task OnAccumulatedStatsAddedOrUpdated(DeltaDictionaryChangedEventArgs<StatResource, IAccumulatedStatClientBroadcast> args)
        {
            OnSomeStatAddUpdInternal(args.Key, args.Value, StatKind.Accumulated);
            return Task.CompletedTask;
        }

        private Task OnAccumulatedStatsRemoved(DeltaDictionaryChangedEventArgs<StatResource, IAccumulatedStatClientBroadcast> args)
        {
            OnSomeStatRemoveInternal(args.Key, args.Value, StatKind.Accumulated);
            return Task.CompletedTask;
        }
    }
}