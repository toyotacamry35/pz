using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.GameObjectAssembler.Res
{
    public class UnityGameObjectDef : BaseResource
    {
        public string Name { get; set; }
        public ResourceRef<UnityGameObjectDef>[] Children { get; set; } = System.Array.Empty<ResourceRef<UnityGameObjectDef>>();
        public ResourceRef<ComponentDef>[] Components { get; set; } = System.Array.Empty<ResourceRef<ComponentDef>>();
    }
}
