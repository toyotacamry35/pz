namespace GeneratedDefsForSpells
{
    public class MarkerSpellAttributeDef : SharedCode.Wizardry.SpellWordDef
    {
        public SpellMarkerType Marker { get; set; }
    }

    public enum SpellMarkerType
    {
        None,
        PartialUnInterruptable,
    }
}
