using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Aspects.Doings;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.MapSystem;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Aspects.Sessions;
using SharedCode.Config;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class MapHostEntity : IHookOnReplicationLevelChanged, IHookOnInit, IHookOnDestroy, IHookOnUnload
    {
        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            AsyncUtils.RunAsyncTask(() =>
            {
                if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast))
                {
                    if (ClientCommRuntimeDataProvider.CharRuntimeData != null)
                        ClientCommRuntimeDataProvider.CharRuntimeData.CurrentPrimaryWorldSceneRepositoryId = OwnerNodeId;
                    ClientCommRuntimeDataProvider.CurrentPrimaryWorldSceneRepositoryId = OwnerNodeId;
                }
                return Task.CompletedTask;
            });
        }
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task OnInit()
        {
            EntitiesRepository.NewEntityUploaded += EntitiesRepository_NewEntityUploaded;
            return Task.CompletedTask;
        }

        public async ValueTask<bool> AddUsersDirectImpl(List<Guid> users)
        {
            await CheckAndAddUsers(users);

            return true;
        }

        private Task EntitiesRepository_NewEntityUploaded(int typeId, Guid entityId)
        {
            if (ReplicaTypeRegistry.GetIdByType(typeof(IClientCommunicationEntity)) == typeId)
                AsyncUtils.RunAsyncTask(async () => {
                    if(!await AddUser(entityId))
                        Logger.IfError()?.Message("Can't get client communication entity with id {repo_id}", entityId).Write();
                });
            return Task.CompletedTask;
        }
        ConcurrentDictionary<Guid, MapDef> _maps = new ConcurrentDictionary<Guid, MapDef>();
        public async Task<HostOrInstallMapResult> HostMapImpl(Guid mapGuid, Guid realmId, MapDef map, RealmRulesDef realmRules)
        {
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Host map mapGuid:{mapGuid} realmId:{realmId} realmRules:{realmRules}").Write();
            if (mapGuid == Guid.Empty)
            {
                Logger.IfError()?.Message("Cant host map {0} with empty Guid", map?.____GetDebugShortName()).Write();
                return HostOrInstallMapResult.Error;
            }

            var eref = await EntitiesRepository.Load<IMapEntity>(mapGuid);
            if (eref == null)
            {
                if(Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("MapEntity is not exists id DB. Create new one.").Write();;
                await EntitiesRepository.Create<IMapEntity>(mapGuid, async (m) =>
                {
                    m.Map = map;
                    m.RealmId = realmId;
                    m.RealmRules = realmRules;
                });
                
                using (var loginW = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
                {
                    var loginEntity = loginW.GetFirstService<ILoginServiceEntityServer>();
                    await loginEntity.AttachMapToRealm(mapGuid, map, realmId);
                }
            }
            using (var w = await EntitiesRepository.Get<IMapEntity>(mapGuid))
            {
                var m = w.Get<IMapEntity>(mapGuid);
                m.SubscribePropertyChanged(nameof(IMapEntity.Dead), async e =>
                {
                    Logger.IfError()?.Message($"Map is notified to be destoryed, will do: {(bool)e.NewValue}").Write();
                    if (!(bool)e.NewValue)
                        return;
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    Guid wsGuid;
                    using (var wrapper = await EntitiesRepository.Get<IMapEntity>(mapGuid))
                    {
                        var mapEntity = wrapper?.Get<IMapEntityServer>(mapGuid);

                        //TODO поддержать много WorldSpaces-ов
                        wsGuid = mapEntity.WorldSpaces.First().WorldSpaceGuid;
                    }
                    using (var wrapper2 = await EntitiesRepository.Get<IWorldSpaceServiceEntityServer>(wsGuid))
                    {
                        var worldSpaceEntity = wrapper2.Get<IWorldSpaceServiceEntityServer>(wsGuid);
                        await worldSpaceEntity.LogoutAll();
                    }
                    await EntitiesRepository.Destroy(MapEntity.StaticTypeId, mapGuid, true);
                    using (var ww = await this.GetThisWrite())
                        HostedMaps.Remove(map);
                });
            }
            _maps[mapGuid] = map;
            HostedMaps.Add(map);

            if (GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId != Guid.Empty)
                await EntitiesRepository.SubscribeReplication(ReplicaTypeRegistry.GetIdByType(typeof(IMapEntity)), mapGuid, GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, ReplicationLevel.Server);

            return HostOrInstallMapResult.Success;
        }

        private async Task CheckAndAddUsers(List<Guid> users)
        {
            var usersNeedConnection = new List<Guid>();

            foreach(var user in users)
            {
                if (user == Guid.Empty)
                {
                    Logger.IfError()?.Message("userId == Guid.Empty").Write();
                    continue;
                }
                if(!await AddUser(user))
                {
                    usersNeedConnection.Add(user);
                }
            }

            if (!usersNeedConnection.Any())
                return;

            EndpointAddress address;
            using (var wrapper = await EntitiesRepository.Get<IRepositoryCommunicationEntityServer>(OwnerRepositoryId))
            {
                var repositoryMapEntity = wrapper.Get<IRepositoryCommunicationEntityServer>(OwnerRepositoryId);
                if (repositoryMapEntity == null)
                {
                    Logger.IfError()?.Message("Cant find repository comm entity {0}", OwnerRepositoryId).Write();
                    return;
                }

                address = repositoryMapEntity.ExternalAddress;
            }

            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
            {
                var loginInternalEntity = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                await loginInternalEntity.AddClientConnection(usersNeedConnection, address.Address, address.Port, OwnerRepositoryId);
            }
        }

        //это не RPC-метод энтити, имейте ввиду
        private async Task<bool> AddUser(Guid userId)
        {
            MapHostInitialInformation mapHostInfo = null;
            using (var wrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userId))
            {
                if (!wrapper.TryGet<IClientCommunicationEntityServer>(userId, out var clientCommunicationEntity))
                    return false;

                mapHostInfo = await clientCommunicationEntity.GetMapHostInitialInformation();
            }

            if (mapHostInfo == null)
            {
                Logger.IfError()?.Message("Cant get GetMapHostInitialInformation from user {0}", userId).Write();
                //TODO в случае неудачи нужно повторить попытку через время, но не держа залоченным энтити мап хоста
            }

            try
            {
                var mapId = Guid.Empty;
                using (var wrapper = await EntitiesRepository.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>())
                {
                    var worldCoordinator = wrapper.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>();
                    if (worldCoordinator == null)
                    {
                        Logger.IfError()?.Message("WorldCoordinator not found for user {0}", userId).Write();
                        return true;
                    }

                    mapId = await worldCoordinator.GetMapIdByUserId(userId);
                }
                Guid wsGuid = Guid.Empty;
                using (var wrapper = await EntitiesRepository.Get<IMapEntity>(mapId))
                {
                    var mapEntity = wrapper?.Get<IMapEntityServer>(mapId);
                    if (mapEntity == null)
                    {
                        Logger.IfError()?.Message("Map entity {0} not found for userd {1}", mapId, userId).Write();
                        return true;
                    }

                    //TODO поддержать много WorldSpaces-ов
                    wsGuid = mapEntity.WorldSpaces.First().WorldSpaceGuid;
                }
                {
                    AddClientResult result = AddClientResult.None;
                    using (var wrapper2 = await EntitiesRepository.Get<IWorldSpaceServiceEntityServer>(wsGuid))
                    {
                        var worldSpaceEntity = wrapper2.Get<IWorldSpaceServiceEntityServer>(wsGuid);
                        result = await worldSpaceEntity.Login(null, mapHostInfo.SpawnPointPath, userId, new MapOwner(mapId, mapId));//main scene has the same id as the map

                    }
                    switch (result)
                    {
                        case AddClientResult.None:
                            break;
                        case AddClientResult.Added:
                            await EntitiesRepository.SubscribeReplication(ReplicaTypeRegistry.GetIdByType(typeof(IMapEntity)), mapId, userId, ReplicationLevel.ClientBroadcast);
                            using (var clientCommEntitywrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userId))
                            {
                                var clientCommunicationEntity = clientCommEntitywrapper.Get<IClientCommunicationEntityServer>(userId);
                                if (clientCommunicationEntity == null)
                                {
                                    Logger.IfError()?.Message("Can't get client communication entity").Write();
                                    return true;
                                }

                                await clientCommunicationEntity.ConfirmLogin();
                            }
                            break;

                        case AddClientResult.AlreadyExist:
                            break;

                        case AddClientResult.Error:
                            using (var clientCommEntitywrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userId))
                            {
                                var clientCommunicationEntity = clientCommEntitywrapper.Get<IClientCommunicationEntityServer>(userId);
                                if (clientCommunicationEntity == null)
                                {
                                    Logger.IfError()?.Message("Can't get client communication entity").Write();
                                    return true;
                                }

                                await clientCommunicationEntity.DisconnectByError("Login returned generic error", "");
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Disconnecting user {0}: {1}", userId, e).Write();

                using (var wrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userId))
                {
                    var clientCommunicationEntity = wrapper.Get<IClientCommunicationEntityServer>(userId);
                    if (clientCommunicationEntity == null)
                    {
                        Logger.IfError()?.Message("Can't get client communication entity").Write();
                        return true;
                    }

                    await clientCommunicationEntity.DisconnectByError(e.Message, e.ToString());
                }
            }
            return true;
        }
        public async Task OnUnload()
        {
            await OnDestroy();
        }

        public async Task OnDestroy()
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                foreach (var map in _maps)
                {
                    var mapName = map.Value;
                    await EntitiesRepository.Destroy(ReplicaTypeRegistry.GetIdByType(typeof(IMapEntity)), map.Key, true);
                }
            });
        }

        public async Task<bool> LogoutUserFromMapImpl(Guid userId, Guid mapId, bool terminal)
        {
            Guid wsGuid = Guid.Empty;
            using (var wrapper = await EntitiesRepository.Get<IMapEntity>(mapId))
            {
                var mapEntity = wrapper?.Get<IMapEntityServer>(mapId);
                if (mapEntity == null)
                {
                    Logger.IfError()?.Message("Map entity {0} not found for userd {1}", mapId, userId).Write();
                    return true;
                }

                //TODO поддержать много WorldSpaces-ов
                wsGuid = mapEntity.WorldSpaces.First().WorldSpaceGuid;
            }
            await EntitiesRepository.UnsubscribeReplication(MapEntity.StaticTypeId, mapId, userId, ReplicationLevel.ClientBroadcast);
            using (var wrapper2 = await EntitiesRepository.Get<IWorldSpaceServiceEntityServer>(wsGuid))
            {
                var worldSpaceEntity = wrapper2.Get<IWorldSpaceServiceEntityServer>(wsGuid);
                await worldSpaceEntity.Logout(userId, terminal);
            }

            if(terminal)
            {
                using (var wrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userId))
                {
                    var clientCommunicationEntity = wrapper.Get<IClientCommunicationEntityServer>(userId);
                    if (clientCommunicationEntity == null)
                    {
                        Logger.IfError()?.Message("Can't get client communication entity").Write();
                        return true;
                    }

                    await clientCommunicationEntity.GracefullLogout();
                }
            }
            return true;
        }
    }
}
