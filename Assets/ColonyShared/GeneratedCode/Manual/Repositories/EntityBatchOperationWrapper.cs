using GeneratedCode.Manual.Repositories;
using SharedCode.EntitySystem;

namespace GeneratedCode.Repositories
{
    public readonly struct EntityBatchOperationWrapper
    {
        public EntityBatchOperationWrapper(ReadWriteEntityOperationType operation, EntityQueue entityQueue)
        {
            Operation = operation;
            EntityQueue = entityQueue;
        }

        public ReadWriteEntityOperationType Operation { get; }
            
        public EntityQueue EntityQueue { get; }
    }
}