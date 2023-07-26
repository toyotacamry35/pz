using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;

namespace SharedCode.Entities
{
    public interface IHasPackDef
    {
        PackDef PackDef { get; set; }
    }
}

