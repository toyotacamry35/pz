using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src;
using ColonyDI;
using SharedCode.Logging;
using Src.Debugging;
using UnityEngine;

namespace Uins.Intro
{
    public class IntroLogic : MonoBehaviour, IDependencyNode, IStartupIntermediateLogic
    {
        [SerializeField] private IntroPlayer _introPlayer;
        
        public IDependencyNode Parent { get; set; }
        public IEnumerable<IDependencyNode> Children => Enumerable.Empty<IDependencyNode>();

        public void AfterDependenciesInjected()
        {}

        public void AfterDependenciesInjectedOnAllProviders()
        {}

        public async Task<bool> CanStart() => await UnityQueueHelper.RunInUnityThread(() => !DebugState.Instance.SkipIntro && !StartupParams.Instance.AutoConnect && !StartupParams.Instance.PlayParams.AutoPlay);

        public bool MustBeUnloadedAfterLogic => true;

        public Task DoLogic(CancellationToken ct)
        {
            Log.StartupStopwatch.Milestone("IntroLogic");
            return _introPlayer.Play(ct);
        }
    }
}