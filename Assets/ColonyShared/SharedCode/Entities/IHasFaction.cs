using Assets.Src.Aspects.Impl.Factions.Template;
using SharedCode.EntitySystem;

namespace ColonyShared.SharedCode.Entities
{
    public interface IHasFaction
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        FactionDef Faction { get; set; }
    }
}