using Assets.Src.GuiRoot;
using ColonyDI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.EntitySystem;
using SharedCode.Serializers.Protobuf;
using UnityEngine;
using Infrastructure.Cloud;
using System.Threading;
using ResourceSystem.Reactions;

namespace Assets.Src.Lobby
{
    public class LobbyStartup : MonoBehaviour, IDependencyNode, IStartupLogic
    {
        public IDependencyNode Parent { get; set; }

        public IEnumerable<IDependencyNode> Children => Enumerable.Empty<IDependencyNode>();

        [Dependency]
        private LobbyGuiNode LobbyNode { get; set; }

        private readonly StartupParams _startupParams = StartupParams.Instance;

        public Task<bool> CanStart() => Task.FromResult(true);

        public void AfterDependenciesInjected()
        {
        }

        public void AfterDependenciesInjectedOnAllProviders()
        {
        }

        public Task DoStart(Task probufInitTask, InnerProcess inner, CancellationToken ct)
        {
            AsyncProcess enterLobby = (innerInner, ctInner) =>
                LoadLobbyProcess.EnterLobbySequenceInf(LobbyNode, innerInner, ctInner);
            AsyncProcess hideLoadingScreen = (innerInner, ctInner) =>
                LoadLobbyProcess.HideLoadingScreenInf(LobbyNode.LoadingScreenUtility, innerInner, ctInner);
            AsyncProcess main = async (innerInner, ctInner) =>
            {  
            	await probufInitTask;
                await GameState.Instance.MainLobbyProcess(innerInner, ctInner);
            };
            AsyncProcess autoConnect = (innerInner, ctInner) =>
                LoadLobbyProcess.SetStartupParams(LobbyNode, _startupParams, innerInner, ctInner);
            
            return enterLobby.Then(main).Then(autoConnect).Then(hideLoadingScreen).Exec(inner, ct);
        }
    }
}