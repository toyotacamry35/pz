using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.GameObjectAssembler.Res
{
    public abstract class ComponentDef : BaseResource, IComponentDef
    {
        public bool ReuseExisting { get; set; } = false;
    }

    public interface IComponentDef : IResource
    {
        bool ReuseExisting { get; set; }
    }
}
