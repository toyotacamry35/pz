using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Aspects.Item.Templates
{
    public class CharacterPawnSchedulerDef : BaseResource
    {
        public float NearDistance;
        public float FarDistance;
        public int MinCharacters;
        public int MaxCharacters;
        public float ThrottleOnFarDistance;
        public float ThrottleOnMaxCharacters;
    }
}