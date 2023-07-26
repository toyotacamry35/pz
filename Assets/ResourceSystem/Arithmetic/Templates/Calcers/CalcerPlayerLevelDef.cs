using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ResourceSystem.Account;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerPlayerLevelDef : CalcerDef<float>
    {
        //public ResourceRef<LevelTableDef> LevelTable { get; set; }
        public ResourceRef<LevelUpDatasDef> LevelTable { get; set; }
}
}
