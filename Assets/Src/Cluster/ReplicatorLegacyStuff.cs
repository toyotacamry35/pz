using JetBrains.Annotations;
using NLog;
using System;
using System.Collections.Concurrent;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.Cluster
{
    public class ReplicatorLegacyStuff
    {
        public static ReplicatorLegacyStuff Instance = new ReplicatorLegacyStuff();

        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        ///#TC-2630: Is it still needed?
        // Is needed for Host mode - to be shared by S. & Cl. to avoid double G.o. creating.
        public readonly ConcurrentDictionary<Type, ConcurrentDictionary</*Guid*/string, GameObject>> GameObjectsForEntities = new ConcurrentDictionary<Type, ConcurrentDictionary</*Guid*/string, GameObject>>();

        internal void TryAddToGameObjectsForEntities<T>(string guid, GameObject go)
        {
            AddToGameObjectsForEntities<T>(guid, go, false);
        }

        //@param `errorIfKeyPresent` - means case "dic. contains key already is unexpected & should cause error"
        internal void AddToGameObjectsForEntities<T>(string guid, GameObject go, bool errorIfKeyPresent = true)
        {
            ConcurrentDictionary<string, GameObject> dic;
            if (!GameObjectsForEntities.TryGetValue(typeof(T), out dic))
            {
                Logger.IfWarn()?.Message($"{nameof(GameObjectsForEntities)} doesn't contains dic. by type {typeof(T)}. Will be added").Write();
                dic = new ConcurrentDictionary<string, GameObject>();
                GameObjectsForEntities.TryAdd(typeof(T), dic);
            }

            if (!dic.ContainsKey(guid))
                dic.TryAdd(guid, go);
            else if (errorIfKeyPresent)
                Logger.IfError()?.Message($"{nameof(GameObjectsForEntities)} already contains entry by Type {typeof(T)} & guid {guid}").Write();
        }
    }
}
