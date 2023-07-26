namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasLifespanDef
    {
        // "0" means HasLifespan functionality 'll be off.
        float LifeSpanSec             { get; set; }
        OnLifespanExpired DoOnExpired { get; set; }
    }
}
