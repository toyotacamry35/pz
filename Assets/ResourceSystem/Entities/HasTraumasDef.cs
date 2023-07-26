using Assets.Src.Aspects.Impl.Traumas.Template;
using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Entities
{
    public interface IHasTraumasDef : IResource
    {
        ResourceRef<TraumasDef> AllTraumas { get; set; }
    }
}
