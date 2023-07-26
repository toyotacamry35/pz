using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Entities;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.EntitySystem;

namespace SharedCode.Entities
{
    public interface IHasMutationMechanics : IHasFaction
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IMutationMechanics MutationMechanics { get; set; }
    }


    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IMutationMechanics : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        MutatingFactionsDef FactionsDef { get; set; } // TODOA

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        MutatingFactionDef Faction { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        MutatingFactionDef NewFaction { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        MutationStageDef Stage { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        MutationStageDef NewStage { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        float Mutation { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        long AllowedTimeMutationChange { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task<bool> CanChangeMutation(float value, MutatingFactionDef toFaction);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<bool> ChangeMutation(float value, MutatingFactionDef toFaction, float coolDownTime, bool forceChange);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> ApplyMutationChangeForced(MutationStageDef newStage, MutatingFactionDef newFaction);
    }
}