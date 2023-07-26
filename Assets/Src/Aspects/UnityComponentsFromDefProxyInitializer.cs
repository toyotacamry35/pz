using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Src.Aspects
{
    // We can't no more init Unity native components from def directly (big overhead). So there is special proxy component
    //.. `UnityComponentsFromDefProxyInitializer`, which is filled from its def (`UnityComponentsFromDefProxyInitializerDef`)
    //.. and inits Unity native components fields at its awake.
    public class UnityComponentsFromDefProxyInitializer : MonoBehaviour
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [UsedImplicitly] // is set from def
        private float _navMeshAgentStoppingDistance;

        private void Awake()
        {
            NavMeshAgent nma = GetComponent<NavMeshAgent>();
            if (nma != null)
                nma.stoppingDistance = _navMeshAgentStoppingDistance;
            else
                Logger.IfError()?.Message($"`{nameof(UnityComponentsFromDefProxyInitializer)}` can't get `{nameof(NavMeshAgent)}`").Write();
        }
    }
}
