using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using SharedCode.EntitySystem;

namespace ColonyShared.SharedCode.Entities
{
    // можно было, конечно, просто завести процедурный стат, но с сундуками возникает проблема, заключающаяся в том,
    // что либо калкер этого стата должен лезть в entity владельца, что неприемлимо,
    // либо, необходимая для калкера  информация о владельце, должна сохранятся в сундуке, и нужны калкеры умеющие эту информацию из сундука добывать, 
    // либо, необходима возможность добавлять прцедурный стат в stats engine на лету (или менять калкер существующего стата на лету), чего сейчас нет 
    public interface IHasIncomingDamageMultiplier 
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<CalcerDef<float>> GetIncomingDamageMultiplier();
    }

    public interface IHasPersistentIncomingDamageMultiplier : IHasIncomingDamageMultiplier
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        DamageMultiplierDef IncomingDamageMultiplier { get; set; }
    }
}