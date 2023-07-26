using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace ColonyShared.SharedCode.Aspects.Combat
{
    public interface IHasAttackEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IAttackEngine AttackEngine { get; set; }       
    }
}