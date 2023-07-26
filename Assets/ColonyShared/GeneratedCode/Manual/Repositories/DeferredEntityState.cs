using SharedCode.EntitySystem;

namespace GeneratedCode.Manual.Repositories
{
    public readonly struct DeferredEntityState
    {
        public DeferredEntityState(ReplicationLevel deferredLevel, ReplicationLevel existedLevel)
        {
            DeferredLevel = deferredLevel;
            ExistedLevel = existedLevel;
        }

        public ReplicationLevel DeferredLevel { get; }
        public ReplicationLevel ExistedLevel { get; }
    }
}