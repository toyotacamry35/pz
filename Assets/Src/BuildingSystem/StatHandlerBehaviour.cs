using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Src.Aspects.Impl.Stats;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.Src.BuildingSystem
{
    public abstract class StatHandlerBehaviour : EntityGameObjectComponent
    {
        protected NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public JdbMetadata StatResource = null;

        protected abstract void OnAwakeHandler();
        protected abstract void OnDestroyHandler();
        protected abstract void OnGotClient();
        protected abstract void OnLostClient();
        protected abstract void OnStatChanged(float newValue);
        protected abstract void OnSetInitialValue(float initialValue);
        protected abstract void OnSetFinalValue(float finalValue);

        private StatResource GetStatResource()
        {
            return StatResource?.Get<StatResource>() ?? null;
        }

        void Awake()
        {
            OnAwakeHandler();
        }

        protected override void DestroyInternal()
        {
            OnDestroyHandler();
        }

        protected override void GotClient()
        {
            var capturedClientRepo = ClientRepo;
            var capturedStatResource = GetStatResource();
            if (capturedStatResource == null)
            {
                 Logger.IfError()?.Message("StatHandlerBehaviour.GotClient() StatResource is null").Write();;
                return;
            }
            OnGotClient();
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await capturedClientRepo.Get(TypeId, EntityId))
                {
                    var _entity = wrapper.Get<IHasStatsEngineClientBroadcast>(TypeId, EntityId, ReplicationLevel.ClientBroadcast);
                    if (_entity == null)
                    {
                         Logger.IfError()?.Message("StatHandlerBehaviour.GotClient() can't access to entity").Write();;
                        return;
                    }
                    var _entityStat = await _entity.Stats.GetBroadcastStat(capturedStatResource);
                    if (_entityStat == null)
                    {
                         Logger.IfError()?.Message("StatHandlerBehaviour.GotClient() can't access to entity stat").Write();;
                        return;
                    }
                    var _accStat = _entityStat as IAccumulatedStat;
                    if (_accStat != default)
                    {
                        //_entityStat.StatChanged += OnStatChangedAsync;
                        _accStat.SubscribePropertyChanged(nameof(IAccumulatedStat.ValueCache), OnStatChangedAsync);
                        var capturedInitialValue = _accStat.ValueCache;
                        await UnityQueueHelper.RunInUnityThread(() =>
                        {
                            if ((this != null) && IsClient)
                            {
                                OnSetInitialValue(capturedInitialValue);
                            }
                        });
                    }
                }
            });
        }

        protected override void LostClient()
        {
            var capturedClientRepo = ClientRepo;
            var capturedStatResource = GetStatResource();
            if (capturedStatResource == null)
            {
                 Logger.IfError()?.Message("StatHandlerBehaviour.LostClient() StatResource is null").Write();;
                return;
            }
            OnLostClient();
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await capturedClientRepo.Get(TypeId, EntityId))
                {
                    var _entity = wrapper.Get<IHasStatsEngineClientBroadcast>(TypeId, EntityId, ReplicationLevel.ClientBroadcast);
                    if (_entity == null)
                    {
                         Logger.IfError()?.Message("StatHandlerBehaviour.LostClient() can't access to entity").Write();;
                        return;
                    }
                    var _entityStat = await _entity.Stats.GetBroadcastStat(capturedStatResource);
                    if (_entityStat == null)
                    {
                         Logger.IfError()?.Message("StatHandlerBehaviour.LostClient() can't access to entity stat").Write();;
                        return;
                    }
                    var _accStat = _entityStat as IAccumulatedStat;
                    if (_accStat != default)
                    {
                        //_entityStat.StatChanged += OnStatChangedAsync;
                        _accStat.UnsubscribePropertyChanged(nameof(IAccumulatedStat.ValueCache), OnStatChangedAsync);
                        var capturedInitialValue = _accStat.ValueCache;
                        await UnityQueueHelper.RunInUnityThread(() =>
                        {
                            if ((this != null) && IsClient)
                            {
                                OnSetFinalValue(capturedInitialValue);
                            }
                        });
                    }
                }
            });
        }

        private async Task OnStatChangedAsync(EntityEventArgs args)
        {
            var capturedNewValue = (float)(args.NewValue);
            await UnityQueueHelper.RunInUnityThread(() =>
            {
                OnStatChanged(capturedNewValue);
            });
        }
    }
}