using Assets.Src.Character.Events;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.SpawnSystem
{
    public class VisualEventProxyDef : ComponentDef
    {
        public ResourceRef<FXEventsDef> Events { get; set; }
    }
}