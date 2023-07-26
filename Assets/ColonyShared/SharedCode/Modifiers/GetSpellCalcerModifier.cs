using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects;
using ResourceSystem.Reactions;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.Modifiers
{
    public static partial class SpellModifierExtensions
    {
        public static float GetSpellCalcerModifier(this IEnumerable<SpellModifierDef> modifiers, ArgDef<float> variable, CalcerSpellModifierStackingPolicy stackingPolicy, float @default)
        {
            bool hasResult = false;
            float result = 0;
            bool debugEnabled = Logger.IsDebugEnabled;
            var @string = debugEnabled ? StringBuildersPool.Get : null;
            if (modifiers != null)
                foreach (var modifier in modifiers)
                    if (modifier is SpellCalcerModifierDef<float> calcerModifier && calcerModifier.Variable == variable)
                    {
                        float value = calcerModifier.Value;
                        switch (stackingPolicy)
                        {
                            case CalcerSpellModifierStackingPolicy.Multiply:
                                result = hasResult ? result * value : value;
                                if (debugEnabled) @string.Append(hasResult ? " * " : string.Empty).Append(modifier.____GetDebugRootName()).Append("=").Append(value);
                                break;
                            case CalcerSpellModifierStackingPolicy.Add:
                                result = hasResult ? result + value : value;
                                if (debugEnabled) @string.Append(hasResult ? " + " : string.Empty).Append(modifier.____GetDebugRootName()).Append("=").Append(value);
                                break;
                            case CalcerSpellModifierStackingPolicy.Min:
                                result = hasResult ? Math.Min(result, value) : value;
                                if (debugEnabled) @string.Append(hasResult ? ", " : "Min ").Append(modifier.____GetDebugRootName()).Append("=").Append(value);
                                break;
                            case CalcerSpellModifierStackingPolicy.Max:
                                result = hasResult ? Math.Max(result, value) : value;
                                if (debugEnabled) @string.Append(hasResult ? ", " : "Max ").Append(modifier.____GetDebugRootName()).Append("=").Append(value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(stackingPolicy), stackingPolicy, null);
                        }
                        hasResult = true;
                    }

            if (!hasResult)
                result = @default;
            
            if (debugEnabled) Logger.IfDebug()?.Message($"GetSpellCalcerModifier | {variable.____GetDebugRootName()} = {result} | modifiers:{@string}").Write();

            return result;
        }
    }
}