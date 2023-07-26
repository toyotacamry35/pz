using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Entities.GameObjectEntities;

namespace SharedCode.Aspects.Item.Templates
{
    /// <summary>
    /// Данные по установке на карте Entity-объекта
    /// </summary>
    [Localized]
    public class MountingData
    {
        public ResourceRef<IEntityObjectDef> EntityObjectDef { get; set; }
        public string ActionTitle { get; set; }
        public LocalizedString ActionTitleLs { get; set; }
    }
}