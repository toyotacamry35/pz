using System;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace Src.Animation
{
    public static class AnimationDoerExtensions
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static void SetAnimationDoerToEntity(this IAnimationDoer doer, OuterRef<IHasAnimationDoerOwnerClientBroadcast> entityRef, IEntitiesRepository repository)
        {
            if (doer == null) throw new ArgumentNullException(nameof(doer));
            if (!entityRef.IsValid) throw new ArgumentException(nameof(entityRef));
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"SetAnimationDoerToEntity | Entity:{entityRef} Repo:{repository.Id}({repository.CloudNodeType})").Write();
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var container = await repository.Get(entityRef))
                {
                    var entity = container.Get(entityRef, ReplicationLevel.ClientBroadcast);
                    if (entity != null)
                        await entity.AnimationDoerOwner.SetAnimationDoer(doer);
                }
            });
        }
        
        public static void UnsetAnimationDoerToEntity(this IAnimationDoer doer, OuterRef<IHasAnimationDoerOwnerClientBroadcast> entityRef, IEntitiesRepository repository)
        {
            if (doer == null) throw new ArgumentNullException(nameof(doer));
            if (!entityRef.IsValid) throw new ArgumentException(nameof(entityRef));
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UnsetAnimationDoerToEntity | Entity:{entityRef} Repo:{repository.Id}({repository.CloudNodeType})").Write();
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var container = await repository.Get(entityRef))
                {
                    var entity = container.Get(entityRef, ReplicationLevel.ClientBroadcast);
                    if (entity != null)
                        await entity.AnimationDoerOwner.UnsetAnimationDoer(doer);
                }            
            });
        }
    }
}