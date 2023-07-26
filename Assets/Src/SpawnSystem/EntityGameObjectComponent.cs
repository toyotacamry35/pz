using System;
using Assets.Src.NetworkedMovement;
using NLog;
using SharedCode.EntitySystem;
using JetBrains.Annotations;
using GeneratedCode.Repositories;
using Assets.Src.Server.Impl;
using ResourceSystem.Utils;
using Uins;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using Assets.Src.GameObjectAssembler;

namespace Assets.Src.SpawnSystem
{
    public class EntityGameObjectComponent : HasDisposablesMonoBehaviour, IJsonToGoTemplateComponentInit
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("EntityGameObjectComponent");

        bool _isServer = false;
        bool _isClient = false;
        bool _hasClientAuthority = false;
        bool _hasServerAuthority = false;
        bool _isPathdinginOwner = false;
        public EntityGameObject Ego;

        protected bool IsGlobalServer => ServerProvider.IsServer;
        protected bool IsGlobalClient => ServerProvider.IsClient;
        protected bool IsClient => _isClient;
        protected bool IsServer => _isServer;
        protected IEntitiesRepository ClientRepo => Ego.ClientRepo;
        protected IEntitiesRepository ServerRepo => Ego.ServerRepo;
        protected bool HasAuthority => _hasClientAuthority || _hasServerAuthority;
        protected bool HasClientAuthority => _hasClientAuthority;
        protected bool HasServerAuthority => _hasServerAuthority;
        protected bool IsHostMode => ClientRepo != null && ServerRepo != null;
        //`public` is needed for (see) `AnimatedPawnView : MonoBehaviour`
        /*protected*/
        public Guid EntityId => Ego.EntityId;
        /*protected*/
        public int TypeId => Ego.TypeId;
        public OuterRef<T> GetOuterRef<T>() => new OuterRef<T>(Ego.EntityId, Ego.TypeId);
        public OuterRef OuterRef => new OuterRef(Ego.EntityId, Ego.TypeId);

        protected virtual bool CanBeOnChildObject => false;

        protected override void OnDestroy()
        {
            DestroyInternal();
            base.OnDestroy();
        }

        // Is used to call from another EGOComponent of same EGO to ensure curr. is has all actual EGO flags (IsCl, Has..Auth, e.t.c.).
        public void ForceStatusChanged() => StatusChanged();

        protected void StatusChanged()
        {
            if (false) if (this is Pawn)
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"EgoComp::StatusChanged: {gameObject.name}  " +
                                                                            $"_isServer:{_isServer}, Ego.IsSRepo:{Ego.IsServerRepo}, " +
                                                                            $"_isClient:{_isClient}, Ego.IsClRepo:{Ego.IsClientRepo}, " +
                                                                            $"_hasSAuth:{_hasServerAuthority}, Ego.HasSAuth:{Ego.HasServerAuthority}, " +
                                                                            $"_hasClAuth:{_hasClientAuthority}, Ego.HasClAuth:{Ego.HasClientAuthority}. " +
                                                                            $"EnttyId: {Ego?.EntityId.ToString() ?? "Ego==null"}").Write();

            if (!_isServer && Ego.IsServerRepo)
            {
                _isServer = true;
                GotServer();
            }
            if (!_hasServerAuthority && Ego.HasServerAuthority)
            {
                _hasServerAuthority = true;
                GotServerAuthority();
            }

            if (_hasServerAuthority && !Ego.HasServerAuthority)
            {
                _hasServerAuthority = false;
                LostServerAuthority();
            }
            if (_isServer && !Ego.IsServerRepo)
            {
                _isServer = false;
                LostServer();
            }

            if (!_isClient && Ego.IsClientRepo)
            {
                _isClient = true;
                GotClient();

            }
            if (!_hasClientAuthority && Ego.HasClientAuthority)
            {
                _hasClientAuthority = true;
                GotClientAuthority();
            }

            if ((_hasClientAuthority && !Ego.HasClientAuthority) || (_hasClientAuthority && _isClient && !Ego.IsClientRepo))
            {
                _hasClientAuthority = false;
                LostClientAuthority();
            }
            if (_isClient && !Ego.IsClientRepo)
            {
                _isClient = false;
                LostClient();
            }

            if (_isPathdinginOwner && !Ego.IsPathfindingOwner)
            {
                _isPathdinginOwner = false;
                LostPathfindingOwnership();
            }

            if (!_isPathdinginOwner && Ego.IsPathfindingOwner)
            {
                _isPathdinginOwner = true;
                GotPathfindingOwnership();
            }
        }


        // Interface ------------------------------------------------------------------------------


        protected virtual void DestroyInternal()
        {
        }
        protected virtual void OnWakeup()
        {
        }

        protected virtual void OnFallAsleep()
        {
        }

        //#note: Authority is not given now at all
        protected virtual void GotServerAuthority()
        {
        }

        //#note: Authority is not given now at all
        protected virtual void LostServerAuthority()
        {
        }

        //#note: Authority is not given now at all
        protected virtual void GotClientAuthority()
        {
        }

        //#note: Authority is not given now at all
        protected virtual void LostClientAuthority()
        {
        }

        protected virtual void GotClient()
        {
        }

        protected virtual void LostClient()
        {
        }

        protected virtual void GotServer()
        {
        }

        protected virtual void LostServer()
        {
        }

        protected virtual void GotPathfindingOwnership()
        {

        }

        protected virtual void LostPathfindingOwnership()
        {

        }

        public void InitForJsonToGoTemplate()
        {
            Ego = GetComponent<EntityGameObject>();
            if (Ego == null)
                Ego = GetComponentInParent<EntityGameObject>();
            if (Ego == null)
            {
                Logger.IfError()?.Message($"Missing EGO on '{transform.FullName()}'").Write();
                return;
            }
        }
    }

}
