using Assets.Src.Aspects.Impl.Traumas.Template;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using SharedCode.Wizardry;

namespace Src.Aspects.Impl.Stats
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ITraumaGiver : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ulong SpellId { get; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        int CurrentTraumaPoints { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        TraumaDef Def { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> StartTrauma(IEntity entity);
        [ReplicationLevel(ReplicationLevel.Master)]
        Task StopTrauma(IEntity entity);
    }

    public static class TraumaGiverExtensions
    {
        public static bool HasActiveSpell(this ITraumaGiver t) => t != null && new SpellId(t.SpellId).IsValid;
    }
}
