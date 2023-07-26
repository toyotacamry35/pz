using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ColonyShared.GeneratedCode;
using GeneratedCode.DeltaObjects;
using NLog;
using NLog.Fluent;

namespace ColonyShared.SharedCode.Utils
{
    public class CalcerBindingCollector : ITypesCollectorAgent
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Dictionary<Type, CalcerBinding> Collection { get; private set; }

        public void Init()
        {
            Collection = new Dictionary<Type, CalcerBinding>();
        }
        
        public void CollectType(Type type)
        {
            var collectionType = typeof(ICalcerBindingsCollector);
            
            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
                return;

            if (collectionType.IsAssignableFrom(type))
            {
                var collector = (ICalcerBindingsCollector)Activator.CreateInstance(type);
                foreach (var t in collector.Collect())
                    TryAddBinding(t, Collection);
            }
            else
                TryAddBinding(type, Collection);
        }

        public void Dump(StringBuilder sb)
        {
            foreach (var pair in Collection)
                sb.Append(pair.Key.NiceName()).Append(" => ").Append(pair.Value.GetType().NiceName()).Append("\n");
        }

        private static void TryAddBinding(Type implType, Dictionary<Type, CalcerBinding> collection)
        {
            foreach (var interfaceType in implType.GetTypeInfo().ImplementedInterfaces)
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICalcerBinding<,>)) // ICalcerBinding<,>
                {
                    var calcType = interfaceType.GetGenericArguments()[0];
                    var retType = interfaceType.GetGenericArguments()[1];
                    if (!collection.ContainsKey(calcType))
                    {
                        var bindingInstn = (CalcerBinding) Activator.CreateInstance(typeof(CalcerBinding<,>).MakeGenericType(calcType, retType));
                        bindingInstn.CalcerImplInstance = Activator.CreateInstance(implType);
                        collection.Add(calcType, bindingInstn);
                    }
                    else
                        throw new Exception($"[{nameof(CalcerBindingCollector)}] Def already registered | Def:{calcType.NiceName()} OldBinding:{collection[calcType].GetType().NiceName()} NewBinding:{implType.NiceName()}");
                }
            }
        }
    }
}