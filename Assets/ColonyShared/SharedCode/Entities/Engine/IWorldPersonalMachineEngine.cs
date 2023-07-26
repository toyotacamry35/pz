using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.Aspects.Impl.Definitions;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ColonyShared.SharedCode.Entities.Engine
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IWorldPersonalMachineEngine : IDeltaObject
    {
        /// <summary>
        /// key: WorldBench  value: ICraftEngine
        /// </summary>
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<OuterRef, OuterRef> WorldPersonalMachines { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] Task<OuterRef> GetOrAddMachine(WorldPersonalMachineDef def, OuterRef key);
        [ReplicationLevel(ReplicationLevel.Server)] Task RemoveMachine(OuterRef key);

    }
}
