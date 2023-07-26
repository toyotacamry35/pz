using System.Collections.Generic;
using ResourceSystem.Aspects;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Modifiers
{
    public static partial class SpellModifierExtensions
    {
        public static IEnumerable<SpellDef> GetCastExtraSpellModifier(this IReadOnlyList<SpellModifierDef> modifiers)
        {
            if (modifiers != null)
                foreach (var modifier in modifiers)
                    if (modifier is CastExtraSpellModifierDef castModifier && castModifier.Spell != null)
                        yield return castModifier.Spell;
        }
    }
}