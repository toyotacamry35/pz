using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;

namespace SharedCode.Entities.GameObjectEntities
{
    public interface INonEntityObjectDef : IResource
    {
        LocalizedString NameLs { get; set; }
        ResourceRef<SpellDef> SpellDef { get; set; }
    }
}