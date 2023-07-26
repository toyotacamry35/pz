using System;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using Src.Aspects.Impl.Stats.Proxy;
using Uins;

namespace Assets.Src.ContainerApis
{
    public class StatListenerBroadcast
    {
        protected static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        public event StatChangedDelegate StatValueChanged;

        protected static long Counter = 0;


        //=== Props =======================================================

        public readonly long Id;

        public StatResource StatResource { get; private set; }

        public bool IsSubscribed { get; protected set; }

        /// <summary>
        /// Текущее значение, раздаваемое подписчикам
        /// </summary>
        public AnyStatState Current { get; protected set; }

        /// <summary>
        /// Обновления
        /// </summary>
        public AnyStatState Changes { get; protected set; }

        public bool HasChanges => !Current.Equals(Changes);


        //=== Ctor ========================================================

        public StatListenerBroadcast()
        {
            Id = Counter++;
        }


        //=== Public ======================================================

        public void SetStatResource(StatResource statResource)
        {
            StatResource = statResource;
        }

        /// <summary>
        /// Высавляет значения и подписывается, если не подписан
        /// </summary>
        public virtual void SetAndSubscribeOnly(IDeltaObject deltaObject, StatKind statKind)
        {
            switch (statKind)
            {
                case StatKind.Procedural:
                    var proceduralStatClientBroadcast = (IProceduralStatClientBroadcast) deltaObject;
                    if (proceduralStatClientBroadcast.AssertIfNull(nameof(proceduralStatClientBroadcast)))
                        return;

                    Changes = new AnyStatState(statKind, proceduralStatClientBroadcast.LimitMinCache,
                        proceduralStatClientBroadcast.LimitMaxCache, proceduralStatClientBroadcast.ValueCache); //set

                    if (!IsSubscribed)
                    {
                        proceduralStatClientBroadcast.SubscribePropertyChanged(
                            nameof(IProceduralStatClientBroadcast.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        proceduralStatClientBroadcast.SubscribePropertyChanged(
                            nameof(IProceduralStatClientBroadcast.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        proceduralStatClientBroadcast.SubscribePropertyChanged(
                            nameof(IProceduralStatClientBroadcast.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Accumulated:
                    var accumulatedStatClientBroadcast = (IAccumulatedStatClientBroadcast) deltaObject;
                    if (accumulatedStatClientBroadcast.AssertIfNull(nameof(accumulatedStatClientBroadcast)))
                        return;

                    Changes = new AnyStatState(statKind, accumulatedStatClientBroadcast.LimitMinCache,
                        accumulatedStatClientBroadcast.LimitMaxCache, accumulatedStatClientBroadcast.ValueCache); //set

                    if (!IsSubscribed)
                    {
                        accumulatedStatClientBroadcast.SubscribePropertyChanged(
                            nameof(IAccumulatedStatClientBroadcast.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        accumulatedStatClientBroadcast.SubscribePropertyChanged(
                            nameof(IAccumulatedStatClientBroadcast.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        accumulatedStatClientBroadcast.SubscribePropertyChanged(
                            nameof(IAccumulatedStatClientBroadcast.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Time:
                    var timeStatClientBroadcast = (ITimeStatClientBroadcast) deltaObject;
                    if (timeStatClientBroadcast.AssertIfNull(nameof(timeStatClientBroadcast)))
                        return;

                    Changes = new AnyStatState(statKind, timeStatClientBroadcast.LimitMinCache,
                        timeStatClientBroadcast.LimitMaxCache, timeStatClientBroadcast.State); //set

                    if (!IsSubscribed)
                    {
                        timeStatClientBroadcast.SubscribePropertyChanged(
                            nameof(ITimeStatClientBroadcast.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        timeStatClientBroadcast.SubscribePropertyChanged(
                            nameof(ITimeStatClientBroadcast.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        timeStatClientBroadcast.SubscribePropertyChanged(
                            nameof(ITimeStatClientBroadcast.State), OnTimeStatStateChanged);
                    }

                    break;

                default:
                    Logger.IfError()?.Message($"{nameof(SetAndSubscribeOnly)} Unhandled kind: {statKind}").Write();
                    return;
            }

            IsSubscribed = true;
            Current = Current.SetKind(statKind);
            //UI.CallerLog($"kind={statKind} <{deltaObject?.GetType()}>  -- {this}"); //DEBUG
        }

        public virtual void UnsubscribeAndReset(IDeltaObject deltaObject, StatKind statKind)
        {
            //UI.CallerLog($"kind={statKind}, <{deltaObject?.GetType()}>  -- {this}"); //DEBUG
            switch (statKind)
            {
                case StatKind.Procedural:
                    var proceduralStatClientBroadcast = (IProceduralStatClientBroadcast) deltaObject;
                    if (proceduralStatClientBroadcast.AssertIfNull(nameof(proceduralStatClientBroadcast)))
                        return;

                    Changes = new AnyStatState(statKind, proceduralStatClientBroadcast.LimitMinCache,
                        proceduralStatClientBroadcast.LimitMaxCache, proceduralStatClientBroadcast.ValueCache); //set

                    if (IsSubscribed)
                    {
                        proceduralStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(IProceduralStatClientBroadcast.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        proceduralStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(IProceduralStatClientBroadcast.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        proceduralStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(IProceduralStatClientBroadcast.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Accumulated:
                    var accumulatedStatClientBroadcast = (IAccumulatedStatClientBroadcast) deltaObject;
                    if (accumulatedStatClientBroadcast.AssertIfNull(nameof(accumulatedStatClientBroadcast)))
                        return;

                    Changes = new AnyStatState(statKind, accumulatedStatClientBroadcast.LimitMinCache,
                        accumulatedStatClientBroadcast.LimitMaxCache, accumulatedStatClientBroadcast.ValueCache); //set

                    if (IsSubscribed)
                    {
                        accumulatedStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(IAccumulatedStatClientBroadcast.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        accumulatedStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(IAccumulatedStatClientBroadcast.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        accumulatedStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(IAccumulatedStatClientBroadcast.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Time:
                    var timeStatClientBroadcast = (ITimeStatClientBroadcast) deltaObject;
                    if (timeStatClientBroadcast.AssertIfNull(nameof(timeStatClientBroadcast)))
                        return;

                    Changes = new AnyStatState(statKind, timeStatClientBroadcast.LimitMinCache,
                        timeStatClientBroadcast.LimitMaxCache, timeStatClientBroadcast.State); //set

                    if (IsSubscribed)
                    {
                        timeStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(ITimeStatClientBroadcast.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        timeStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(ITimeStatClientBroadcast.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        timeStatClientBroadcast.UnsubscribePropertyChanged(
                            nameof(ITimeStatClientBroadcast.State), OnTimeStatStateChanged);
                    }

                    break;

                default:
                    Logger.IfError()?.Message($"{nameof(UnsubscribeAndReset)} Unhandled kind: {statKind}").Write();
                    return;
            }

            IsSubscribed = false;
        }

        /// <summary>
        /// Безусловно отправляет сообщение об изменениях
        /// </summary>
        public void FireOnSlotItemChanged(bool isForced = false)
        {
            if (StatValueChanged == null && HasChanges)
            {
                TakeChanges();
                return;
            }

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (StatValueChanged != null && (HasChanges || isForced))
                {
                    //UI.Logger.IfInfo()?.Message($"{nameof(FireOnSlotItemChanged)}(isForced{isForced.AsSign()}) {this}").Write(); //DEBUG
                    TakeChanges();
                    var invocationList = StatValueChanged.GetInvocationList();
                    for (int i = 0; i < invocationList.Length; i++)
                    {
                        try
                        {
                            var changedDelegate = (StatChangedDelegate) invocationList[i];
                            changedDelegate(StatResource, Current);
                        }
                        catch (Exception e)
                        {
                            UI.Logger.IfError()?.Message($"Exception: {e}\n{this}").Write();
                        }
                    }
                }
            });
        }

        public void TakeChanges()
        {
            Current = Changes;
        }

        public override string ToString()
        {
            return $"({nameof(StatListenerBroadcast)}[{Id}.{StatResource.____GetDebugRootName()}] " +
                   $"{nameof(Current)}={Current}, {nameof(Changes)}={Changes}, {nameof(IsSubscribed)}{IsSubscribed.AsSign()}, " +
                   $"{nameof(HasChanges)}{HasChanges.AsSign()})";
        }

        //=== Protected =======================================================

        protected Task OnSomeStatLimitMinCacheChanged(EntityEventArgs args)
        {
            var newMinCache = (float) args.NewValue;
            //                UI.CallerLog($"{newMinCache} -- {this}"); //DEBUG
            Changes = Changes.SetMin(newMinCache);

            if (HasChanges)
                FireOnSlotItemChanged();

            return Task.CompletedTask;
        }

        protected Task OnSomeStatLimitMaxCacheChanged(EntityEventArgs args)
        {
            var newMaxCache = (float) args.NewValue;
            //                UI.CallerLog($"{newMaxCache} -- {this}"); //DEBUG
            Changes = Changes.SetMax(newMaxCache);

            if (HasChanges)
                FireOnSlotItemChanged();

            return Task.CompletedTask;
        }

        protected Task OnTimeStatStateChanged(EntityEventArgs args)
        {
            var newState = (TimeStatState) args.NewValue;
            //                UI.CallerLog($"{newState.ToDebug()} -- {this}"); //DEBUG
            Changes = Changes.SetTimeStatState(newState);
            //                UI.CallerLog($"after apply: {nameof(HasChanges)}{HasChanges.AsSign()}   {this}"); //DEBUG
            if (HasChanges)
                FireOnSlotItemChanged();

            return Task.CompletedTask;
        }

        protected Task OnSomeStatValueCacheChanged(EntityEventArgs args)
        {
            var newValueCache = (float) args.NewValue;
            //                UI.CallerLog($"{newValueCache} -- {this}"); //DEBUG
            Changes = Changes.SetVal(newValueCache);

            if (HasChanges)
                FireOnSlotItemChanged();

            return Task.CompletedTask;
        }
    }
}