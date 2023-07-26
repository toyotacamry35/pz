using JetBrains.Annotations;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactGrantLootTableItemDef : SpellImpactDef
    {
        public bool DieAfterLoot { get; [UsedImplicitly] set; }
    }
}
