using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IMortalObjectDef
    {
        // Game-designer mistake fuse. Leave it false (by dflt) and death 'll be triggered by timeout after `OnZeroHealthSpell` casted.
        // #NOTE: It's just fuse for safety - don't rely on it - call Die yourself from `OnZeroHealthSpell`
        bool DisablePreDeathHandlerAutoDeathByTimeout { get; set; }
        bool KnockedDownOnZeroHealth { get; set; }
        // --- Candidate to move into separate def of `PreDeathHandler`: -------------------------------------------------

        // If is set, Mortal logic 'll not call `Die` when 0-health reached. It'll just cast this spell
        //.. And in this case provider of this spell is totally responsible for calling `Die` from spell
        //.. (or return entty to consistent condition in any other way)
        ResourceRef<SpellDef> OnZeroHealthSpell { get; set; }
        ResourceRef<SpellDef> KnockDownSpell { get; set; }
        ResourceRef<SpellDef>[] SpellsOnResurrect { get; set; }
        bool IsHpRegenAllowedInPreDeathState    { get; set; }
        
    }
}
