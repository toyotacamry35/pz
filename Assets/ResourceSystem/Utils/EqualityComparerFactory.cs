using System;
using System.Collections.Generic;
using System.Linq;
using EnumerableExtensions;

namespace ColonyShared.SharedCode.Utils
{
    public static class EqualityComparerFactory
    {
        public static IEqualityComparer<T> Create<T>()
        {
            return EqualityComparer<T>.Default;
        }
        
        public static IEqualityComparer<T> Create<T>(Func<T,T,bool> compare)
        {
            return new CustomComparer<T>(compare);
        }

        public static IEqualityComparer<T> Create<T>(Func<T,T,bool> compare, Func<T,int> hash)
        {
            return new CustomComparer<T>(compare, hash);
        }

        public static IEqualityComparer<IEnumerable<T>> CreateForEnumerable<T>()
        {
            return new CustomComparer<IEnumerable<T>>((x,y) => x.NullableSequenceEqual(y), x => x.Sum(CustomComparer<T>.DefaultGetHashCode));
        }

        public static IEqualityComparer<IEnumerable<T>> CreateForEnumerable<T>(Func<T,T,bool> compare)
        {
            return new CustomComparer<IEnumerable<T>>((x,y) => x.NullableSequenceEqual(y, Create(compare)), x => x.Sum(CustomComparer<T>.DefaultGetHashCode));
        }

        public static IEqualityComparer<IEnumerable<T>> CreateForEnumerable<T>(Func<T,T,bool> compare, Func<T,int> hash)
        {
            return new CustomComparer<IEnumerable<T>>((x,y) => x.NullableSequenceEqual(y, Create(compare, hash)), x => x.Sum(hash));
        }
        
        public static IEqualityComparer<T> FalseComparer<T>()
        {
            return CustomComparer<T>.FalseComparer;
        }

        
        private class CustomComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _compare;
            private readonly Func<T, int> _hash;

            public CustomComparer(Func<T, T, bool> compare, Func<T, int> hash = null)
            {
                _compare = compare;
                _hash = hash ?? DefaultGetHashCode;
            }

            public bool Equals(T x, T y) => _compare(x, y);

            public int GetHashCode(T x) => _hash(x);

            public static readonly Func<T, int> DefaultGetHashCode = EqualityComparer<T>.Default.GetHashCode;
            
            public static readonly CustomComparer<T> FalseComparer = new CustomComparer<T>((x,y) => false);
        }
    }
}