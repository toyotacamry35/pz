using Assets.Src.Aspects.Impl;
using SharedCode.Wizardry;
using System.Collections.Generic;

namespace Assets.Src.InteractionSystem
{
    public class SpellVariant
    {
        public SpellDef Spell { get; internal set; }
        public IEnumerable<object> Conditions { get; internal set; }
    }
}