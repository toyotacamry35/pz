using System.Collections.Generic;
using ColonyShared.SharedCode.Entities.Reactions;
using Core.Environment.Logging.Extension;
using NLog;
using ResourceSystem.Reactions;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Reactions
{
    public static  class MapReactionArgsToSpellParams
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public static IEnumerable<SpellCastParameter> Map(IEnumerable<(ISpellParameterDef, IVar)> @params, ArgTuple[] args)
        {
            if (@params != null)
                foreach (var tuple in @params)
                {
                    if (tuple.Item2.TryGetValue(args, out var value))
                        yield return SpellCastParameters.Create(tuple.Item1, value);
                    else
                        Logger.IfError()?.Message($"Can't map reaction argument {tuple.Item2} to spell parameter {tuple.Item1}").Write();
                }
        }
    }
}