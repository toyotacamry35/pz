using Assets.Src.GameObjectAssembler;
using Assets.Src.ResourceSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Tools;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using ReactivePropsNs;
using UnityEngine;
using SharedCode.MovementSync;
using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.ContainerApis;
using Core.Environment.Logging.Extension;
using SharedCode.Serializers;

namespace Assets.Src.SpawnSystem
{
    public class EntityGameObject : MonoBehaviour, IFromDef<EntityGameObjectDef>, IDebugInfoProvider, IJsonToGoTemplateComponentAfterAllInit
    {
        public bool SelfMergeWithJson = false;
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public EntityGameObjectComponent[] _egoComponents;
        public List<EntityApi> EntityApis { get; set; }

        private void InvokeStatusChanged()
        {
            if (_egoComponents == null)
                Logger.IfError()?.Message("EGO components is null for {GameObject}", name).Entity(EntityId).Write();
            else
                for (int i = 0;  i < _egoComponents.Length;  ++i)
                {
                    try
                    {
                        if (_egoComponents[i] != null)
                            _egoComponents[i].ForceStatusChanged();
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Exception(e).Write();
                    }
                }
        }

        public event Action<bool> Initialized;
        public void AddRepo(IEntitiesRepository repo, bool clientMapTEST)
        {
            if (repo.CloudNodeType == SharedCode.Cloud.CloudNodeType.Server || clientMapTEST)
            {
                SetServerRepo(repo);
            }

            if (repo.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client)
            {
                if (ClientRepo != null)
                    throw new InvalidOperationException($"Invalid logic: trying to unset client repo {repo}, while client repo is {ClientRepo}");

                IsClientRepo = true;
                ClientRepoRp.Value = ClientRepo = repo;
            }

            InvokeStatusChanged();
        }

        private void SetServerRepo(IEntitiesRepository repo)
        {
            if (ServerRepo != null)
                throw new InvalidOperationException($"Invalid logic: trying to set server repo {repo}, while server repo is {ServerRepo}");
            ServerRepoRp.Value = ServerRepo = repo;
            IsServerRepo = true;
        }

      

        public void RemoveRepo(IEntitiesRepository repo)
        {
            if (repo.CloudNodeType == SharedCode.Cloud.CloudNodeType.Server)
                IsServerRepo = false;
            if (repo.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client)
                IsClientRepo = false;
            InvokeStatusChanged();
            if (repo.CloudNodeType == SharedCode.Cloud.CloudNodeType.Server)
            {
                if (ServerRepo == null || ServerRepo != repo)
                    throw new InvalidOperationException($"Invalid logic: trying to unset server repo {repo}, while server repo is {ServerRepo} | Entity:{OuterRef}");

                ServerRepoRp.Value = ServerRepo = null;
            }

            if (repo.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client)
            {
                if (ClientRepo == null || ClientRepo != repo)
                    throw new InvalidOperationException($"Invalid logic: trying to unset client repo {repo}, while client repo is {ClientRepo} | Entity:{OuterRef}");

                ClientRepoRp.Value = ClientRepo = null;
                ClientAuthorityRepoRp.Value = null; //вызовы очистки репо и LostClientAuthority происходят в произвольном порядке
            }

        }

        public void GotPathfindingOwnership()
        {
            // got pathfinding can be called only when client repository inited 
             Logger.IfInfo()?.Message("Got pathfinding ownership").Write();;
            SetServerRepo(ClientRepo);
            IsPathfindingOwner = true;
            InvokeStatusChanged();

            
        }

        public void TryCleanPathfindingOwnership()
        {
            if (IsServerRepo)
            {
                 Logger.IfInfo()?.Message("Lost pathfinding ownership").Write();;
                IsServerRepo = false;
                InvokeStatusChanged();
                ServerRepoRp.Value = ServerRepo = null;
                // in two steps because clean pathfinding should be called when ServerRepo = null
                IsPathfindingOwner = false;
                InvokeStatusChanged();  ///PZ-2020.05.OPTIMIZ: ??:to Vova_I.: зачем - с 2ух мобов получается 4 этих вызова и 40 `EGOComp.StatusChanged()`. Возможно, т.к. разбивается в Pawn об `IsHost` (пока сервер ещё в строю)
            }
        }

        //Не путайте с репозиторием с мастер копией. Это ссылка на дедик-репозиторий, 
        //в котором геймобж-компоненты могут писать в энтити (например позицию физически симулируемого объекта)
        public IEntitiesRepository AuthorityRepo;

        public int _typeId;
        public int TypeId
        {
            get
            {
                return _typeId;
            }
            set
            {
                _typeId = value;

#if UNITY_EDITOR
                gameObject.name += $" ({value})";
#endif
                //  if (_entityId != Guid.Empty && _typeId != 0)
                //  StatsProxy.Register(_typeId, _entityId, ClusterCommands.Repository);
            }
        }
        private Guid _wsId;
        public Guid WorldSpaceId
        {
            get
            {
                return _wsId;
            }
            set
            {
                _wsId = value;
#if UNITY_EDITOR
                gameObject.name += $" ({value})";
#endif

                //  if (_entityId != Guid.Empty && _typeId != 0)
                //     StatsProxy.Register(_typeId, _entityId, ClusterCommands.Repository);
            }
        }

        private Guid _entityId;
        public Guid EntityId
        {
            get
            {
                return _entityId;
            }
            set
            {
                _entityId = value;
#if UNITY_EDITOR
                gameObject.name = gameObject.name.Replace("(Clone)", "") + $" <{_entityId}>";
#endif
                //  if (_entityId != Guid.Empty && _typeId != 0)
                //     StatsProxy.Register(_typeId, _entityId, ClusterCommands.Repository);
            }
        }

        public OuterRef<IEntity> OuterRefEntity => new OuterRef<IEntity>(EntityId, TypeId);
        public OuterRef<IEntityObject> OuterRef => new OuterRef<IEntityObject>(EntityId, TypeId);
        public IEntityObjectDef EntityDef;
        public JdbMetadata StaticDef;
        public IEntitiesRepository ServerRepo { get; set; }
        public IEntitiesRepository ClientRepo { get; set; }
        public bool IsServerRepo { get; set; }
        public bool IsPathfindingOwner { get; set; }
        public bool IsClientRepo { get; set; }
        public bool HasServerAuthority { get; private set; } = false;
        public bool HasClientAuthority { get; private set; } = false;

        public ReactiveProperty<IEntitiesRepository> ServerRepoRp = new ReactiveProperty<IEntitiesRepository>();
        public ReactiveProperty<IEntitiesRepository> ClientRepoRp = new ReactiveProperty<IEntitiesRepository>();
        public ReactiveProperty<IEntitiesRepository> ServerAuthorityRepoRp = new ReactiveProperty<IEntitiesRepository>();
        public ReactiveProperty<IEntitiesRepository> ClientAuthorityRepoRp = new ReactiveProperty<IEntitiesRepository>();

        public bool HasNoRepos => ServerRepo == null && ClientRepo == null;

        public EntityGameObjectDef Def { get; set; }


        public bool IsPlayer = false;



        internal void NotifyOfServerAuthority()
        {
            if (ServerRepo == null)
                return;

            ServerAuthorityRepoRp.Value = ServerRepo;
            HasServerAuthority = true;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "GotServerAuthority").Write();
            InvokeStatusChanged();
        }

        internal void NotifyOfClientAuthority()
        {
            if (ClientRepo == null)
                return;

            ClientAuthorityRepoRp.Value = ClientRepo;
            HasClientAuthority = true;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "GotClientAuthority").Write();
            InvokeStatusChanged();
        }

        internal void NotifyOfClientAuthorityLost()
        {
            HasClientAuthority = false;
            ClientAuthorityRepoRp.Value = null;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "LostServerAuthority").Write();
            InvokeStatusChanged();
        }

        internal void NotifyOfServerAuthorityLost()
        {
            HasServerAuthority = false;
            ServerAuthorityRepoRp.Value = null;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "LostClientAuthority").Write();
            InvokeStatusChanged();
        }




        ///#TC-2630
        //#Tmp hack. All where this is needed should be `EntityG.o.Component`
        public static EntityDesc Tmp_GetEntityGuidAndTypeId(GameObject go)
        {
            if (go == null)
                return EntityDesc.Invalid;
            var ego = go?.GetRoot()?.GetComponent<EntityGameObject>();
            if (!ego)
                return EntityDesc.Invalid;
            return new EntityDesc(ego.EntityId, ego.TypeId);
        }

        // --- Dbg ----------------------------------------------------------------



        // --- IDebugInfoProvider: ---------------------------------
        public string GetDebugInfo()
        {
            var grid = VisibilityGrid.Get(WorldSpaceId, ServerRepo ?? ClientRepo);
            var charsHash = (PointSpatialHash<CharacterMovementState>)grid?.Dbg_GetPointHashByType(typeof(CharacterMovementState));

            return $"EGO::1 _visible:  \n"
                 + $"EGO::2 R ForGridQuery:  \n"
                 //+ $"EGO::3 grid data:  {_dbg_pointSpaHashData}\n";
                 + $"EGO::3.1 Cell my:  {new PointSpatialHash<MobMovementState>.CellVector3(transform.position.ToShared())}\n"
                 //+ $"EGO::3.2 Cells chars:  {_dbg_pointSpaHashData}\n";
                 + $"EGO::3.2 Cells chars:  {(charsHash != null ? string.Join(";\n", charsHash.Dbg_GetAllObjects()) : "Can't get data")}";
        }


        public void SubscribeOnClientRepo<T>(ICollection<IDisposable> disposibles, ReplicationLevel replicationLevel, Action<T> subscribe, Action<T> unsubscribe)
            where T : class
        {
            IEntitiesRepository subscribedRepo = null;
            var typeId = TypeId;
            var entityId = EntityId;
            ClientRepoRp.Action(disposibles, clientRepo =>
            {
                AsyncUtils.RunAsyncTask(async () =>
                    {
                        if (clientRepo != null)
                        { // Подписка
                            using (var wrapper = await clientRepo.Get(typeId, entityId))
                            {
                                var entity = wrapper?.Get<T>(typeId, entityId, replicationLevel);
                                if (entity == null)
                                    return;
                                subscribedRepo = clientRepo;
                                subscribe(entity);
                            }
                        }
                        else
                        { // Отписка
                            using (var wrapper = await subscribedRepo.Get(typeId, entityId))
                            {
                                var entity = wrapper?.Get<T>(typeId, entityId, replicationLevel);
                                if (entity == null)
                                    return;
                                unsubscribe(entity);
                                subscribedRepo = null;
                            }
                        }
                    });
            });
        }
        public void InitAfterAllForJsonToGoTemplate()
        {
            _egoComponents = GetComponentsInChildren<EntityGameObjectComponent>();
        }
    }

    public static class GameObjectExtension
    {
        public static EntityGameObject GetEntityGameObject(this GameObject go) => go?.GetRoot().GetComponent<EntityGameObject>();

        public static OuterRef<IEntityObject> GetEntityRef(this GameObject go) => go.GetEntityGameObject()?.OuterRef ?? OuterRef<IEntityObject>.Invalid;
    }

}
