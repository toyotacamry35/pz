using GeneratorAnnotations;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using ProtoBuf;
using ResourceSystem.Aspects.Misc;
using SharedCode.Entities;

namespace SharedCode.Entities
{
    public interface IHasAccountStats
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IAccountStats AccountStats { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface IAccountStats : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int AccountExperience { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        GenderDef Gender { get; set; }

        ValueTask<bool> SetAccountStats(AccountStatsData value);
    }

    [ProtoContract]
    public struct AccountStatsData
    {
        [ProtoMember(1)] public int Experience { get; set; }
        [ProtoMember(2)] public GenderDef Gender { get; set; }

        public override string ToString()
        {
            return $"Exp:{Experience} Gender:{Gender.____GetDebugAddress()}";
        }
    }
}
namespace GeneratedCode.DeltaObjects
{
    public partial class AccountStats : IAccountStatsImplementRemoteMethods
    {
        public ValueTask<bool> SetAccountStatsImpl(AccountStatsData value)
        {
            AccountExperience = value.Experience;
            Gender = value.Gender;
            return new ValueTask<bool>(true);
        }
    }
}