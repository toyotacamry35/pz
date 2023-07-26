using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Server;
using Assets.Src.Server.Impl;
using JetBrains.Annotations;
using SharedCode.EntitySystem;

namespace Assets.Src.Shared
{
    [UsedImplicitly]
    public class ColonyBehaviour : InternalBaseBehaviour
    {

        //не трогать пока все импакты/эффекты и прочее не уедут на кластер
        public IResource TemporarysDef { get; set; }
        protected float TickTime { get; set; } = 0f;

        // protected static IEntitiesRepository ServerUnityRepository => ServerProvider.Server.EntitiesRepository;
        // protected static IEntitiesRepository ClientRepository => NodeAccessor.Repository;


        //=== Props ===========================================================

        // protected IServer Server => ServerProvider.Server;

        // protected bool IsServer => ServerProvider.IsServer;
        // protected bool IsClient => ServerProvider.IsClient || Server.Host;


        //=== Unity ===========================================================

        private void Start()
        {
            AnyStart();
            // if (IsServer)
            //     ServerStart();
            // if (IsClient)
            //     ClientStart();
            /*
            //Bug - non-local players on host would receive localStart for no reason
            if (HasAuthority || (Server != null && Server.Host))
                LocalStart();
                */
        }


        //=== Protected =======================================================

        protected virtual void AnyStart()
        {
        }

        protected virtual void ClientStart()
        {
        }

        protected virtual void ServerStart()
        {
        }
    }
}