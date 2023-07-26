using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourceSystem.Aspects
{
    public static class SpellModifierExtension
    {
        public static IReadOnlyList<T> Combine<T>(this IReadOnlyList<T> source1, IReadOnlyList<SpellModifierDef> source2) where T : SpellModifierDef
        {
            if (source1 != null)
            {
                if (source2 != null)
                    return source1.Concat(source2.OfType<T>()).ToArray();
                return source1;
            }
            return source2?.OfType<T>().ToArray();
        }

        public static string ToStringExt<T>(this IEnumerable<T> modifiers) where T : SpellModifierDef
        {
            return new StringBuilder().AppendExt(modifiers).ToString(); 
        }
        
        public static StringBuilder AppendExt<T>(this StringBuilder sb, IEnumerable<T> modifiers) where T : SpellModifierDef
        {
            if (modifiers == null)
                return sb;
            sb.Append('[');
            string separator = String.Empty;
            foreach (var modifier in modifiers)
            {
                sb.Append(separator).Append(modifier.____GetDebugShortName());
                separator = ", ";
            }
            sb.Append(']');
            return sb;
        }
    }
}