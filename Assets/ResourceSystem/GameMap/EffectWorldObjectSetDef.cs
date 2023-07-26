using Assets.Src.ResourcesSystem.Base;
using Entities.GameMapData;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectWorldObjectSetDef : SpellEffectDef
    {
        public ResourceRef<WorldObjectInformationClientSubSetDef> Set { get; set; }
    }
}
