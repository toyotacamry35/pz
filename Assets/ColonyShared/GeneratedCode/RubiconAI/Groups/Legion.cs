using System.Collections.Generic;
using Assets.Src.RubiconAI.KnowledgeSystem;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Assets.Src.RubiconAI
{
    // Is used for solving: a) belonging mob to some pack(group); b) sharing knowledges; c) shared overall AI (not done yet)
    public class Legion
    {
        // For belonging to same legion ref in Legionary.`AssignedLegion` is used. But this field probably 'll be used later.
        public HashSet<Legionary> Members = new HashSet<Legionary>();
        public Legion ParentLegion; //groups are arranged in hierarchies

        public Knowledge Knowledge => Legionary.Knowledge;
        public LegionDef Def { get; private set; }

        // Legion is Legionary itself. Is used for: a)  knowledge sharing (already) + shared overall AI (in future)
        public MobLegionary Legionary { get; private set; } // self-legionary

        public Legion(GameObject host, LegionDef def, IEntitiesRepository repo)
        {
            Legionary = new MobLegionary(null, null, repo);
            Def = def;
        }
        public void Assign(Legionary legionary)
        {
            if (Members.Add(legionary))
                legionary.AssignToLegion(this);
        }
        public void Unassign(Legionary legionary)
        {
            if (Members.Remove(legionary))
                legionary.UnassignFromLegion();
        }

    }

}
