using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Core.Reflection;

namespace Utilities
{
    public static class TypesUtilities
    {

        public static List<Type> GetTypesCastableTo<T>()
        {
            List<Type> types = new List<Type>();
            IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssembliesSafe().Where((Assembly assembly) => assembly.FullName.Contains("Assembly"));
            foreach (Assembly assembly in scriptAssemblies)
            {
                foreach (Type type in assembly.GetTypesSafe().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T))))
                {
                    types.Add(type);
                }
            }
            return types;
        }
        public static List<Type> GetTypesWithAttribute(Type attribute)
        {
            List<Type> types = new List<Type>();
            IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssembliesSafe().Where((Assembly assembly) => assembly.FullName.Contains("Assembly"));
            foreach (Assembly assembly in scriptAssemblies)
            {
                foreach (Type type in assembly.GetTypesSafe().Where(type=>type.GetCustomAttributes(attribute, false).Length > 0))
                {
                    types.Add(type);
                }
            }
            return types;
        }
        public static List<Type> GetChildrenOfGenericType(Type genericType)
        {
            List<Type> types = new List<Type>();
            IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssembliesSafe().Where((Assembly assembly) => assembly.FullName.Contains("Assembly"));
            foreach (Assembly assembly in scriptAssemblies)
            {
                foreach (Type type in assembly.GetTypesSafe().Where(type => type.IsClass && !type.IsAbstract && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == genericType))
                {
                    types.Add(type);
                }
            }

            return types;
        }

        static Dictionary<string, Type> types;
        public static Type FindType(string fullname)
        {
            if(types == null)
            {
                types = new Dictionary<string, Type>();
                var scriptAssemblies = AppDomain.CurrentDomain.GetAssembliesSafe();
                foreach (Assembly assembly in scriptAssemblies)
                {
                    foreach (Type type in assembly.GetTypesSafe())
                    {
                        if (types.ContainsKey(type.FullName))
                        {
                            //Debug.Log("Duplicate: " + type.FullName);

                        } 
                        else
                        {
                            types.Add(type.FullName, type);
                            //Debug.Log(type.FullName);
                        }
                    }
                }
            }
            Type resultType = null;
            if(!types.TryGetValue(fullname, out resultType))
            {
                Debug.LogWarning("Can't find " + fullname);
            }
            return resultType;
        }
    }    
}

