using GeneratorAnnotations;
using ResourceSystem.Aspects.Rewards;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System.Threading.Tasks;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    public interface ISessionResult : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaList<RewardDef> Achievements { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ValueTask<bool> Clear();
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class SessionResult
    {
        public ValueTask<bool> ClearImpl()
        {
            Achievements.Clear();

            return new ValueTask<bool>(true);
        }
    }
}