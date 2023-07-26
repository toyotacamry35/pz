namespace GeneratedDefsForSpells
{
    public class ImpactStopAllSpellsOfGroupsDef : SharedCode.Wizardry.SpellImpactDef
    {
        public Assets.Src.ResourcesSystem.Base.ResourceRef<SharedCode.Wizardry.SpellEntityDef> Target { get; set; }
        public System.Collections.Generic.List<Assets.Src.ResourcesSystem.Base.ResourceRef<SharedCode.Wizardry.SpellGroupDef>> Groups { get; set; }
        public FinishReasonType Reason { get; set; } = FinishReasonType.Success;
    }

    // --- Internal types: ----
    public enum FinishReasonType
    {
        None,   // invalid value
        Success,// finished correctly
        Fail    // predicates failed
    }
}
