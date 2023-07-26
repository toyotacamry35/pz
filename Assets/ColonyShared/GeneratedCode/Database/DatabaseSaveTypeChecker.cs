using SharedCode.EntitySystem;
using SharedCode.Repositories;
using System;

namespace GeneratedCode.DatabaseUtils
{
    public static class DatabaseSaveTypeChecker
    {
        public static DatabaseSaveType GetDatabaseSaveType(int typeId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            return ReplicaTypeRegistry.GetDatabaseSaveType(type);
        }
        public static DatabaseSaveType GetDatabaseSaveType(Type type) => ReplicaTypeRegistry.GetDatabaseSaveType(type);
        public static DatabaseServiceType GetDatabaseServiceType(int typeId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            return ReplicaTypeRegistry.GetDatabaseServiceType(type);
        }
        public static DatabaseServiceType GetDatabaseServiceType(Type type) => ReplicaTypeRegistry.GetDatabaseServiceType(type);
    }
}
