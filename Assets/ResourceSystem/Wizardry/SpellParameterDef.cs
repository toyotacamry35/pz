using System;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Wizardry
{
    [Obsolete] // Вместо него надо использовать ISpellParameterDef
    public abstract class SpellParameterDef : BaseResource {}

    [Obsolete]
    public class SpellParameterDirection3Def : SpellParameterDef
    {
        public float X, Y, Z;
    }
    
    [Obsolete]
    public class SpellParameterDirection2Def : SpellParameterDef
    {
        public float X, Y;
    }

    [Obsolete]
    public class SpellParameterDirectionDef : SpellParameterDirection3Def {}

    [Obsolete]
    public class SpellParameterPrevSpellIdDef : SpellParameterDef
    {}

    [Obsolete]
    public class SpellParameterDurationDef : SpellParameterDef
    {
        public float Duration;
    }
}
