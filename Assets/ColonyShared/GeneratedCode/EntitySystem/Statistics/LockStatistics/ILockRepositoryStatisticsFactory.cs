using System;

namespace GeneratedCode.EntitySystem.Statistics
{
    public interface ILockRepositoryStatisticsFactory
    {
        LockRepositoryStatisticsPrometheus GetOrAdd(Guid repositoryId);
    }
}