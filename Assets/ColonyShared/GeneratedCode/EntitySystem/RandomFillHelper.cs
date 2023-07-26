using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.EntitySystem
{
    internal static class RandomFillHelper
    {
        public static T CrateInstance<T>()
        {
            var isStruct = typeof(T).IsValueType;
            if (isStruct)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            else
            {
                if (typeof(T).IsAbstract)
                {
                    return default;
                }
                
                var ctor = typeof(T).GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, Array.Empty<Type>(), null);
                var res = (T)ctor.Invoke(Array.Empty<object>());
                return res;
            }
        }
    }
}
