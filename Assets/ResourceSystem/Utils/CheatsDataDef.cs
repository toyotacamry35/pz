using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class CheatsDataDef : BaseResource
    {
        // ReSharper disable once InconsistentNaming
        public ResourceRef<SpellDef> KillAllObjectsAround_20Spell  { get; set; }
        // ReSharper disable once InconsistentNaming
        public ResourceRef<SpellDef> KillAllObjectsAround_50Spell  { get; set; }
        // ReSharper disable once InconsistentNaming
        public ResourceRef<SpellDef> KillAllObjectsAround_100Spell { get; set; }
    }

}
