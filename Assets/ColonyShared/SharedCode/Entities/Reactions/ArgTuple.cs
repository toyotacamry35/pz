using System;
using System.Collections.Generic;
using Assets.Src.Arithmetic;
using ProtoBuf;
using ResourceSystem.Reactions;

namespace ColonyShared.SharedCode.Entities.Reactions
{
    [ProtoContract]
    public class ArgTuple
    {
        [ProtoMember(1)] public ArgDef Def;
        [ProtoMember(2)] public ArgValue Value;

        public static ArgTuple Create<T>(ArgDef<T> def, ArgValue<T> value)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new ArgTuple{ Def = def, Value = value };
        }

        public static ArgTuple CreateTypeless(ArgDef def, ArgValue value)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (def.ValueType != value.ValueType)
                throw new Exception($"Reaction argument type mismatch: argument is {def.GetType()} ({def.ValueType.Name}) value is {value.GetType()} ({value.ValueType.Name})"); 
            return new ArgTuple{ Def = def, Value = value };
        }

        public override string ToString() => $"Def:{Def} Value:{Value}";
    }

    public static class ArgTupleExtensions
    {
        public static T GetValue<T>(this IVar<T> src, IEnumerable<ArgTuple> args)
        {
            if (src.TryGetValue(args, out var rv))
                return rv;
            throw new Exception($"Reaction argument not found: {src}");
        }

        public static bool TryGetValue<T>(this IVar<T> src, IEnumerable<ArgTuple> args, out T value)
        {
            if (src is IValue<T> v)
            {
                value = v.Value;
                return true;
            }
            if (src is IArg<T> arg)
            {
                foreach (var a in args)
                {
                    if (a.Def == arg.Def)
                    {
                        var proxy = a.Value as IValue<T> ?? throw new Exception($"Reaction argument type mismatch: got {a.Value?.GetType().Name} but expected {typeof(IValue<T>).Name}");
                        value = proxy.Value;
                        return true;
                    }
                }
            }
            value = default(T);
            return false;
        }
        
        public static Value GetValue(this IVar src, IEnumerable<ArgTuple> args)
        {
            if (src.TryGetValue(args, out var rv))
                return rv;
            throw new Exception($"Reaction argument not found: {src}");
        }

        public static bool TryGetValue(this IVar src, IEnumerable<ArgTuple> args,  out Value value)
        {
            if (src is IValue v)
            {
                value = v.Value;
                return true;
            }
            if (src is IArg arg)
            {
                foreach (var a in args)
                {
                    if (a.Def == arg.Def)
                    {
                        value = a.Value.Value;
                        return true;
                    }
                }
            }
            value = new Value();
            return false;
        }        
        
        public static CalcerContext.Arg[] ToCalcerArgs(this IReadOnlyList<ArgTuple> args)
        {
            var rv = new CalcerContext.Arg[args.Count];
            for (int i=0, cnt = args.Count; i < cnt; ++i)
            {
                var arg = args[i];
                rv[i] = new CalcerContext.Arg(arg.Def, arg.Value.Value);
            }
            return rv;
        }
    }
}