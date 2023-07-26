using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;

namespace SharedCode.Wizardry
{
    public static class SpellDefExtension
    {
        [NotNull] internal static readonly NLog.Logger Logger = LogManager.GetLogger("SpellDefExtension");
        
        private static readonly ConcurrentDictionary<SpellDef, SpellDefMetadata> SpellsMetadata = new ConcurrentDictionary<SpellDef, SpellDefMetadata>();

        public static bool RequireParameterOfType<T>(this SpellDef spell) where T : SpellContextValueDef //SpellEntityDef
        {
            return RequireParameterOfType(spell, typeof(T));
        }

        public static bool RequireParameterOfType(this SpellDef spell, Type type)
        {
            // параметры указаны в спелле явным образом
            if (spell.Params != null)
            {
                foreach (var param in spell.Params)
                    if (param.GetType() == type)
                    {
                        if (Logger.IsDebugEnabled)
                            Logger.IfDebug()?.Message($"RequireComponentOfType<{type.Name}>({spell.____GetDebugAddress()}) is True by SpellDef.Params").Write();
                        return true;
                    }
                return false;
            }
            
            
            // параметры НЕ указаны в спелле явным образом, пытаемся найти их в словах спелла через рефлекшн
            SpellDefMetadata metadata = SpellsMetadata.GetOrAdd(spell, k => new SpellDefMetadata());
            lock (metadata)
            {
                if (metadata.ParametersTypes == null)
                {
                    if (Logger.IsDebugEnabled) Logger.IfTrace()?.Message($"Spell: {spell.____GetDebugAddress()}").Write();
                    var refType = typeof(ResourceRef<>);
                    var paramType = typeof(SpellContextValueDef);
                    metadata.ParametersTypes = spell.SubSpells
                        .Select(x => x.Spell.Target)
                        .Append(spell)
                        .SelectMany(x => x.Words)
                        .Where(x => x != null)
                        .Select(x => new { Word = x, Properties = x.GetType().GetProperties() })
                        .SelectMany(x => x.Properties.Select(p => new { x.Word, Property = p }))
                        .Where(x => x.Property.PropertyType.IsGenericType && x.Property.PropertyType.GetGenericTypeDefinition() == refType && paramType.IsAssignableFrom(x.Property.PropertyType.GetGenericArguments()[0]))
                        .Select(x => ((IRefBase)x.Property.GetValue(x.Word)).TargetBase?.GetType())
                        .Where(x => x != null)
                        .Distinct()
                        .Select(x =>
                        {
                            if (Logger.IsDebugEnabled) Logger.IfTrace()?.Message($"{spell.____GetDebugAddress()} requires {x}").Write();
                            return x;
                        })
                        .ToList();
                }
                
                if (metadata.ParametersTypes.Any(t => t.IsAssignableFrom(type)))
                {
                    if (Logger.IsDebugEnabled)
                        Logger.IfDebug()?.Message ($"#Dbg: [2]  RequireComponentOfTypeDo(..)   >>> GOT <<<   (T==`{type.Name}`, answ==`true`) spell: {spell}")
                            .Write();
                    return true;
                }

                if (Logger.IsDebugEnabled)
                    Logger.IfDebug()?.Message ($"#Dbg: [2]  RequireComponentOfTypeDo(..)   >>> GOT <<<   (T==`{type.Name}`, answ==`false`) spell: {spell}")
                        .Write();
                
                return false;
            }
        }
    }

    public class SpellDefMetadata
    {
        //public bool IsTargetRequired;
        public List<Type> ParametersTypes;
    }

    public struct RequiredComponentOfType : IEquatable<RequiredComponentOfType>
    {
        public Type Type;
        public bool IsRequired;

        public RequiredComponentOfType(Type type, bool isRequired)
        {
            Type = type;
            IsRequired = isRequired;
        }

        public bool IsValid => Type != null;

        public bool Equals(RequiredComponentOfType other)
        {
            return other.Type == Type && IsRequired == other.IsRequired;
        }
    }
}
