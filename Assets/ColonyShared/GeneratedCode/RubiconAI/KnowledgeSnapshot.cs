using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.KnowledgeSystem;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Src.RubiconAI
{
    public class KnowledgeSnapshot
    {
        public Dictionary<BaseResource, List<RememberedLegionary>> Legionaries = new Dictionary<BaseResource, List<RememberedLegionary>>();
        public KnowledgeSnapshot(Knowledge knowledge, MemorySnapshot memorySnapshot, LegionaryKnowledgeAndMemoryStatus status)
        {
            Select(knowledge, memorySnapshot, status);
        }

        private void Select(Knowledge knowledge, MemorySnapshot memorySnapshot, LegionaryKnowledgeAndMemoryStatus status)
        {
            foreach (var source in knowledge.KnownStuff)
            {
                List<RememberedLegionary> legs = null;
                if (!Legionaries.TryGetValue(source.Value.GetId(), out legs))
                    Legionaries.Add(source.Value.GetId(), legs = new List<RememberedLegionary>());
                legs.AddRange(source.Value.Legionaries.Select(x => { Legionary.LegionariesByRef.TryGetValue(x.Key, out var leg); return leg; }).Where(x => x != null).Select(x => status.GetMemoryOfLegionary(x)));
            }
            if (knowledge.TranscendentKnowledge != null)
            {
                memorySnapshot.LegionMemory = new MemorySnapshot(knowledge.TranscendentKnowledge.Memory, status);
                Select(knowledge.TranscendentKnowledge, memorySnapshot.LegionMemory, status);
            }
        }
    }
}
