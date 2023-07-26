namespace SharedCode.Wizardry
{
    public abstract class SpellImpactDef : SpellWordDef
    {
        public SpellImpactTiming WhenToApply { get; set; }
        public virtual bool UnityAuthorityServerImpact => false;
    }
}
