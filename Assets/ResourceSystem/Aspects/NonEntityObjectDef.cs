using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;

namespace SharedCode.Entities.GameObjectEntities
{
    public class NonEntityObjectDef : BaseResource, INonEntityObjectDef
    {
        public LocalizedString NameLs { get; set; }
        public ResourceRef<SpellDef> SpellDef { get; set; }
    }
}