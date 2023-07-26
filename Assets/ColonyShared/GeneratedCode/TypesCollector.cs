using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Environment.Logging.Extension;
using Core.Reflection;

namespace ColonyShared.GeneratedCode
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false)]
    public class CollectTypesAttribute : Attribute
    {
        public Type AgentType { get; }

        public CollectTypesAttribute(Type agentType = null)
        {
            AgentType = agentType;
        }
    }
    
    
    public static class TypesCollector
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly Type CollectTypesAttributeType = typeof(CollectTypesAttribute);
        
        public static void CollectTypes()
        {
            var allTypes = AllTypes.ToArray();
            var typeCollectorAgents = allTypes
                .SelectMany(x => x.GetMembers(BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic))    
                .Where(x => x.IsDefined(CollectTypesAttributeType, false))
                .Select<MemberInfo,(ITypesCollectorAgent Agent, Action AgentSetter)>(x =>
                {
                    switch (x)
                    {
                        case PropertyInfo prop:
                        {
                            if (!prop.CanWrite) throw new Exception($"Property {prop.DeclaringType.NiceName()}.{x.Name} must be writeable");
                            if (!typeof(ITypesCollectorAgent).IsAssignableFrom(prop.PropertyType)) throw new Exception($"Type of {x.DeclaringType.NiceName()}.{x.Name} must implements {nameof(ITypesCollectorAgent)}");
                            var agentType = ((CollectTypesAttribute)prop.GetCustomAttributes(CollectTypesAttributeType).First()).AgentType ?? prop.PropertyType; 
                            var agent = (ITypesCollectorAgent) Activator.CreateInstance(agentType) ?? throw new Exception($"Can't create types collector agent {x.DeclaringType.NiceName()}");
                            return (agent, () => prop.SetValue(null, agent));
                        }
                        case FieldInfo fld:
                        {
                            if (!typeof(ITypesCollectorAgent).IsAssignableFrom(fld.FieldType)) throw new Exception($"Type of {x.DeclaringType.NiceName()}.{x.Name} must implements {nameof(ITypesCollectorAgent)}");
                            var agentType = ((CollectTypesAttribute)fld.GetCustomAttributes(CollectTypesAttributeType).First()).AgentType ?? fld.FieldType; 
                            var agent = (ITypesCollectorAgent) Activator.CreateInstance(agentType) ?? throw new Exception($"Can't create types collector agent {x.DeclaringType.NiceName()}");
                            return (agent, () => fld.SetValue(null, agent));
                        }
                        default:
                            throw new Exception($"{x.DeclaringType.NiceName()}.{x.Name} is not a property or field");
                    }
                }).ToArray();
            CollectTypes(typeCollectorAgents.Select(x => x.Agent), allTypes);
            foreach (var setter in typeCollectorAgents.Select(x => x.AgentSetter))
                setter.Invoke();
        }

        public static void CollectTypes(params ITypesCollectorAgent[] typeCollectorAgents)
        {
            CollectTypes(typeCollectorAgents, AllTypes);
        }     

        public static void CollectTypes(IEnumerable<ITypesCollectorAgent> typeCollectorAgents)
        {
            CollectTypes(typeCollectorAgents, AllTypes);
        }     
        
        private static void CollectTypes(IEnumerable<ITypesCollectorAgent> typeCollectorAgents, IEnumerable<Type> allTypes)
        {
            List<Exception> exceptions = new List<Exception>();
            var agents = typeCollectorAgents.ToArray();
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Agents:[{string.Join(", ", agents.Select(x => x.GetType()))}]").Write();
            var sw = new Stopwatch();
            sw.Start();
            
            foreach (var agent in agents)
                agent.Init();
            
            foreach (var type in allTypes)
            {
                foreach (var agent in agents)
                {
                    try
                    {
                        agent.CollectType(type);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
            }
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            sw.Stop();
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Collecting types takes {sw.Elapsed.TotalSeconds} sec").Write();

            if (Logger.IsTraceEnabled)
            {
                var sb = new StringBuilder();
                foreach (var agent in agents)
                {
                    try
                    {
                        sb.Clear();
                        agent.Dump(sb);
                        Logger.IfTrace()?.Message($"{agent}:\n{sb}\n").Write();
                    }
                    catch (Exception e)
                    {
                        if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message(e, "").Write();
                    }
                }
            }
        }
        
        private static IEnumerable<Type> AllTypes => AppDomain.CurrentDomain.GetAssembliesSafeNonStandard().SelectMany(x => x.GetTypesSafe());
    }

    public interface ITypesCollectorAgent
    {
        void Init();
        void CollectType(Type type);
        void Dump(StringBuilder sb);
    }
    
    public abstract class TypesCollectorAgent : ITypesCollectorAgent
    {
        public abstract void CollectType(Type type);

        public virtual void Init() {}
        public virtual void Dump(StringBuilder sb) {}
    }
}