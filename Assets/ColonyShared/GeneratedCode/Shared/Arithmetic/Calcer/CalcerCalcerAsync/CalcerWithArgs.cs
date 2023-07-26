using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using NLog;
using ColonyShared.SharedCode;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerWithArgs<ReturnType> : ICalcerBinding<CalcerWithArgsDef<ReturnType>, ReturnType>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask<ReturnType> Calc(CalcerWithArgsDef<ReturnType> def, CalcerContext ctx)
        {
            var argsCount = ctx.Args?.Length ?? 0;
            var args = new CalcerContext.Arg[argsCount + def.Args.Count];
            if (argsCount > 0)
                Array.Copy(ctx.Args, args, argsCount);
            foreach (var arg in def.Args)
            {
                if (Logger.IsWarnEnabled && Array.Exists(args, x => x.Name == arg.Key))
                    Logger.IfWarn()?.Message($"Overriding argument {arg.Key} in calcer {def.____GetDebugAddress()}").Write();
                args[argsCount++] = new CalcerContext.Arg(arg.Key, await arg.Value.Target.CalcAsync(ctx));
            }
            return await def.Calcer.Target.CalcAsync(ctx.SetArgs(args));
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerWithArgsDef<ReturnType> def)
            => def.Calcer.Target.CollectStatNotifiers().Concat(def.Args.SelectMany(x => x.Value.Target.CollectStatNotifiers()));
    }
    
    [UsedImplicitly]
    public class CalcerWithArgsCollector : ICalcerBindingsCollector
    {
        public IEnumerable<Type> Collect()
        {
            var genericType = typeof(CalcerWithArgs<>);
            return Value.SupportedTypes.Where(x => x.Item1 != Value.Type.None).Select(x => genericType.MakeGenericType(x.Item2));
        }
    }
}
