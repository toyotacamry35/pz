using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using SharedCode.Entities.Engine;
using SharedCode.Entities.Mineable;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Utils;
using System;
using System.Threading.Tasks;

namespace SharedCode.Entities
{
    public interface IHasQuestEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IQuestEngine Quest { get; set; }
    }
}

