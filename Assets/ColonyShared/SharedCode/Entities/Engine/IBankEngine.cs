using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using Src.Aspects.Impl.Stats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ResourceSystem.Aspects.Banks;

namespace SharedCode.Entities.Engine
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IBankEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Server)] BankDef BankDef { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<OuterRef<IEntity>> OpenBankCell();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<OuterRef<IEntity>> CloseBankCell();
    }
}