using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.InputActions
{
    public readonly struct InputActionHandlerContext
    {
        public readonly SpellId CurrentSpell;

        public InputActionHandlerContext(SpellId currentSpell = default)
        {
            CurrentSpell = currentSpell;
        }

        public override string ToString() => $"CurrentSpell:{CurrentSpell.ToString()}";
    }
}