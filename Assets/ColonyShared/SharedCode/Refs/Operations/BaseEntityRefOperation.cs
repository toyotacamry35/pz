namespace SharedCode.Refs.Operations
{
    public abstract class BaseEntityRefOperation
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("EntityRefOperation");
        
        public abstract bool Do(IEntityRef entityRef, out EntityRefOperationResult? result);
    }
}
