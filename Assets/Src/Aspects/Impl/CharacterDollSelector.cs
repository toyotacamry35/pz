using System.Linq;
using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.Aspects.Impl.Factions.Template;
using ResourceSystem.Aspects.Misc;

namespace Src.Aspects.Impl
{
    public static class CharacterDollSelector
    {
        public static VisualDollDef SelectDoll(WorldCharacterDef def, MutationStageDef mutation, GenderDef gender)
        {
            foreach (var defBody in def.Bodies)
                if ((defBody.Value.Genders == null || defBody.Value.Genders.Count == 0 || defBody.Value.Genders.Any(x => x == gender)) &&
                    (defBody.Value.Mutations == null || defBody.Value.Mutations.Count == 0 || defBody.Value.Mutations.Any(x => x == mutation)))
                    return defBody.Key;
            return null;
        }
    }
}