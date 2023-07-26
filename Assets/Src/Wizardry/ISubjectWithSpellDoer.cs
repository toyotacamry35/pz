using SharedCode.Wizardry;

namespace Assets.Src.Wizardry
{
    public interface ISubjectWithSpellDoer
    {
        ISpellDoer SpellDoer { get; }
    }
}