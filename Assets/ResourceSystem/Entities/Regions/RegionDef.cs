using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace SharedCode.Entities.Regions
{
    public class RegionDef : BaseResource
    {
        public string Tag = string.Empty;
        public SpellDef WhileInsideSpell { get; set; }
        public int AmbientPriority { get; set; }
        public string AmbientName { get; set; }
        public string AmbientTypeName { get; set; }
    }
}