using Assets.Src.ResourcesSystem.Base;
using SharedCode.EntitySystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Reflection;
using Scripting;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;

namespace SharedCode.Utils
{
    public static class DefToType
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        static ConcurrentDictionary<Type, Type> _instanceToDefType = new ConcurrentDictionary<Type, Type>();
        static ConcurrentDictionary<Type, Type> _defToSourceType = new ConcurrentDictionary<Type, Type>();
        static ConcurrentDictionary<Type, Type> _defToSourceNoInterfaceType = new ConcurrentDictionary<Type, Type>();
        static ConcurrentDictionary<Type, Type> _defToEntityType = new ConcurrentDictionary<Type, Type>();
        static ConcurrentDictionary<Type, int> _typeToNetId = new ConcurrentDictionary<Type, int>();
        static ConcurrentDictionary<int, Type> _netIdToType = new ConcurrentDictionary<int, Type>();
        static DefToType()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssembliesSafeNonStandard().SelectMany(x => x.GetTypesSafe());
            var unityTypes = AppDomain.CurrentDomain.GetAssembliesSafe().Where(x => x.FullName.StartsWith("unityengine", StringComparison.OrdinalIgnoreCase)).SelectMany(x => x.GetTypesSafe());

            var nameToType = new Dictionary<string, Type>();
            var defs = new List<Type>();
            var netIds = new List<Type>();
            foreach (var type in allTypes)
            {
                if (type.IsAbstract && type.IsSealed)
                    continue;
                if (typeof(IResource).IsAssignableFrom(type))
                {
                    if (type.CustomAttributes.Any(x => x.AttributeType == typeof(HasNetIdAttribute)))
                        netIds.Add(type);
                    defs.Add(type);
                }
                else
                {
                    if (type.FullName != null)
                    {
                        nameToType[type.Name] = type;
                        nameToType[type.FullName] = type;
                    }
                }
            }
            for(int i = 0; i < netIds.Count; i++)
            {
                var crc = (int)(Crc64.Compute(netIds[i].Name) % int.MaxValue);
                _typeToNetId[netIds[i]] = crc;
                if (!_netIdToType.TryAdd(crc, netIds[i]))
                {
                    throw new Exception($"Same net CRC {netIds[i].FullName} {_netIdToType[crc].FullName} {crc}");
                }
            }
            foreach (var type in unityTypes)
            {
                if (type.FullName != null)
                    nameToType[type.FullName] = type;
            }
            foreach (var defType in defs)
            {
                Type iType = null;
                Type sType = null;

                if (defType.GetCustomAttributesSafe<DefToTypeAttribute>(false).FirstOrDefault() is DefToTypeAttribute attr)
                {
                    if (nameToType.TryGetValue(attr.TypeName, out iType))
                        sType = iType;
                }
                else
                if (defType.Name.EndsWith("Def"))
                {
                    var typeName = defType.Name.Substring(0, defType.Name.Length - 3);
                    if (nameToType.TryGetValue("I" + typeName, out iType) && !((typeof(IDeltaObject).IsAssignableFrom(iType) && !(typeof(IEntity).IsAssignableFrom(iType)))))
                        nameToType.TryGetValue(typeName, out sType);
                    else
                    if( nameToType.TryGetValue(typeName, out iType) )
                        sType = iType;
                }

                if (iType != null)
                {
                    if (typeof(IEntity).IsAssignableFrom(iType))
                        _defToEntityType.TryAdd(defType, iType);
                    else
                    {
                        _instanceToDefType[iType] = defType;
                        _defToSourceType.TryAdd(defType, iType);
                    }
                }
                
                if (sType != null)
                {
                    _defToSourceNoInterfaceType[defType] = sType;
                }
            }
        }

        public static IEnumerable<Type> GetAllDefs<T>() where T : IResource
        {
            foreach (var defToSource in _defToSourceType.Where(x => typeof(T).IsAssignableFrom(x.Key)))
                yield return defToSource.Key;
        }
        public static IEnumerable<KeyValuePair<Type, Type>> GetAllDefsToInstanceNoInterface<T>() where T : IResource
        {
            // Get an selection of all heirs of `T`:
            foreach (var defToSource in _defToSourceNoInterfaceType.Where(x => typeof(T).IsAssignableFrom(x.Key)))
                yield return defToSource;
        }
        public static IEnumerable<KeyValuePair<Type, Type>> GetAllDefsToInstance<T>() where T : IResource
        {
            // Get an selection of all heirs of `T`:
            foreach (var defToSource in _defToSourceType.Where(x => typeof(T).IsAssignableFrom(x.Key)))
                yield return defToSource;
        }
        public static Type GetDefTypeFromInstance(Type instanceType)
        {
            if (_instanceToDefType.ContainsKey(instanceType))
                return _instanceToDefType[instanceType];
            return null;
        }
        public static Type GetType(Type defType)
        {
            Type resultType;
            _defToSourceType.TryGetValue(defType, out resultType);
            return resultType;
        }
        public static Type GetEntityType(Type defType)
        {
            Type ret = null;
            _defToEntityType.TryGetValue(defType, out ret);
            if (ret == null)
                Logger.IfError()?.Message($"{nameof(GetEntityType)}: Not found entityType by {nameof(defType)}={defType}").Write();
            return ret;
        }

        public static int GetNetIdForType(Type type)
        {
            return _typeToNetId[type];
        }
        public static Type GetTypeFromNetId(int netId)
        {
            return _netIdToType[netId];
        }

    }

}
