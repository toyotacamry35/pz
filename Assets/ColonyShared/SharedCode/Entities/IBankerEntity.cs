using Assets.ResourceSystem.Aspects.Banks;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Threading.Tasks;
using ResourceSystem.Utils;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    public interface IBankerEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)] IDeltaDictionary<BankDef, IBankHolder> BankCells { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] Task<OuterRef> GetBankCell(BankDef bankDef, OuterRef bankCell);
        [ReplicationLevel(ReplicationLevel.Server)] Task DestroyBankCells(BankDef bankDef, PropertyAddress corpseInventoryAddress);
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IBankHolder : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Server)] IDeltaDictionary<OuterRef, OuterRef> Cells { get; set; }
    }
}
