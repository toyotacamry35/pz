using System;
using System.Collections.Generic;
using Assets.Src.Aspects.Impl.Stats;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects;

namespace ColonyShared.SharedCode.Modifiers
{
    public static partial class SpellModifierExtensions
    {
        public static float ApplyAttackStatOverride(this IReadOnlyList<AttackModifierDef> modifiers, StatResource statDef, float statValue)
        {
            if (statDef == null) throw new ArgumentNullException(nameof(statDef));
            if (modifiers == null)
                return statValue;
            foreach (var modifier in modifiers)
                if (modifier is AttackStatOverrideDef statModifier)
                    if (statModifier.Stat == statDef)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"ApplyAttackStatOverride | {statDef.____GetDebugRootName()} = {modifier.____GetDebugShortName()}={statModifier.Value}").Write();
                        return statModifier.Value;
                    }
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"ApplyAttackStatOverride | {statDef.____GetDebugRootName()} = {statValue}").Write();
            return statValue;
        }
    }
}