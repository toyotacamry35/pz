using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;

namespace Assets.Src.ContainerApis
{
    public class StatListenerFull : StatListenerBroadcast
    {
        //=== Public ======================================================

        /// <summary>
        /// Высавляет значения и подписывается, если не подписан
        /// </summary>
        public override void SetAndSubscribeOnly(IDeltaObject deltaObject, StatKind statKind)
        {
            switch (statKind)
            {
                case StatKind.Procedural:
                    var proceduralStatClientFull = (IProceduralStatClientFull) deltaObject;
                    if (proceduralStatClientFull.AssertIfNull(nameof(proceduralStatClientFull)))
                        return;

                    Changes = new AnyStatState(statKind, proceduralStatClientFull.LimitMinCache,
                        proceduralStatClientFull.LimitMaxCache, proceduralStatClientFull.ValueCache); //set

                    if (!IsSubscribed)
                    {
                        proceduralStatClientFull.SubscribePropertyChanged(
                            nameof(IProceduralStatClientFull.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        proceduralStatClientFull.SubscribePropertyChanged(
                            nameof(IProceduralStatClientFull.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        proceduralStatClientFull.SubscribePropertyChanged(
                            nameof(IProceduralStatClientFull.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Proxy:
                    if (deltaObject is IProxyStatClientFull proxyStatClientFull)
                    {
                        Changes = new AnyStatState(statKind, proxyStatClientFull.LimitMinCache,
                            proxyStatClientFull.LimitMaxCache, proxyStatClientFull.ValueCache); //set

                        if (!IsSubscribed)
                        {
                            proxyStatClientFull.SubscribePropertyChanged(
                                nameof(IProceduralStatClientFull.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                            proxyStatClientFull.SubscribePropertyChanged(
                                nameof(IProceduralStatClientFull.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                            proxyStatClientFull.SubscribePropertyChanged(
                                nameof(IProceduralStatClientFull.ValueCache), OnSomeStatValueCacheChanged);
                        }
                    }
                    else
                    {
                        Logger.IfError()?.Message($"{nameof(deltaObject)} of unexpected type: {deltaObject?.GetType().NiceName()}").Write();
                    }

                    break;

                case StatKind.Accumulated:
                    var accumulatedStatClientFull = (IAccumulatedStatClientFull) deltaObject;
                    if (accumulatedStatClientFull.AssertIfNull(nameof(accumulatedStatClientFull)))
                        return;

                    Changes = new AnyStatState(statKind, accumulatedStatClientFull.LimitMinCache,
                        accumulatedStatClientFull.LimitMaxCache, accumulatedStatClientFull.ValueCache); //set

                    if (!IsSubscribed)
                    {
                        accumulatedStatClientFull.SubscribePropertyChanged(
                            nameof(IAccumulatedStatClientFull.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        accumulatedStatClientFull.SubscribePropertyChanged(
                            nameof(IAccumulatedStatClientFull.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        accumulatedStatClientFull.SubscribePropertyChanged(
                            nameof(IAccumulatedStatClientFull.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Time:
                    var timeStatClientFull = (ITimeStatClientFull) deltaObject;
                    if (timeStatClientFull.AssertIfNull(nameof(timeStatClientFull)))
                        return;

                    Changes = new AnyStatState(statKind, timeStatClientFull.LimitMinCache,
                        timeStatClientFull.LimitMaxCache, timeStatClientFull.State); //set

                    if (!IsSubscribed)
                    {
                        timeStatClientFull.SubscribePropertyChanged(
                            nameof(ITimeStatClientFull.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        timeStatClientFull.SubscribePropertyChanged(
                            nameof(ITimeStatClientFull.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        timeStatClientFull.SubscribePropertyChanged(
                            nameof(ITimeStatClientFull.State), OnTimeStatStateChanged);
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

        public override void UnsubscribeAndReset(IDeltaObject deltaObject, StatKind statKind)
        {
            //UI.CallerLog($"kind={statKind}, <{deltaObject?.GetType()}>  -- {this}"); //DEBUG
            switch (statKind)
            {
                case StatKind.Procedural:
                    var proceduralStatClientFull = (IProceduralStatClientFull) deltaObject;
                    if (proceduralStatClientFull.AssertIfNull(nameof(proceduralStatClientFull)))
                        return;

                    Changes = new AnyStatState(statKind, proceduralStatClientFull.LimitMinCache,
                        proceduralStatClientFull.LimitMaxCache, proceduralStatClientFull.ValueCache); //set

                    if (IsSubscribed)
                    {
                        proceduralStatClientFull.UnsubscribePropertyChanged(
                            nameof(IProceduralStatClientFull.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        proceduralStatClientFull.UnsubscribePropertyChanged(
                            nameof(IProceduralStatClientFull.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        proceduralStatClientFull.UnsubscribePropertyChanged(
                            nameof(IProceduralStatClientFull.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Accumulated:
                    var accumulatedStatClientFull = (IAccumulatedStatClientFull) deltaObject;
                    if (accumulatedStatClientFull.AssertIfNull(nameof(accumulatedStatClientFull)))
                        return;

                    Changes = new AnyStatState(statKind, accumulatedStatClientFull.LimitMinCache,
                        accumulatedStatClientFull.LimitMaxCache, accumulatedStatClientFull.ValueCache); //set

                    if (IsSubscribed)
                    {
                        accumulatedStatClientFull.UnsubscribePropertyChanged(
                            nameof(IAccumulatedStatClientFull.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        accumulatedStatClientFull.UnsubscribePropertyChanged(
                            nameof(IAccumulatedStatClientFull.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        accumulatedStatClientFull.UnsubscribePropertyChanged(
                            nameof(IAccumulatedStatClientFull.ValueCache), OnSomeStatValueCacheChanged);
                    }

                    break;

                case StatKind.Time:
                    var timeStatClientFull = (ITimeStatClientFull) deltaObject;
                    if (timeStatClientFull.AssertIfNull(nameof(timeStatClientFull)))
                        return;

                    Changes = new AnyStatState(statKind, timeStatClientFull.LimitMinCache,
                        timeStatClientFull.LimitMaxCache, timeStatClientFull.State); //set

                    if (IsSubscribed)
                    {
                        timeStatClientFull.UnsubscribePropertyChanged(
                            nameof(ITimeStatClientFull.LimitMinCache), OnSomeStatLimitMinCacheChanged);
                        timeStatClientFull.UnsubscribePropertyChanged(
                            nameof(ITimeStatClientFull.LimitMaxCache), OnSomeStatLimitMaxCacheChanged);
                        timeStatClientFull.UnsubscribePropertyChanged(
                            nameof(ITimeStatClientFull.State), OnTimeStatStateChanged);
                    }

                    break;

                default:
                    Logger.IfError()?.Message($"{nameof(UnsubscribeAndReset)} Unhandled kind: {statKind}").Write();
                    return;
            }

            IsSubscribed = false;
        }


        public override string ToString() //TODOM
        {
            return $"({nameof(StatListenerFull)}[{Id}.{StatResource.____GetDebugRootName()}] " +
                   $"{nameof(Current)}={Current}, {nameof(Changes)}={Changes}, {nameof(IsSubscribed)}{IsSubscribed.AsSign()}, " +
                   $"{nameof(HasChanges)}{HasChanges.AsSign()})";
        }
    }
}