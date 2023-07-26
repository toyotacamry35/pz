using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Entities
{
    public interface IHasStatsDef
    {
        ResourceRef<StatsDef> Stats { get; set; }
    }
}

