using System;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.CustomData;
using Assets.ColonyShared.SharedCode.Shared;
using Core.Environment.Logging.Extension;
using Assets.Src.Shared.Impl;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Network.Statistic;
using GeneratedCode.Repositories;
using Infrastructure.Cloud;
using NLog;
using SharedCode.Cloud;
using SharedCode.Config;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;

namespace Assets.Src.Client
{
    public static class ClientRepositoryProcessRunner
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task StartInf(
            EntitiesRepositoryConfig repositoryConfig,
            ConnectionParams connectionParams,
            string verifyServerAddress,
            InnerProcess next,
            CancellationToken baseCt)
        {
            var host = connectionParams.ConnectionAddress;
            var port = connectionParams.ConnectionPort;
            var sharedConfig = new CloudSharedDataConfig
            {
                ClientEntryPoint = new CloudEntryPointConfig
                {
                    Host = host,
                    Port = port
                }
            };

            using (CoreStatisticsManager.Init())
            {
                var serializer = new ProtoBufSerializer();
                var entitySerializer = new EntitySerializer(serializer);
                var repository = new EntitiesRepository(
                    sharedConfig,
                    repositoryConfig,
                    CloudNodeType.Client,
                    0,
                    serializer,
                    entitySerializer,
                    new EntitySubscriptionsProcessor()
                );

                var cachedGameState = GameState.Instance;
                try
                {
                    await repository.InitCommunicationNode();

                    using (var repositoryHost = new ClientRepositoryProcess(repository))
                    {
                        try
                        {
                            GameObjectCreator.ClusterSpawnInstance.Clear();
                            GameState.Instance.ClientRepositoryHost = repositoryHost;
                            await repositoryHost.SubscribeDisconnectedEvent();
                            GameState.Instance.NetIssueText = null;
                            using (var connectCts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(connectCts.Token, baseCt))
                            {
                                bool done = false;
                                for (int i = 0; i < 6; i++)
                                {
                                    try
                                    {
                                        await repository.ConnectToCluster(linkedCts.Token);
                                        done = true;
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.IfError()?.Exception(e);
                                    }

                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                }

                                if (!done)
                                {
                                    GameState.Instance.NetIssueText =
                                        $"Unable to connect to server {host} {port}. Please check that you don't have UDP restrictions on port {port}. If this problem persists, contant your Internet Service Provider.";
                                    throw new Exception($"Can't connect {host} {port}");
                                }
                            }

                            cachedGameState.ClientClusterNode = repository;
                            NodeAccessor.Repository = repository;

                            if (BotCoordinator.IsActive)
                                using (var wrapper = await repository.GetFirstService<IBotCoordinatorServer>())
                                {
                                    if (wrapper == null)
                                        Logger.IfError()?.Message("IBotCoordinator not found {0}", repository.Id).Write();

                                    var service = wrapper?.GetFirstService<IBotCoordinatorServer>();
                                    if (service != null)
                                        await service.Register();
                                }

                            var (runtimeData, loginResult, loginEntityId) = await repositoryHost.ConnectToCluster(
                                verifyServerAddress,
                                connectionParams.Code,
                                connectionParams.UserId,
                                connectionParams.Version,
                                baseCt
                            );

                            ClientCommRuntimeDataProvider.CharRuntimeData = runtimeData;
                            GameState.Instance.CharacterRuntimeData = runtimeData;
                            GameState.Instance.AccountId = loginResult.AccountData.AccountId;
                            GameState.Instance.LoginEntityId = loginEntityId;
                            var platformApiToken = loginResult.PlatformApiToken;
                            UnityQueueHelper.RunInUnityThreadNoWait(
                                () => { GameState.Instance.PlatformApiTokenRp.Value = platformApiToken; });

                            using (var cts = new CancellationTokenSource())
                            {
                                var outer = AsyncProcessExtensions.EmptyProcess(baseCt);
                                var inner = next(cts.Token);
                                var our = repositoryHost.DisconnectionTask;

                                try
                                {
                                    await Task.WhenAny(our, inner, outer);
                                }
                                finally
                                {
                                    cts.Cancel();
                                    await inner;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.IfError()?.Exception(e).Write();
                            throw;
                        }
                        finally
                        {
                            await repositoryHost.UnsubscribeDisconnectedEvent();
                            if (GameState.Instance != null)
                                GameState.Instance.ClientRepositoryHost = null;
                        }
                    }
                }
                finally
                {
                    Logger.IfInfo()?.Message("Client Repository Stopping...").Write();

                    PawnDataSharedProxy.Clean(); // Clearing static debug cache
                    cachedGameState.CharacterRuntimeData = null;
                    ClientCommRuntimeDataProvider.CharRuntimeData = null;
                    cachedGameState.ClientClusterNode = null;
                    NodeAccessor.Repository = null;

                    using (var loginWrapToLogout = await repository.GetFirstService<ILoginServiceEntityClientFull>())
                    {
                        var loginServiceEntity = loginWrapToLogout?.GetFirstService<ILoginServiceEntityClientFull>();
                        if (loginServiceEntity != null)
                        {
                            Logger.IfInfo()?.Message("Client Repository Logout Process In LoginServiceEntity").Write();
                            await loginServiceEntity.Logout();
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                    await repository.Stop();

                    Logger.IfInfo()?.Message("Client Repository Stopped Successfully").Write();
                    ;
                }
            }
        }
    }
}