using Assets.Src.ResourcesSystem.Base;
using Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode;
using ResourceSystem.Utils;
using UnityEngine;
using NLog;

namespace ResourcesSystem.Loader
{
    public static class KnownTypesCollector
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const string GenericMarker = @"<>";
        private const string DefPrefix = @"Def";

        private static readonly Dictionary<string, Type> GenericArgTypes = Value.SupportedTypes.Where(x => x.Item1 != Value.Type.None).ToDictionary(x => x.Item1.ToString().ToLower(), x => x.Item2);
        
        public static IReadOnlyDictionary<string, Type> KnownTypes { get; }

        private static readonly Type ResourceType = typeof(IResource);

        private static readonly Type[] BinarySerializable = (AppDomain.CurrentDomain.GetAssembliesSafeNonStandard()
                .Where(v => !v.IsDynamic)
                .SelectMany(v => v.GetTypesSafe())
                .Where(type => typeof(IBinarySerializable).IsAssignableFrom(type))).ToArray();

        private static readonly Type[] OurUnityTypesCollection =
        {
            typeof(Sprite),
            typeof(AudioClip),
            typeof(GameObject),
            //typeof(LocalizedString)
        };

        static KnownTypesCollector()
        {
            var defTypesCollection = AppDomain.CurrentDomain.GetAssembliesSafeNonStandard()
                .Where(v => !v.IsDynamic)
                .SelectMany(v => v.GetTypesSafe())
                .Where(type => typeof(BaseResource).IsAssignableFrom(type) || type.CustomAttributes.Any(v => v.AttributeType == typeof(KnownToGameResourcesAttribute)));

            var typesArray = defTypesCollection.Concat(OurUnityTypesCollection);
            var typesPairs = typesArray.SelectMany(GetDefName);

            (string, Type)[] additionalTypesPairs =
            {
                ( "float", typeof(float)),
                ( "int", typeof(int)),
                ( "bool", typeof(bool)),
                ( "string", typeof(string)),
                ( "OuterRef", typeof(OuterRef))
            };

            var typesPairsTotal = typesPairs.Concat(additionalTypesPairs);

            var typesGroups = typesPairsTotal.GroupBy(v => v.Item1);

            var duplicateTypes = typesGroups.Where(v => v.Skip(1).Any());
            var uniqueTypes = typesGroups.Where(v => !v.Skip(1).Any());

            KnownTypes = uniqueTypes.ToDictionary(v => v.Key, v => v.Single().Item2);

            foreach (var dupe in duplicateTypes)
            {
                var types = string.Join(", ", dupe.Select(v => v.Item2));
                GameResources.ThrowError($"Type mapping collision: Alias {dupe.Key} maps to types {types}");
            }
        }

        private static IEnumerable<(string name, Type type)> GetDefName(Type t)
        {
            if (t.IsGenericTypeDefinition && !t.IsAbstract)
            {
                IEnumerable<Type> argsToSubstitute;
                if (t.GetGenericArguments()[0].GetGenericParameterConstraints().FirstOrDefault() == typeof(IBinarySerializable))
                    argsToSubstitute = BinarySerializable;
                else if (t.GetGenericArguments()[0].GetGenericParameterConstraints().FirstOrDefault() == typeof(UnityEngine.Object))
                    argsToSubstitute = OurUnityTypesCollection;
                else
                    argsToSubstitute = Value.SupportedTypes.Select(v => v.Item2).Where(v => v != typeof(void)).ToArray();

                foreach (var valType in argsToSubstitute)
                {
                    Type closedGeneric;
                    try
                    {
                        closedGeneric = t.MakeGenericType(valType);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        yield break;
                    }
                    foreach (var substitution in GetDefName(closedGeneric))
                        yield return substitution;
                }
                yield break;
            }

            if (t.IsNested && ResourceType.IsAssignableFrom(t)) // для не ресурсов (например типов помеченных KnownToGameResourcesAttribute) нестинг не работает, так как они читаются не через наш DefConverter :(  
            {
                var (name, nameWithoutDef) = GetCleanName(t);
                var (contName, contNameWithoutDef) = GetCleanName(t.DeclaringType);
                yield return ($"{contName}.{name}", t);
                if (nameWithoutDef != name)
                    yield return ($"{contName}.{nameWithoutDef}", t);

                if (contName != contNameWithoutDef)
                {
                    yield return ($"{contNameWithoutDef}.{name}", t);
                    if (nameWithoutDef != name)
                        yield return ($"{contNameWithoutDef}.{nameWithoutDef}", t);
                }
            }
            else
            {
                var (name, nameWithoutDef) = GetCleanName(t);
                yield return (name, t);
                if (nameWithoutDef != name)
                    yield return (nameWithoutDef, t);
            }
        }

        private static (string, string) GetCleanName(Type type)
        {
            var cleanNameFull = type.NiceName();
            cleanNameFull = cleanNameFull.Replace("BaseResource", "Resource");
            var cleanNameWithoutDef = RemoveDef(cleanNameFull);
            return (cleanNameFull, cleanNameWithoutDef);
        }

        private static string RemoveDef(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            if (name.Contains("<"))
            {
                var defIndex = name.LastIndexOf("Def<");
                if (defIndex != -1)
                    return name.Remove(defIndex, "Def".Length);
                else
                    return name;
            }
            else
                return name.EndsWith(DefPrefix) ? name.Substring(0, name.Length - DefPrefix.Length) : name;
        }
    }
}
