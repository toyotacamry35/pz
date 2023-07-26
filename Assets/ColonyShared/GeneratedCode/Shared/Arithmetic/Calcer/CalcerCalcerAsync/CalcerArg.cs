using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using ColonyShared.SharedCode;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerArg<ReturnType> : ICalcerBinding<CalcerArgDef<ReturnType>, ReturnType> 
    {
        public ValueTask<ReturnType> Calc(CalcerArgDef<ReturnType> def, CalcerContext ctx)
        {
            var idx = ctx.Args != null ? Array.FindIndex(ctx.Args, x => x.Def != null && x.Def == def.ArgDef.Target || x.Name != null && x.Name == def.Arg) : -1;
            if (idx == -1)
                throw new Exception($"Missing argument '{def.Arg ?? def.ArgDef.Target?.ToString()}' while invocation the calcer {def.____GetDebugAddress()}");
            return new ValueTask<ReturnType>(ValueConverter<ReturnType>.Convert(ctx.Args[idx].Value));
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerArgDef<ReturnType> _) => Enumerable.Empty<StatResource>();
    }

    [UsedImplicitly]
    public class CalcerArgCollector : ICalcerBindingsCollector
    {
        public IEnumerable<Type> Collect()
        {
            var genericType = typeof(CalcerArg<>);
            return Value.SupportedTypes.Where(x => x.Item1 != Value.Type.None).Select(x => genericType.MakeGenericType(x.Item2));
        }
    }
}
