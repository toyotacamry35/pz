using System;
using System.Collections.Generic;
using System.Text;
using Assets.Src.Aspects.Impl.Stats;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.Modifiers
{
    public static partial class SpellModifierExtensions
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public static float ApplyAttackStatModifiers(this IReadOnlyList<AttackModifierDef> modifiers, StatResource statDef, float statValue)
        {
            if (statDef == null) throw new ArgumentNullException(nameof(statDef));
            if (modifiers == null)
                return statValue;
            bool debugEnabled = Logger.IsDebugEnabled;
            var multiplierString = debugEnabled ? StringBuildersPool.Get : null;
            var summandString = debugEnabled ? StringBuildersPool.Get : null;
            string mulSign = string.Empty;
            string addSign = string.Empty;
            float multiplier = 1, summand = 0;
            foreach (var modifier in modifiers)
                if (modifier is AttackStatModifierDef statModifier)
                    if (statModifier.Stat == statDef)
                    {
                        multiplier *= statModifier.Multiplier;
                        summand += statModifier.Summand;
                        if (debugEnabled)
                        {
                            multiplierString.Append(mulSign).Append(statModifier.____GetDebugRootName()).Append('=').Append(statModifier.Multiplier);
                            summandString.Append(addSign).Append(statModifier.____GetDebugRootName()).Append('=').Append(statModifier.Summand);
                            mulSign = " * ";
                            addSign = " + ";
                        }
                    }
            var result = statValue * multiplier + summand;
            if (debugEnabled) Logger.IfDebug()?.Message($"ApplyAttackStatModifiers | {statDef.____GetDebugRootName()} = {statValue} x multiplier:{multiplier} + summand:{summand} = {result} | multiplier:({multiplierString}) summand:({summandString})").Write();
            return result;
        }
    }
}