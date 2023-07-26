using System.Threading.Tasks;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace SharedCode.Entities
{
    /// <summary>
    /// Субъект, за убийство/разрушение которого должна выдаваться награда убийце/разрушителю
    /// </summary>
    public interface ICanGiveRewardForKillingMe
    {
        [ReplicationLevel(ReplicationLevel.Server)] IKillingRewardMechanics KillingRewardMechanics { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IKillingRewardMechanics : IDeltaObject
    {
        // кто нанёс решающий удар
        [ReplicationLevel(ReplicationLevel.Master)] OuterRef KillingDamageDealer { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<OuterRef> ReplaceKillingDamageDealer(OuterRef entity);
    }
}