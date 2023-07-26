using System;
using System.Collections.Generic;
using System.Reflection;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using MongoDB.Bson;
using System.Linq;
using ResourcesSystem.Loader;
using Core.Reflection;
using NLog;
using GeneratedCode.DatabaseUtils;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Core.Environment.Logging.Extension;

namespace SharedCode.Utils.BsonSerialization
{
    using MigratorKey = ValueTuple<Type, string, string>;

    public class DiffItem
    {
        public DiffItem(string fromVersion, string toVersion, DiffDescriptor diff)
        {
            FromVersion = fromVersion;
            ToVersion = toVersion;
            Diff = diff;
        }
        public string FromVersion { get; private set; }
        public string ToVersion { get; private set; }
        public DiffDescriptor Diff { get; private set; }
    }

    public interface IMigrator
    {
        string FromVersion { get; }
        string ToVersion { get; }
        Type EntityType { get; }
        BsonDocument Convert(BsonDocument bsonDocument);
    }

    public abstract class BaseMigrator<T>: IMigrator where T : IEntity
    {
        public Type EntityType { get { return typeof(T); } }

        public abstract string FromVersion { get; } 
        public abstract string ToVersion { get; }

        public abstract BsonDocument Convert(BsonDocument bsonDocument);
    }

    public static class MigrationHelper
    {
        private static string SavePath = "/EntityVersions/";

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static IMigrator GetMigrator(Type type, string fromVersion, string toVersion)
        {
            MigratorKey key = new MigratorKey(type, fromVersion, toVersion);
            IMigrator value;
            if (!Migrators.TryGetValue(key, out value))
                return null;
            return value;
        }

        public static IList<IMigrator> GetMigrators(Type type, string fromVersion, string toVersion)
        {
            List<MigratorKey> chain;
            if (!MigratorChains.TryGetValue(type, out chain))
                return null;

            string currentVersion = toVersion;

            List<IMigrator> result = new List<IMigrator>();

            foreach (var migratorKey in chain)
            {
                if (migratorKey.Item3 != currentVersion)
                    return null;

                result.Add(Migrators[migratorKey]);
                currentVersion = migratorKey.Item2;
                if (migratorKey.Item2 == fromVersion)
                {
                    return result;
                }
            }

            return null;
        }

        public static void SaveCurrentEntityVersionsSnapshot()
        {
            var entityVersionsSnapshot = new EntityVersionsSnapshot();
            foreach(var pair in EntityHashes)
            {
                entityVersionsSnapshot.Set(pair.Key, pair.Value, TypeHashCalculator.GetTypeDescriptor(pair.Key));
            }

            string resultPath;
            GameResources.SimpleSave(GameResourcesHolder.Instance.Deserializer.Loader.GetRoot() + SavePath, ThisAssembly.GitCommitId, entityVersionsSnapshot, out resultPath);
        }

        private static readonly object _lock = new object();

        public static void InitializeMigrations()
        {
            lock (_lock)
            {
                var versionsResources = GameResourcesHolder.Instance.LoadResource<EntityVersionsSnapshotsCollection>(SavePath + "versions");
                Versions.Clear();
                foreach (var snapshot in versionsResources.Snapshots)
                {
                    Versions.Add(snapshot.Target);
                }

                Dictionary<Type, string> currentEntityHashes = new Dictionary<Type, string>(EntityHashes);

                for (int i = Versions.Count - 1; i >= 0; --i)
                {
                    foreach (var key in currentEntityHashes.Keys.ToList())
                    {
                        var result = Versions[i].Get(key);
                        if (result != null)
                        {
                            var value = currentEntityHashes[key];
                            if (!Enumerable.SequenceEqual(result.Hash, value))
                            {
                                var migrationKey = new MigratorKey(key, result.Hash, value);
                                Migrators[migrationKey] = null;
                                MigratorChains[key].Add(migrationKey);
                                currentEntityHashes[key] = result.Hash;
                            }
                        }
                    }
                }

                foreach (var type in typeof(MigrationHelper).Assembly.GetTypesSafe())
                {
                    if (!type.IsAbstract && !type.IsInterface && typeof(IMigrator).IsAssignableFrom(type))
                    {
                        IMigrator migrator = (IMigrator)Activator.CreateInstance(type);

                        IMigrator temp;
                        MigratorKey key = new MigratorKey(migrator.EntityType, migrator.FromVersion, migrator.ToVersion);
                        if (Migrators.TryGetValue(key, out temp))
                        {
                            if (temp != null)
                                throw new Exception($"Migrator for type:{migrator.EntityType.Name} fromVersion:{migrator.FromVersion} toVersion:{migrator.ToVersion}");
                            Migrators[key] = migrator;
                        }
                        else
                        {
                            Logger.IfWarn()?.Message("Migrator {0} seems to be useless", type.Name).Write();
                        }
                    }
                }
            }
        }

        public static async Task CheckMigrations()
        {
            Dictionary<Type, HashItem> currentEntityHashes = new Dictionary<Type, HashItem>();

            var results = new Dictionary<Type, List<DiffItem>>();

            foreach(var pair in EntityHashes)
            {
                currentEntityHashes[pair.Key] = new HashItem(pair.Value, TypeHashCalculator.GetTypeDescriptor(pair.Key));
            }

            for (int i = Versions.Count - 1; i >= 0; --i)
            {
                foreach (var key in currentEntityHashes.Keys.ToList())
                {
                    List<DiffItem> resultsForType;
                    if(!results.TryGetValue(key, out resultsForType))
                    {
                        resultsForType = new List<DiffItem>();
                        results[key] = resultsForType;
                    }

                    var result = Versions[i].Get(key);
                    if (result != null)
                    {
                        var value = currentEntityHashes[key];
                        if (!Enumerable.SequenceEqual(result.Hash, value.Hash))
                        {
                            var resultItem = new DiffItem(result.Hash, value.Hash, value.TypeDescriptor.DiffFromVersion(result.TypeDescriptor));
                            resultsForType.Add(resultItem);
                            currentEntityHashes[key] = result;
                        }
                    }
                }
            }

            string basePath = await PathUtils.GetEntityDiffsDirectory();

            foreach(var pair in results)
            {
                if(pair.Value.Count > 0)
                {
                    string diffString = JsonConvert.SerializeObject(pair.Value,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                    File.WriteAllText(System.IO.Path.Combine(basePath, pair.Key.Name + ".json"), diffString);
                }
            }

            bool succes = true;

            foreach (var pair in Migrators)
            {
                if (pair.Value == null)
                {
                    succes = false;
                    Logger.IfError()?.Message($"No migrator for type:{pair.Key.Item1} fromVersion:{pair.Key.Item2} toVersion:{pair.Key.Item3}").Write();
                }
            }
            if(!succes)
                throw new Exception("Missed some migrators");
        }

        private static Dictionary<Type, string> EntityHashes = new Dictionary<Type, string>();
        private static List<EntityVersionsSnapshot> Versions = new List<EntityVersionsSnapshot>();
        private static Dictionary<MigratorKey, IMigrator> Migrators = new Dictionary<MigratorKey, IMigrator>();
        private static Dictionary<Type, List<MigratorKey>> MigratorChains = new Dictionary<Type, List<MigratorKey>>();

        static MigrationHelper()
        {
            foreach (var assembly in GetCustomAssemblies())
            {
                foreach (var type in assembly.GetTypesSafe())
                {
                    if (!type.IsAbstract && !type.IsInterface && typeof(IEntity).IsAssignableFrom(type) && !type.IsGenericType && !typeof(BaseEntityWrapper).IsAssignableFrom(type))
                    {
                        Type interfaceType = type;
                        string interfaceName = 'I' + type.Name;
                        foreach (var @interface in type.GetInterfaces())
                        {
                            if(@interface.Name == interfaceName)
                            {
                                interfaceType = @interface;
                                break;
                            }
                        }
                        var databaseSaveType = DatabaseSaveTypeChecker.GetDatabaseSaveType(interfaceType);
                        if (databaseSaveType != DatabaseSaveType.None)
                        {
                            EntityHashes[type] = TypeHashCalculator.GetHashByType(type);
                            MigratorChains[type] = new List<MigratorKey>();
                        }
                    }
                }
            }
        }

        private static IEnumerable<Assembly> GetCustomAssemblies()
        {
            var allowedAssembliesSubstrs = new List<string>() { "SharedCode", "GeneratedCode" };
            var assemblies = AppDomain.CurrentDomain.GetAssembliesSafe().Where(x => !x.IsDynamic && allowedAssembliesSubstrs.Any(y => x.FullName.Contains(y)));
            return assemblies;
        }

    }

}