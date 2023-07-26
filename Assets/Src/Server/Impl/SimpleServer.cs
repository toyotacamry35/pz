using System;
using System.Linq;
using Assets.ColonyShared.SharedCode.Shared;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Entities.Service;
using SharedCode.Config;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Config;
using Infrastructure.Cloud;
using SharedCode.OurSimpleIoC;
using System.Threading;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using SharedCode.MovementSync;
using ResourcesSystem.Loader;

namespace Assets.Src.Server.Impl
{
    class SimpleServer : IServer
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private const string ClusterHostName = "main";
        public bool Host { get; }

        private SimpleServer(bool host)
        {
            Host = host;
        }
        public static Task RunServer(ContainerConfig cfg, CloudSharedDataConfig shared, InnerProcess next, CancellationToken ct)
        {
            AsyncProcessOut<ICloudHost> nodeProcess = (inner, ctInner) => InitClusterNode(cfg, shared, inner, ctInner);
            AsyncProcessIn<ICloudHost> pci = (host, inner, ctInner) => PostClusterInit(false, host, inner, ctInner);
            return nodeProcess.Then(pci).Exec(next, ct);
        }

        public static Task RunHost(ContainerConfig serverConfig, CloudSharedDataConfig sharedConfig, InnerProcess next, CancellationToken ct)
        {
            //Logger.IfWarn()?.Message("START LOADING RESOURCES").Write();
            //((IGameResourcesRandomExtension)GameResourcesHolder.Instance).LoadAllResources();
            //Logger.IfWarn()?.Message("FINISH LOADING RESOURCES").Write();
            var cloudHostConfig = serverConfig;
            if (cloudHostConfig == null)
                throw new InvalidOperationException("Cant find host with id " + ClusterHostName);

            AsyncProcessOut<ICloudHost> clusterProcess = (inner, ctInner) => InitClusterNode(serverConfig, sharedConfig, inner, ctInner);
            // AsyncProcessIn<ICloudHost> pci = (host, inner, ctInner) => PostClusterInit(true, host, inner, ctInner);
            return clusterProcess.Exec(next, ct);
        }

        private static async Task PostClusterInit(bool isHost, ICloudHost host, InnerProcess next, CancellationToken ct)
        {
            var unityRepo = await FindUnityRepo(host.Repositories.Values.ToArray());
            if (unityRepo == null)
                throw new InvalidOperationException("Cannot find unity repository");

            // ServicesPool.Services.Register<IWorldSpaceVisibilitySystemProvider>(new WorldSpaceVisibilitySystemProvider(unityRepo));

            NodeAccessor.Repository = unityRepo;
            ServerProvider.IsServer = true;
            ServerProvider.ServerInited = true;
            VisibilityGrid.ClearAll();

            try
            {
                await next(ct);
            }
            finally
            {
                // ServerProvider.Server = null;
                //NodeAccessor.Repository = null;

                ServicesPool.Clear();
            }
        }

        private static async Task InitClusterNode(ContainerConfig containerConfig, CloudSharedDataConfig sharedConfig, InnerProcess<ICloudHost> inner, CancellationToken ct)
        {
             Logger.IfInfo()?.Message("Starting node...").Write();;
            var dataPath = await UnityQueueHelper.RunInUnityThread(() => UnityEngine.Application.dataPath);
            System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            w.Start();
            await CloudHost.InitContainer(containerConfig, sharedConfig, dataPath, new EntitySubscriptionsProcessor(),  inner, ct);
            w.Stop();
            Logger.IfWarn()?.Message($"InitClustedNode took {w.Elapsed.TotalSeconds}").Write();
        }

        private static async Task<bool> IsUnityRepo(IEntitiesRepository repo)
        {
            using (var wrapper = await repo.Get<IWorldNodeServiceEntity>(repo.Id))
            {
                return wrapper.TryGet<IWorldNodeServiceEntity>(repo.Id, out var _);
            }
        }

        private static async Task<IEntitiesRepository> FindUnityRepo(IReadOnlyList<IEntitiesRepository> repos)
        {
            foreach (var repo in repos)
            {
                if (await IsUnityRepo(repo))
                    return repo;
            }
            return null;
        }

    }

    public static class ServerProvider
    {
        public static bool ServerInited { get; set; }

        public static bool IsServer { get; set; }

        public static bool IsClient => !IsServer;
    }
}