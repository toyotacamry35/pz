using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using ResourceSystem.Aspects.Misc;
using SharedCode.Wizardry;

namespace SharedCode.Aspects.Item.Templates
{

    public class CharacterConstantsResource : BaseResource
    {   
        public ResourceRef<WorldCharacterDef> WorldCharacter { get; set; }
        public ResourceArray<GenderDef> Genders { get; set; }
        public GenderDef DefaultGender => Genders[0];
    }
}
