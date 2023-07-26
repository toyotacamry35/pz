using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utilities
{
    public static class ServicesResolver
    {
        static Dictionary<Type, object> locators = new Dictionary<Type, object>();
        public static T Get<T>(object caller) where T : class
        {
            object obj;
            if(!locators.TryGetValue(typeof(T), out obj))
                return default(T);
            if (obj == null)
                return default(T);
            if (obj.GetType().BaseType.IsGenericType && obj.GetType().BaseType.GetGenericTypeDefinition() == typeof(ServiceLocator<>))
            {
                return (obj as ServiceLocator<T>).Get(caller);
            }
            else
                return obj as T;
        }
        public static void Set<T>(object obj)
        {
            Set(typeof(T), obj);
        }
        public static void Set(Type t, object obj)
        {
            if(locators.ContainsKey(t))
                locators[t] = obj;
            else
                locators.Add(t, obj);
        }
    }

    public abstract class ServiceLocator<T>
    {
        public abstract T Get(object caller);
    }
    
}

