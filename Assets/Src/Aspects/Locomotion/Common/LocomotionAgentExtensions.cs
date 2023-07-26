using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Utils;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace Src.Locomotion
{
    public class LocomotionAgentExtensions
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void SetLocomotionToEntity(bool initOrClean, ILocomotionEngineAgent agent, IDirectMotionProducer motionProducer, IGuideProvider guideProvider, OuterRef entityRef, IEntitiesRepository repo)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled) DbgLog.Log(17474, $"pz-17474: {entityRef.Guid}: SetLocomotionToEntity(initOrClean:{initOrClean}, motionProducer:{motionProducer != null}, guideProvider:{guideProvider != null})");

            if (!initOrClean)
                agent.SetReady(false);

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cnt = await repo.Get(entityRef.TypeId, entityRef.Guid))
                {
                    IHasLocomotionOwnerClientBroadcast hasLocomotion = cnt.Get<IHasLocomotionOwnerClientBroadcast>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (hasLocomotion != null)
                    {
                        ILocomotionOwnerClientBroadcast locomotionOwner = hasLocomotion.LocomotionOwner;
                        await locomotionOwner.SetLocomotion(agent, motionProducer, guideProvider);
                    }
                    else
                        Logger.IfError/*IfDebug*/()?.Message($"hasLocomotion == null (initOrClean:{initOrClean}, motionProducer:{motionProducer!=null}, guideProvider:{guideProvider!=null}, entityRef:{entityRef.Guid}).  [ It's ok if LostCL & LostS - both ]");
                }
            
                if (initOrClean)
                    agent.SetReady(true);
            });
        }

        public static void CleanLocomotionToEntity(ILocomotionEngineAgent agent, OuterRef entityRef, IEntitiesRepository repo)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled) DbgLog.Log(17474, $"pz-17474: {entityRef.Guid}: CleanLocomotionToEntity");

            SetLocomotionToEntity(false, agent, null, null, entityRef, repo);
        }
    }
}