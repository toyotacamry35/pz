using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Server.Impl;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using UnityEngine;
using static SharedCode.Wizardry.UnityEnvironmentMark;

using SharedPosRot = SharedCode.Entities.GameObjectEntities.PositionRotation;


namespace Assets.Src.Aspects.Impl.EntityGameObjectComponents
{
    //! Don't use it - subscribe directly to IMortalObject
    [DisallowMultipleComponent]
    [Obsolete] //Это временное решение-заглушка к проблеме описанной в (см. коммент "(Подробный коммент)" в файле Pawn.cs)
    public class MortalObjectComponent : EntityGameObjectComponent
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("MortalObjectComponent");

        public event Func<Guid, int, Task> DieClientRetranslatedEvent;
        public event Func<Guid, int, Task> DieServerRetranslatedEvent;

        protected override void GotClient() => GotAny(ServerOrClient.Client);
        private void GotAny(ServerOrClient forWhom)
        {
            var repo = NodeAccessor.Repository;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(TypeId, EntityId))
                {
                    //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"$$ {TypeId}, {EntityId}").Write();
                    if (wrapper == null)
                    {
                         Logger.IfError()?.Message("wrapper == null").Write();;
                        return;
                    }

                    var entity = wrapper.Get<IHasMortalClientBroadcast>(TypeId, EntityId, ReplicationLevel.ClientBroadcast);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("entity == null").Write();;
                        return;
                    }

                    SubscribeToEntity(entity, forWhom);
                }
            }, repo);
        }

        protected override void LostClient() => LostAny(ServerOrClient.Client);
        private void LostAny(ServerOrClient forWhom)
        {
            var repo = NodeAccessor.Repository;

            if (repo != null)
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await repo.Get(TypeId, EntityId))
                    {
                        if (wrapper.TryGet<IHasMortalClientBroadcast>(TypeId, EntityId, ReplicationLevel.ClientBroadcast, out var entity))
                            UnsubscribeFromEntity(entity, forWhom);
                    }
                }, repo);
        }

        private void SubscribeToEntity(IHasMortalClientBroadcast entity, ServerOrClient forWhom)
        {
            Logger.IfDebug()?.Message/*Warn*/($"#DBG: subscribed forWhom: {forWhom}").Write();
            //UnsubscribeFromEntity(entity, forWhom);
            if (entity == null)
            {
                Logger.IfError()?.Message($"Missing entity.").Write();
                return;
            }

            if (forWhom == ServerOrClient.Client)
                entity.Mortal.DieEvent += OnDieClient;
            else
                entity.Mortal.DieEvent += OnDieServer;
        }

        private void UnsubscribeFromEntity(IHasMortalClientBroadcast entity, ServerOrClient forWhom)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"#DBG: unsubscribed forWhom: {forWhom}").Write();
            if (entity == null)
                return;

            if (forWhom == ServerOrClient.Client)
                entity.Mortal.DieEvent -= OnDieClient;
            else
                entity.Mortal.DieEvent -= OnDieServer;
        }

        private Task OnDieClient(Guid entityId, int entityTypeId, SharedPosRot corpsePlace)
        {
            UnityQueueHelper.RunInUnityThread(async () =>
            {
                if (DieClientRetranslatedEvent != null)
                    await DieClientRetranslatedEvent(entityId, entityTypeId);
            });
            return Task.CompletedTask;
        }

        private Task OnDieServer(Guid entityId, int entityTypeId, SharedPosRot corpsePlace)
        {
            UnityQueueHelper.RunInUnityThread(async () =>
            {
                if (DieServerRetranslatedEvent != null)
                    await DieServerRetranslatedEvent(entityId, entityTypeId);
            });
            return Task.CompletedTask;
        }
    }
}
