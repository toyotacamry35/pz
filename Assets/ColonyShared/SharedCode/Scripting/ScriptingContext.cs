using Assets.Src.Arithmetic;
using ColonyShared.SharedCode;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scripting
{
    /*
     * Идейно всё так: все спеллы запускаются закреплённые за каким-то объектом. Очевидно, что по какой-то причине. 
     * Эта причина отражается контекстом, включающим в себя input'ы в спелл и host-объект
     * Существуют переходы от одного контекста в другой. Они формулируются самой системой, которая принимает на вход один контекст, а кастит спеллы с другим
     * Предполагается, что в этом случае она сама отвечает за передачу или не передачу кастомных параметров
     * Кастомные параметры могут быть единичными, списком или селектором списка
     * this + параметры метода + кастомный пейлоад со спеллом
     * 
     */


    [ProtoContract]
    public class ScriptingContext
    {
        public OuterRef<IEntity> Host { get; set; }
        [ProtoMember(1)]
        [ProtoMap(DisableMap = true)]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, Value> TypedArgs { get; set; }
        [ProtoMember(2)]
        [ProtoMap(DisableMap = true)]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<ContextArgTypeDef, Value> CustomArgs { get; set; }
    }

    public static class ScriptingContextExtensions
    {
        public static ScriptingContext Clone(this ScriptingContext @this) => @this != null ? new ScriptingContext { Host = @this.Host, TypedArgs = @this.TypedArgs, CustomArgs = @this.CustomArgs } : null;
    }
    
    public static class ContextFieldDefMethod
    {
        public static void Set(this ScriptingContext ctx, Type argType, Value value)
        {
            if (ctx.TypedArgs == null)
                ctx.TypedArgs = new Dictionary<int, Value>();
            ctx.TypedArgs[DefToType.GetNetIdForType(argType)] = value;

        }

        public static void Set(this ScriptingContext ctx, ContextArgTypeDef argType, Value value)
        {
            if (ctx.CustomArgs == null)
                ctx.CustomArgs = new Dictionary<ContextArgTypeDef, Value>();
            ctx.CustomArgs[argType] = value;
        }

        public static void Set<T>(this ContextFieldDef<T> field, Value value, ScriptingContext ctx) where T : ContextArg
        {
            if (ctx.TypedArgs == null)
                ctx.TypedArgs = new Dictionary<int, Value>();
            ctx.TypedArgs[DefToType.GetNetIdForType(typeof(T))] = value;
        }
        public static Value Get(this ScriptingContext ctx, ContextArgTypeDef argType)
        {
            if (ctx.CustomArgs == null)
                return default;
            ctx.CustomArgs.TryGetValue(argType, out var val);
            return val;
        }
        public static Value Get(this ScriptingContext ctx, Type argType)
        {
            if (ctx.TypedArgs == null)
                return default;
            ctx.TypedArgs.TryGetValue(DefToType.GetNetIdForType(argType), out var val);
            return val;
        }

        public static async Task<ScriptingContext> CalcFromDef(this ScriptingContextDef def, OuterRef<IEntity> host, ScriptingContext from, IEntitiesRepository repo)
        {
            var ctx = new ScriptingContext();
            if (def == null)
                return ctx;
            var scriptingProps = def.GetType().GetProperties(
                ).Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(ContextFieldDef<>));
            foreach (var prop in scriptingProps)
            {
                var val = ((IContextFieldDef)prop.GetValue(def));
                if (val != null)
                    ctx.Set(prop.PropertyType.GetGenericArguments()[0], await val.Calcer.Target.CalcAsync(host, from, repo));
            }
            if (def.CustomArgs != null)
                foreach (var customArg in def.CustomArgs)
                {
                    ctx.Set(customArg.Key, await customArg.Value.Target.CalcAsync(host, from, repo));
                }
            return ctx;
        }



    }

}
