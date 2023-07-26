using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem.Statistics;
using GeneratedCode.Infrastructure.Cloud;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Network.Statistic;
using GeneratedCode.Repositories;
using Infrastructure.Config;
using NLog;
using NLog.Fluent;
using Prometheus;
using ResourcesSystem.Loader;
using ResourceSystem.Reactions;
using SharedCode.Cloud;
using SharedCode.Config;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Logger;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;
using SharedCode.Utils.Threads;
using Telemetry;

namespace Infrastructure.Cloud
{
    public class CloudHost : ICloudHost
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private Process[] Processes { get; }

        public IReadOnlyDictionary<RepositoryId, IEntitiesRepository> Repositories { get; }

        private CloudHost(Process[] processes, IReadOnlyDictionary<RepositoryId, IEntitiesRepository> repositories)
        {
            Processes = processes;
            Repositories = repositories;
        }
        public static CloudHost ClusterNode { get; set; }

        public static async Task InitContainer(
            ContainerConfig containerConfig,
            CloudSharedDataConfig sharedConfig,
            string resourceRoot,
            IEntitySubscriptionsProcessor entitySubscriptionsProcessor,
            InnerProcess<ICloudHost> inner,
            CancellationToken ct)
        {
            ContainerNameLayoutRenderer.ContainerName = containerConfig.ToString();

            var currentProcessId = Process.GetCurrentProcess().Id;

            if (sharedConfig?.WebServicesConfig.Target?.Prometheus.Target?.ProcessMetricsEnabled ?? false)
                MonitoringCliInitializer.Start(currentProcessId, $"{Environment.MachineName}:{currentProcessId}",
                    sharedConfig.WebServicesConfig.Target.Prometheus.Target.PushGatewayAddress);

            if (sharedConfig?.WebServicesConfig.Target?.Elastic.Target?.Enabled ?? false)
                ElasticAccessor.Init(sharedConfig.WebServicesConfig.Target.Elastic.Target.Address, sharedConfig.WebServicesConfig.Target.Elastic.Target.Login,
                    sharedConfig.WebServicesConfig.Target.Elastic.Target.Password);
            
            using (CoreStatisticsManager.Init())
            using (PrometheusStatistic.Start("Server", $"{Environment.MachineName}:{currentProcessId}", sharedConfig?.WebServicesConfig.Target?.RealmId?? "empty", sharedConfig?.WebServicesConfig.Target?.Prometheus))
            {
                var hangDetectorSettings = new FreezeDetectorSettings(
                    TimeSpan.FromMilliseconds(50),
                    Metrics.CreateHistogram("thread_sleep", String.Empty, new HistogramConfiguration
                    {
                        Buckets = new double[] {50, 60, 70, 80, 90, 100, 150, 500, 1000, 2000, 5000, 10000}
                    }),
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(100),
                    Metrics.CreateHistogram("thread_pool_sleep", String.Empty, new HistogramConfiguration
                    {
                        Buckets = new double[] {100, 110, 120, 130, 140, 150, 200, 500, 1000, 2000, 5000, 10000}
                    }),
                    TimeSpan.FromMilliseconds(1000));
                // should be after prometheus because we use SuppressDefaultMetrics
                using (FreezeDetectorFactory.Start(hangDetectorSettings))
                {
                    var processes = new List<Process>();
                    foreach (var container in containerConfig.ExternalContainers)
                    {
                        var process = await RunExternalNode(sharedConfig, container, resourceRoot);
                        if (process != null)
                            processes.Add(process);
                    }

                    var repositories = await InitInternalContainerImpl(sharedConfig, entitySubscriptionsProcessor, containerConfig, ct);

                    var host = new CloudHost(processes.ToArray(), repositories);
                    ClusterNode = host;
                    try
                    {
                        await inner(host, ct);
                    }
                    finally
                    {
                        await host.Stop();
                        ClusterNode = null;
                    }
                }
            }
        }

        private static async Task WaitForFirstException(IEnumerable<Task> tasks)
        {
            var taskList = tasks.ToList();
            while(taskList.Any())
            {
                var task = await Task.WhenAny(taskList);
                await task;
                taskList.Remove(task);
            }
        }

        private static async Task<IReadOnlyDictionary<RepositoryId, IEntitiesRepository>> InitInternalContainerImpl(
            CloudSharedDataConfig sharedConfig,
            IEntitySubscriptionsProcessor entitySubscriptionsProcessor,
            ContainerConfig cfg,
            CancellationToken ct)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            try
            {
                ServerCoreRuntimeParameters.Config = cfg;
                ProtoBufSerializer.TryInit(new[] { typeof(IEntity).Assembly, typeof(IResource).Assembly, typeof(ArgDef).Assembly });
                var serializer = new ProtoBufSerializer();
                var entitySerializer = new EntitySerializer(serializer);

                var internalRepos = cfg.EntitiesRepositories;
                var repositories = await CreateRepositories(sharedConfig, entitySubscriptionsProcessor, internalRepos, serializer, entitySerializer);

                using (var cts = new CancellationTokenSource())
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ct))
                {
                    Task[] tasks = repositories.Select(v => ((EntitiesRepository)v.Value).ConnectToCluster(linkedCts.Token)).ToArray();
                    try
                    {
                        await WaitForFirstException(tasks);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            cts.Cancel();
                            await Task.WhenAll(tasks);
                        }
                        catch (Exception)
                        {
                        }
                        throw;
                    }
                }

                ClusterIsReadyVisualizer.PrintClusterIsReadyToConsole();

                return repositories;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "InitInternalContainerImpl exception").Write();;
                throw;
            }
            finally
            {
                w.Stop();
                Logger.IfInfo()?.Message($"InitInternalProcess took {w.Elapsed.TotalSeconds}").Write();
            }
        }

        public static async Task<Dictionary<RepositoryId, IEntitiesRepository>> CreateRepositories(
            CloudSharedDataConfig sharedConfig, 
            IEntitySubscriptionsProcessor entitySubscriptionsProcessor,
            ResourceRef<EntitiesRepositoryConfig>[] internalRepos, 
            ProtoBufSerializer serializer,
            EntitySerializer entitySerializer)
        {
            var repositories = new Dictionary<RepositoryId, IEntitiesRepository>();
            foreach (var entitiesRepositoryConfig in internalRepos)
            {
                for (int i = 0; i < entitiesRepositoryConfig.Target.Count; i++)
                {
                    repositories.Add(GetRepositoryId(entitiesRepositoryConfig.Target.ConfigId, i),
                        await CreateEntitiesRepository(
                            sharedConfig,
                            entitiesRepositoryConfig,
                            i,
                            serializer,
                            entitySerializer,
                            entitySubscriptionsProcessor));
                }
            }

            return repositories;
        }

        private static RepositoryId GetRepositoryId(string id, int num)
        {
            return new RepositoryId(id, num);
        }


        private static async Task<IEntitiesRepository> CreateEntitiesRepository(
            CloudSharedDataConfig sharedConfig, 
            EntitiesRepositoryConfig repositoryConfig, 
            int num, 
            ISerializer serializer, 
            IEntitySerializer entitySerializer, 
            IEntitySubscriptionsProcessor entitySubscriptionsProcessor)
        {
            var repository = new EntitiesRepository(sharedConfig, repositoryConfig, CloudNodeType.Server, num, serializer, entitySerializer, entitySubscriptionsProcessor);
            try
            {
                await repository.InitCommunicationNode();
                return repository;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "CreateEntitiesRepository error").Write();
                throw new RepositoryCriticalException("InitCommunicationNode", e);
            }
        }

        private static string CreateArgs(string extraArgs, CloudSharedDataConfig sharedConfig, ContainerConfig containerConfig, string resourceRootForExternalNodes)
        {
            if (containerConfig == null)
                return extraArgs;

            ResourceIDFull repoCfgPath = GameResourcesHolder.Instance.GetID(containerConfig);
            ResourceIDFull sharedConfigPath = GameResourcesHolder.Instance.GetID(sharedConfig);
            return $"{extraArgs} --watch-pid {Process.GetCurrentProcess().Id} --container-config {repoCfgPath} --shared-config {sharedConfigPath} -logFile - --resource-system-root {resourceRootForExternalNodes}";
        }

        private static async Task<Process> RunExternalNode(CloudSharedDataConfig sharedConfig, ExternalContainerConfig containerConfig, string resourceRootForExternalNodes)
        {
            var command = containerConfig.Commands.Where(v => v != null).Select(v => (new FileInfo(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), v.Target.Path)), v.Target.Arguments)).FirstOrDefault(v => v.Item1.Exists);

            if (command == default)
                throw new FileNotFoundException($"External node executable file(s) not found: {string.Join(",", containerConfig.Commands.Select(v => v.Target.Path))}");

            var arguments = CreateArgs(command.Arguments, sharedConfig, containerConfig.Container, resourceRootForExternalNodes);
            var process = ExternalProcessWrap.Start(command.Item1, arguments);
            await Task.Delay(1000);
            return process;
        }

        private async Task Stop()
        {
            Logger.IfInfo()?.Message("Stopping cloud host ...").Write();

            try
            {
                await Task.WhenAll(Repositories.Values.Cast<EntitiesRepository>().ToList().Select(v => v.Stop()));
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Stop server repository exception").Write();;
            }

            foreach (var process in Processes)
            {
                try
                {
                    Logger.IfInfo()?.Message("Kill process {0} id {1}", process.ProcessName, process.Id).Write();
                    process.Kill();
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "Kill process {0} id {1}", process.ProcessName, process.Id).Write();
                }
            }

            Logger.IfInfo()?.Message("Cloud host stopped").Write();
        }
    }
}
