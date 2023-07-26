using Core.Environment.Logging.Extension;

namespace SharedCode.EntitySystem
{
    public static class DeltaObjectExtensions
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static T To<T>(this IDeltaObject t)
        {
            if (t is IBaseDeltaObjectWrapper wrapper)
                return wrapper.GetBaseDeltaObject().To<T>();

            var replicationLevel = SharedCode.Repositories.ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(T));
            if (!((IDeltaObjectExt)t).ContainsReplicationLevel((long)replicationLevel))
            {
                Logger.IfError()?.Message($"DeltaObject {t} not contains replication level {replicationLevel} | Parent:{t.ParentEntityId}({(t.ParentTypeId != 0 ? Repositories.ReplicaTypeRegistry.GetTypeById(t.ParentTypeId)?.Name : "none")}) Repo:{t.EntitiesRepository?.Id}({t.EntitiesRepository?.CloudNodeType})").Write();
                return default;
            }
            return (T)t.GetReplicationLevel(replicationLevel);
        }
        
        public static T TryTo<T>(this IDeltaObject t)
        {
            if (t is IBaseDeltaObjectWrapper wrapper)
                return wrapper.GetBaseDeltaObject().TryTo<T>();

            var replicationLevel = SharedCode.Repositories.ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(T));
            if (!((IDeltaObjectExt)t).ContainsReplicationLevel((long)replicationLevel))
                return default;
            return (T)t.GetReplicationLevel(replicationLevel);
        }

    }
}
