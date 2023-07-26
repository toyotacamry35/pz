using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.AsyncStack;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    /// <summary>
    /// Ответственность: Контейнер, содержащий обращение за Entity.
    /// Возвращает наружу асинхронно либо то что у нас Entity есть либо что его нет и не будет
    /// </summary>
    public class EntityTouchContainer<T> : ITouchContainer<T> where T : IEntity
    {
        private const int MaxAttempts = 10;

        public readonly IEntitiesRepository Repo;
        public readonly int TypeId;
        public readonly Guid EntityId;
        public readonly ReplicationLevel ReplicationLevel;

        protected Action<T> onEntity;
        protected Action onNoEntity;

        protected readonly object _callbacksLock = new object();
        private bool isDone;

        //=== Props ===========================================================

        public bool WasSucessful { get; protected set; }


        //=== Ctor ============================================================

        public EntityTouchContainer(IEntitiesRepository repo, int typeId, Guid entityId, ReplicationLevel replicationLevel)
        {
            Repo = repo;
            TypeId = typeId;
            EntityId = entityId;
            ReplicationLevel = replicationLevel;
        }

        public EntityTouchContainer(EntityTouchContainer<T> source) : this(source.Repo, source.TypeId, source.EntityId,
            source.ReplicationLevel)
        {
        }


        //=== Public ==========================================================

        public void SetCallbacks(Action<T> onEntityCallback, Action onNoEntityCallback)
        {
            if (isDone)
                throw new Exception("Мы подписываемся на уже протухший контейнер, это признак какой-то ошибки в логике.");
            //TODO подпереть эксепшеном, если кто-то пытается подписаться на контейнер, который уже так или иначе отмучался.
            lock (_callbacksLock)
            {
                if (onEntityCallback != null)
                    onEntity += onEntityCallback;

                if (onNoEntityCallback != null)
                    onNoEntity += onNoEntityCallback;
            }
        }

        public virtual async Task Connect()
        {
            await AsyncUtils.RunAsyncTask(() => InternalConnect());
        }

        protected async Task InternalConnect(int attempt = 0)
        {
            try
            {
                using (var wrapper = await Repo.Get(TypeId, EntityId))
                {
                    if (wrapper != null)
                    {
                        var entity = wrapper.Get<T>(TypeId, EntityId, ReplicationLevel);
                        //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] EntityTouchContainer.Connect() // EntityId:{EntityId}");
                        if (entity == null)
                        {
                            lock (_callbacksLock)
                            {
                                WasSucessful = false;
                                try
                                {
                                    onNoEntity?.Invoke();
                                }
                                catch (Exception e)
                                {
                                    ReactiveLogs.Logger.IfError()?.Message(e, $"onNoEntity.Invoke() was failed: {nameof(TypeId)}={TypeId}, {nameof(EntityId)}={EntityId}, {nameof(ReplicationLevel)}={ReplicationLevel}").Write();
                                }
                                finally
                                {
                                    onEntity = null;
                                    onNoEntity = null;
                                    isDone = true;
                                }
                            }
                            return;
                        }
                        else
                        {
                            lock (_callbacksLock)
                            {
                                WasSucessful = true;
                                try
                                {
                                    onEntity?.Invoke(entity);
                                }
                                catch (Exception e)
                                {
                                    ReactiveLogs.Logger.IfError()?.Message(e, $"onNoEntity.Invoke() was failed: {nameof(TypeId)}={TypeId}, {nameof(EntityId)}={EntityId}, {nameof(ReplicationLevel)}={ReplicationLevel}").Write();
                                }
                                finally
                                {
                                    onEntity = null;
                                    onNoEntity = null;
                                    isDone = true;
                                }
                            }
                            return;
                        }
                    }
                }
            }
            catch (RepositoryTimeoutException timeout)
            {
                ReactiveLogs.Logger.IfError()?.Message(timeout, $"Attempt ({attempt}) to get entity <{typeof(T).NiceName()}> was failed by TimeoutException: {nameof(TypeId)}={TypeId}, {nameof(EntityId)}={EntityId}, {nameof(ReplicationLevel)}={ReplicationLevel}").Write();
            }

            if (attempt < MaxAttempts)
            {
                await InternalConnect(++attempt);
            }
            else
            {
                lock (_callbacksLock)
                {
                    WasSucessful = false;
                    onNoEntity?.Invoke();
                    onNoEntity = null;
                    onEntity = null;
                    ReactiveLogs.Logger.IfError()?.Message($"All attempts ({MaxAttempts}) to get entity <{typeof(T).NiceName()}> was failed: {nameof(TypeId)}={TypeId}, {nameof(EntityId)}={EntityId}, {nameof(ReplicationLevel)}={ReplicationLevel}").Write();
                }
            }
        }
    }
}