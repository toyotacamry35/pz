using Assets.Src.GameObjectAssembler.Res;

namespace Assets.ResourceSystem.Aspects.Misc
{
    // We can't no more init Unity native components from def directly (big overhead). So there is special proxy component
    //.. `UnityComponentsFromDefProxyInitializer`, which is filled from its def (`UnityComponentsFromDefProxyInitializerDef`)
    //.. and inits Unity native components fields at its awake.
    public class UnityComponentsFromDefProxyInitializerDef : ComponentDef
    {
        public float NavMeshAgentStoppingDistance { get; set; }
    }
}
