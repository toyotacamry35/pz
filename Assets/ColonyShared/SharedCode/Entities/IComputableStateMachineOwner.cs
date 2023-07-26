using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasComputableStateMachine
    {
        IComputableStateMachine ComputableStateMachine { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IComputableStateMachine : IDeltaObject
    {
        // Is `true` by default. Set to `false` when object-inner-state changed a way that makes farther statemachine-state changing impossible
        // Don't use setter! (It should be set only twice: to dflt `true` val. in OnInit & to `false` in `MarkNotPristine`. & it already implemented).
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        bool IsPristineInternal { get; set; }
        // Is null until `IsPristine` set to `false`. From that moment last state is fixed here (& 'll not be changed any more). So `GetCurrentStates` 'll return it (instead of calculated state).
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ComputableStatesDef FixedStates { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]  ValueTask<ComputableStateMachineDef> GetStateMachineDef();
        [ReplicationLevel(ReplicationLevel.Master)]  Task MarkNotPristine();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<bool> GetIsPristine();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  Task<ComputableStatesDef> GetCurrentStates();
    }
}
