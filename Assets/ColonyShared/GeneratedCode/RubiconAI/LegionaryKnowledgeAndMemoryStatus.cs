using System.Collections.Generic;

namespace Assets.Src.RubiconAI
{
    public class LegionaryKnowledgeAndMemoryStatus : Detective.Event
    {
        public Dictionary<Legionary, RememberedLegionary> RememberedLegionaries = new Dictionary<Legionary, RememberedLegionary>();
        public KnowledgeSnapshot KnowledgeSnapshot { get; set; }
        public MemorySnapshot MemorySnapshot { get; set; }
        public CooldownsStatus Cooldowns { get; set; }
        public RememberedLegionary GetMemoryOfLegionary(Legionary legionary)
        {
            if (RememberedLegionaries.ContainsKey(legionary))
                return RememberedLegionaries[legionary];
            else
            {
                var remLeg = new RememberedLegionary(legionary);
                RememberedLegionaries.Add(legionary, remLeg);
                return remLeg;
            }
        }
    }
}
