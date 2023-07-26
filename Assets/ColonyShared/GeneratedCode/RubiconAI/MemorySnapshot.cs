using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Src.RubiconAI
{
    public class MemorySnapshot
    {
        public Dictionary<RememberedLegionary, Memory> RememberedLegionaries = new Dictionary<RememberedLegionary, Memory>();
        public MemorySnapshot LegionMemory;
        
        public MemorySnapshot(TimedMemory memory, LegionaryKnowledgeAndMemoryStatus status)
        {
            var list = new List<(MemoryPieceHandle, MemoryData)>();
            memory.GetMemoryPieces(list);
            foreach (var legionary in list.GroupBy(x => x.Item1.About))
            {
                RememberedLegionaries.Add(status.GetMemoryOfLegionary(legionary.Key), new Memory(legionary));
            }
            foreach (var mem in RememberedLegionaries)
            {
                foreach (var stat in mem.Value.Stats)
                    foreach (var statMod in stat.Value.Mods)
                        if (statMod.Key.Assigner != null)
                            status.GetMemoryOfLegionary(statMod.Key.Assigner);
            }
        }
    }
}
