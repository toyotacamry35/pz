using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Aspects.Dialog;

namespace SharedCode.Entities
{
    public interface IHasDialogEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDialogEngine Dialog { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IDialogEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull), BsonIgnore] DialogDef CurrentNode { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] Task<DialogDef> Next(DialogDef nextDialog);
        [ReplicationLevel(ReplicationLevel.Server)] Task<DialogDef> NextWithCheck(DialogDef nextDialog);
    }
}

