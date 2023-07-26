using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Item.Templates
{
    public static class Constants
    {
        public const string WorldConstantsPath = "/Constants/defaults";
        public const string AttackConstantsPath = "/UtilPrefabs/AttackConstants";
        public const string ItemConstantsPath = "/Constants/ItemConstants";
        public const string CharacterConstantsPath = "/Constants/CharacterDefaults";
        public const string RelationshipConstantsPath = "/Constants/Relationship";


        public static ResourceRef<CharacterConstantsResource> CharacterConstantsRef = new ResourceRef<CharacterConstantsResource>(CharacterConstantsPath);
        public static CharacterConstantsResource CharacterConstants => CharacterConstantsRef.Target;

        public static ResourceRef<WorldConstantsResource> WorldConstantsRef = new ResourceRef<WorldConstantsResource>(WorldConstantsPath);
        public static WorldConstantsResource WorldConstants => WorldConstantsRef.Target;
        
        public static ResourceRef<AttackConstantsDef> AttackConstantsRef = new ResourceRef<AttackConstantsDef>(AttackConstantsPath);
        public static AttackConstantsDef AttackConstants => AttackConstantsRef.Target;
        
        public static ResourceRef<ItemConstantsResource> ItemConstantsRef = new ResourceRef<ItemConstantsResource>(ItemConstantsPath);
        public static ItemConstantsResource ItemConstants => ItemConstantsRef.Target;

        public static ResourceRef<RelationshipConstantsDef> RelationshipConstantsRef = new ResourceRef<RelationshipConstantsDef>(RelationshipConstantsPath);
        public static RelationshipConstantsDef RelationshipConstants => RelationshipConstantsRef.Target;
    }
}