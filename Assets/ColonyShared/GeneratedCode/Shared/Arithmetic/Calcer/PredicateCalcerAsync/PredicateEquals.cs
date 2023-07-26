using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.GeneratedCode.Shared.Arithmetic.Calcer.PredicateCalcerAsync;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Utils;

namespace ColonyShared.GeneratedCode.Shared.Arithmetic.Calcer.PredicateCalcerAsync
{
    public class PredicateEquals<T> : ICalcerBinding<PredicateEqualsDef<T>, bool>
    {
        private static readonly IEqualityComparer<T> _Comparer = CreateComparer();
        
        public async ValueTask<bool> Calc(PredicateEqualsDef<T> def, CalcerContext ctx)
        {
            if (def.Lhs == null || def.Rhs == null)
                return false;
            return _Comparer.Equals(await def.Lhs.Target.CalcAsync(ctx), await def.Rhs.Target.CalcAsync(ctx));
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateEqualsDef<T> def)
        {
            if (def.Lhs != null)
                foreach (var res in def.Lhs.GetModifiers())
                    yield return res;
            if (def.Rhs != null)
                foreach (var res in def.Rhs.GetModifiers())
                    yield return res;
        }

        private static IEqualityComparer<T> CreateComparer()
        {
            if (typeof(T) == typeof(Guid))
                return (IEqualityComparer<T>)EqualityComparerFactory.Create<Guid>((x,y) => x != Guid.Empty && y != Guid.Empty && x.Equals(y), x => x.GetHashCode() ); // да, если оба Guid - empty, то они то же НЕ равны!
            if (typeof(T) == typeof(OuterRef))
                return (IEqualityComparer<T>)EqualityComparerFactory.Create<OuterRef>((x,y) => x.IsValid && y.IsValid && x.Equals(y), x => x.GetHashCode() ); // да, если оба OuterRef - not valid, то они то же НЕ равны!
            if (typeof(IResource).IsAssignableFrom(typeof(T)))
                return (IEqualityComparer<T>)EqualityComparerFactory.Create<IResource>((x,y) => x != null && y != null && ReferenceEquals(x, y), x => x.GetHashCode() ); // да, если оба ресурса - null, то они то же НЕ равны!
            return EqualityComparer<T>.Default;
        }
    }
    
    
    
    
    [UsedImplicitly]
    public class PredicateEqualsCollector : ICalcerBindingsCollector
    {
        public IEnumerable<Type> Collect()
        {
            var genericType = typeof(PredicateEquals<>);
            return Value.SupportedTypes.Where(x => x.Item1 != Value.Type.None).Select(x => genericType.MakeGenericType(x.Item2));
        }
    }
}