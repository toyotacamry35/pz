using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.Server.Impl;
using Infrastructure.Cloud;
using Infrastructure.Config;
using JetBrains.Annotations;
using NLog;
using SharedCode.Config;
using System;
using System.Threading;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Serializers.Protobuf;
using UnityEngine;
using Utilities;
using System.IO;
using Core.Environment.CommandLine;
using System.Linq;
using Core.Environment.Logging.Extension;
using ResourceSystem.Reactions;
using SharedCode.Serializers;

namespace Assets.Src.Server
{
    public static class ServerDependencyNode
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static async void RegisterForQuit(int pid)
        {
            try
            {
                await Core.Environment.PidWatcher.Watch(pid);
            }
            finally
            {
                await UnityQueueHelper.RunInUnityThread(Application.Quit);
            }
        }

        private static async Task InitClusterNode(
            ResourceIDFull containerConfig,
            ResourceIDFull sharedConfig,
            int watchPid)
        {
             Logger.IfInfo()?.Message("Try init cluster node").Write();;

            if (watchPid > 0)
                RegisterForQuit(watchPid);

            var containerConfigDef = GameResourcesHolder.Instance.LoadResource<ContainerConfig>(containerConfig);
            var sharedConfigDef = GameResourcesHolder.Instance.LoadResource<CloudSharedDataConfig>(sharedConfig);

            try
            {
                

                AsyncProcess serverProc = (innerInner, ctInner) =>
                    SimpleServer.RunServer(containerConfigDef, sharedConfigDef, innerInner, ctInner);
                await serverProc.Exec(CancellationToken.None);
                Logger.IfFatal()?.Message("Server process finished").Write();
            }
            catch (Exception e)
            {
                Logger.IfFatal()?.Message(e, "Server process finished with exception").Write();
            }
            finally
            {
                await UnityQueueHelper.RunInUnityThread(Application.Quit);
            }
        }

        //=== Private =========================================================

        public static async Task<bool> CanStart()
        {
            var isHeadless = await UnityQueueHelper.RunInUnityThread(
                () => SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
            var botArgIndex = Environment.GetCommandLineArgs().IndexOf(x => x.StartsWith("-bot"));
            var isBot = botArgIndex != -1;

            return isHeadless && !isBot;
        }

        public static Task Start(InnerProcess inner, CancellationToken ct)
        {
            Task Fn(
                ResourceIDFull containerConfig,
                ResourceIDFull sharedConfig,
                int watchPid,
                ResourceIDFull realmsListOverride,
                bool callPlatform,
                DirectoryInfo resourceSystemRoot)
                => InitClusterNode(
                    containerConfig,
                    sharedConfig,
                    watchPid
                );

            return NodeBootstrapper.Run(Environment.GetCommandLineArgs().Skip(1).ToArray(), Fn);
        }
    }
}