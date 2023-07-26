using Assets.Src.App;
using Assets.Src.BuildingSystem;
using Assets.Src.Client;
using Assets.Src.Tools;
using ColonyDI;
using Infrastructure.Cloud;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using Uins;
using UnityEngine;

namespace Assets.Src.GuiRoot
{
    public class BotStartup : MonoBehaviour, IDependencyNode, IStartupLogic
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Bot");

        private const string Version = "Best version ever";

        public IDependencyNode Parent { get; set; }

        public IEnumerable<IDependencyNode> Children => Enumerable.Empty<IDependencyNode>();

        public Task<bool> CanStart() => Task.FromResult(Environment.GetCommandLineArgs().Any(x => x.StartsWith("-bot")));

        public void AfterDependenciesInjected()
        {
        }

        public void AfterDependenciesInjectedOnAllProviders()
        {
        }

        public async Task DoStart(Task protoBufInit, InnerProcess inner, CancellationToken ct)
        {
            await protoBufInit;
            await UnityQueueHelper.RunInUnityThread(DevelopLobbyGuiNode.Init_Log_System);
            
            var (botConnectIp, botConnectPort, realmRulesQuery, player) = CmdArgumentHelper.GetConnectParams();

            var botConnectionParams = CmdArgumentHelper.GetBotParams();

            var cParams = new ConnectionParams(
                botConnectIp,
                botConnectPort,
                Version,
                default,
                botConnectionParams[0]._spawnPoint,
                "",
                ""
            );

            SceneStreamerSystemCheat.StreamerLoadAllCheat(true);

            try
            {
                Cluster.Cheats.ClusterCheats.Initialize_Bots_System(); // Before connect we should initialize Bots system

                AsyncProcess connectProc = (innerInner, ctInner) => ClientRepositoryProcessRunner.StartInf(
                    GameState.Instance.DefaultConfigDef.Target,
                    cParams,
                    string.Empty,
                    innerInner,
                    ctInner);

                AsyncProcess findRealm = (innerInner, ctInner) => GameState.Instance.ClientRepositoryHost.FindGame(
                    GameState.Instance.AccountId,
                    realmRulesQuery,
                    innerInner,
                    ctInner);

                AsyncProcess playGame = (innerInner, ctInner) => GameState.Instance.ClientRepositoryHost.EnterGame(
                    GameState.Instance.AccountId,
                    false,
                    innerInner,
                    ctInner);

                AsyncProcess spawnBotsProc = (innerInner, ctInner) => SpawnBotsImpl(botConnectionParams, innerInner, ctInner);

                await connectProc.Then(findRealm).Then(playGame).Then(ConnectProcess.SwitchIngameFlag).Then(spawnBotsProc).Exec(inner, ct);

                Logger.IfFatal()?.Message("Bot process finished").Write();
            }
            catch (Exception e)
            {
                Logger.IfFatal()?.Message(e, "Bot process failed with exception").Write();
                throw;
            }
            finally
            {
                Logger.IfFatal()?.Message("Bot process finished, terminating application").Write();
                await UnityQueueHelper.RunInUnityThread(() => Application.Quit());
            }
        }

        private async Task SpawnBotsImpl(BotConnectionParams[] botConnectionParams, InnerProcess inner, CancellationToken ct)
        {
            foreach (var botConnectionParam in botConnectionParams)
            {
                ct.ThrowIfCancellationRequested();

                await UnityQueueHelper.RunInUnityThread(
                    () =>
                    {
                        Cluster.Cheats.ClusterCheats.Spawn_Bot(
                            botConnectionParam._count,
                            botConnectionParam._botBrainDefPath,
                            botConnectionParam._spawnPoint);
                    });
            }

            await inner(ct);
        }
    }
}
