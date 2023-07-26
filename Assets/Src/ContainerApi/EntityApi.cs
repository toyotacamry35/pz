using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Shared.Impl;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Uins;

namespace Assets.Src.ContainerApis
{
    public class EntityApi : BindingVmodel
    {
        public const string NullEntityErrMsg = "wrapper/entity is null";
        public const int MinTotalMillisecondsForWarning = 100;

        /// <summary>
        /// Successfully got first wrapper at least.
        /// Is needed 'cos there're attempts to access from UI bindings, which 're executed before is initialized
        /// </summary>
        protected bool IsSubscribedSuccessfully;

        protected static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        private string _name; //TODOM заменить на уникальное имя из стека и id


        //=== Props ===========================================================

        protected Guid EntityGuid { get; private set; }

        protected int EntityTypeId { get; private set; }

        public IEntityObjectDef Def { get; protected set; }

        protected virtual ReplicationLevel ReplicationLevel => ReplicationLevel.ClientBroadcast;

        protected int ReplicationTypeId => EntitiesRepository.GetReplicationTypeId(EntityTypeId, ReplicationLevel);


        //=== Public ==========================================================

        public virtual void Init(Guid entityGuid, int entityTypeId, string debugName)
        {
            _name = debugName;
            EntityTypeId = entityTypeId;
            EntityGuid = entityGuid;
        }

        public static EntityApiWrapper<T> GetWrapper<T>(OuterRef<IEntityObject> outerRef, bool removeOnDispose = false)
            where T : EntityApi, new()
        {
            var go = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(outerRef);
            if (go != null)
            {
                var ego = go.GetComponent<EntityGameObject>();
                return new EntityApiWrapper<T>(outerRef, ego, removeOnDispose);
            }

            return new EntityApiWrapper<T>(outerRef);
        }

        public void OnGotClient()
        {
            AsyncUtils.RunAsyncTask(
                async () =>
                {
                    IsSubscribedSuccessfully = await TryGetEntityWrapper((e) => HandleTimeCheck(OnWrapperReceivedAtStart, e), NodeAccessor.Repository);
                });
        }

        public void OnLostClient()
        {
            if (!IsSubscribedSuccessfully || NodeAccessor.Repository == null)
                return;

            IsSubscribedSuccessfully = false;
            AsyncUtils.RunAsyncTask(async () => { await TryGetEntityWrapperOnDestruction(); });
        }

        public override string ToString()
        {
            return $"{GetType().NiceName()} '{_name}': {EntitiesRepository.GetTypeById(EntityTypeId)?.NiceName()} ({EntityTypeId}), " +
                   $"{nameof(ReplicationTypeId)}{ReplicationTypeId}, {nameof(EntityGuid)}={EntityGuid}, " +
                   $"{nameof(IsSubscribedSuccessfully)}{IsSubscribedSuccessfully.AsSign()}";
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;

            base.Dispose();
            OnLostClient();
        }


        //=== Protected =======================================================

        protected void LogError(string message, LogLevel logLevel = null, [CallerMemberName] string callerMethodName = "")
        {
            if (logLevel == null)
                logLevel = LogLevel.Error;
            Logger.Log(logLevel, $"{callerMethodName}() {message}  -- {this}");
        }

        protected async Task<bool> TryGetEntityWrapper(Func<IEntity, Task> onSuccess, IEntitiesRepository repository)
        {
            if (onSuccess == null)
                return true;

            var objTag = onSuccess.Target?.GetType().Name + "_" + onSuccess.Method.Name;
            using (var wrapper = await repository.Get(ReplicationTypeId, EntityGuid, (object) objTag))
            {
                var entity = wrapper?.Get<IEntity>(ReplicationTypeId, EntityGuid);
                if (entity == null)
                {
                    LogError(NullEntityErrMsg);
                    return false;
                }

                await onSuccess(entity);
            }

            return true;
        }

        protected virtual Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            return Task.CompletedTask;
        }


        //=== Private =========================================================

        private async Task TryGetEntityWrapperOnDestruction()
        {
            using (var container = await NodeAccessor.Repository.Get(ReplicationTypeId, EntityGuid))
            {
                container.TryGet<IEntity>(ReplicationTypeId, EntityGuid, out var entity);
                if (entity == null)
                {
                    LogError(NullEntityErrMsg, LogLevel.Debug);
                    return;
                }

                await HandleTimeCheck(OnWrapperReceivedAtEnd, entity);
            }
        }

        private async Task HandleTimeCheck(Func<IEntity, Task> someFunc, IEntity wrapper)
        {
            var startDateTime = DateTime.Now;
            await someFunc(wrapper);
            var timeSpan = DateTime.Now - startDateTime;
            if (timeSpan.TotalMilliseconds > MinTotalMillisecondsForWarning)
                UI.Logger.IfWarn()?.Message($"<{GetType()}> Too much long {nameof(OnWrapperReceivedAtStart)}() handling: {timeSpan.TotalSeconds:f3}s").Write();
        }
    }
}