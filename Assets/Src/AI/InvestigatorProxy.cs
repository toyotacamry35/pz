using Assets.Src.Aspects;
using Assets.Src.RubiconAI;
using Assets.Src.SpawnSystem;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.AI
{
    public class InvestigatorProxy : EntityGameObjectComponent
    {
        public AIWorldLegionaryHost Host
        {
            get
            {
                return AIWorld.GetWorld(Ego.WorldSpaceId)?.GetHost(new OuterRef<IEntity>(EntityId, TypeId)) ??
                    AIWorld.GetWorld(Ego.ClientRepo.Id)?.GetHost(new OuterRef<IEntity>(EntityId, TypeId));
            }
        }
    }
}
