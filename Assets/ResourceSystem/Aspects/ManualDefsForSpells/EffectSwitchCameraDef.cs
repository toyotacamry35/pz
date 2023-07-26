using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectSwitchCameraDef : SpellEffectDef
    {
        public ResourceRef<CameraDef> Camera;
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}