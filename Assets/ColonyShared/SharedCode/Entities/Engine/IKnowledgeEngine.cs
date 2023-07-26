using SharedCode.Aspects.Science;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Threading.Tasks;
using GeneratorAnnotations;

namespace SharedCode.Entities.Engine
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Implicit)]
    public interface IKnowledgeEngine : IEntity, IHasOwner
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaList<TechnologyDef> KnownTechnologies { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaList<KnowledgeDef> KnownKnowledges { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaList<KnowledgeRecordDef> ShownKnowledgeRecords { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<TechnologyOperationResult> AddKnowledge(KnowledgeDef knowledgeDef);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task AddTechnology(TechnologyDef technologyDef);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<TechnologyOperationResult> Explore(KnowledgeDef knowledgeDef, TechPointCountDef[] rewardPoints);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<TechnologyOperationResult> TryToActivate(TechnologyDef technologyDef, bool doActivate);

        /// <summary>
        /// Отрицательный Count в techPointCounts - отнимает, положительный - добавляет
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<TechnologyOperationResult> ChangeRPoints(TechPointCountDef[] techPointCounts, bool isIncrement);

        /// <summary>
        /// Отрицательный Count в techPointCounts - отнимает, положительный - добавляет
        /// </summary>
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<bool> CanChangeRPoints(TechPointCountDef[] techPointCounts, bool isIncrement);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<TechnologyOperationResult> AddShownKnowledgeRecord(KnowledgeRecordDef knowledgeRecordDef);
    }

    [Flags]
    public enum TechnologyOperationResult
    {
        None = 0x00,
        Success = 1 << 1,
        ErrorNotMatchRequirements = 1 << 2,
        ErrorNullValue = 1 << 3,
        //ErrorHasNoNextLevel = 1 << 4,
        ErrorNotEnoughtResources = 1 << 5,
        ErrorAlreadyKnown = 1 << 6,
        ErrorUnknown = 1 << 7,

        Error = ErrorNotMatchRequirements | ErrorNullValue | ErrorNotEnoughtResources | ErrorAlreadyKnown | ErrorUnknown
    }

    public static class TechnologyOperationResultExtensions
    {
        public static bool Is(this TechnologyOperationResult check, TechnologyOperationResult check2)
        {
            return (check & check2) != 0;
        }
    }
}