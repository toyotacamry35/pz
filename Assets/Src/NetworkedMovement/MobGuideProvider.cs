using System;
using System.Threading.Tasks;
using Assets.Src.Shared.Impl;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourceSystem.Utils;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils.Extensions;
using UnityEngine;
using SVector3 = SharedCode.Utils.Vector3;

namespace Assets.Src.NetworkedMovement
{
    public class MobGuideProvider : IGuideProvider
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly Transform _mobTransform;
        private Transform _currentTargetTransform;

        public MobGuideProvider([NotNull] Transform mobTransform)
        {
            _mobTransform = mobTransform ?? throw new ArgumentNullException(nameof(mobTransform));
        }

        public SVector3 Guide
        {
            get
            {
                Vector3 guide;
                if (_currentTargetTransform && _mobTransform)
                    guide = (_currentTargetTransform.position - _mobTransform.position).normalized;
                else
                if (_mobTransform)
                    guide = _mobTransform.forward;
                else
                    guide = Vector3.zero;
                return guide.ToShared();
            }
        }

        public void Restart(OuterRef entityRef, IEntitiesRepository repo)
        {
            ManageSubscription(entityRef, repo, true);
        }

        public void Stop(OuterRef entityRef, IEntitiesRepository repo)
        {
            if (repo != null) //could be, if LostCl & LostS (both at once)
                ManageSubscription(entityRef, repo, false); ///#PZ-13761: ?? toKyril?: возможно перформанснее бу не отписываться и подписываться, а просто переставать реагировать на подписку?, хотя наверное нет
            _currentTargetTransform = null;
        }

        public void ManageSubscription(OuterRef entityRef, IEntitiesRepository repo, bool subscribe)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cnt = await repo.Get(entityRef.TypeId, entityRef.Guid))
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(entityRef.Guid, $"{(subscribe ? "Subscribe to" : "Unsubscribe from")} {nameof(IAiTargetRecipientAlways)}{nameof(IAiTargetRecipientAlways.Target)} | Repository:{repo}").Write();
                    var hasRecipient = cnt.Get<IHasAiTargetRecipientAlways>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Always);
                    if (subscribe)
                    {
                        hasRecipient.AiTargetRecipient.SubscribePropertyChanged(nameof(IAiTargetRecipientAlways.Target), OnAiTargetChanged);
                        SetTarget(hasRecipient.AiTargetRecipient.Target);
                    }
                    else
                    {
                        hasRecipient?.AiTargetRecipient.UnsubscribePropertyChanged(nameof(IAiTargetRecipientAlways.Target), OnAiTargetChanged);
                        SetTarget(OuterRef.Invalid);
                    }
                }
            });
        }

        private Task OnAiTargetChanged(EntityEventArgs args)
        {
            SetTarget((OuterRef)args.NewValue);
            return Task.CompletedTask;
        }
        
        private void SetTarget(OuterRef targetRef)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    var targetGo = targetRef.IsValid ? GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(targetRef.To<IEntityObject>()) : null;
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Set AI target | Owner:{(_mobTransform ? _mobTransform.name : "<null>")} Target:{(targetGo ? targetGo.transform.FullName() : "<none>")}").Write();
                    _currentTargetTransform = targetGo ? targetGo.transform : null;
                });
        }
    }
}
