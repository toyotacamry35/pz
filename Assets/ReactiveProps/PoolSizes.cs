using System;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;

namespace ReactiveProps
{
    public static partial class PoolSizes
    {
        public const string GeneratedFile = @"Assets/ReactiveProps/PoolSizes.Generated.cs";

        public static CapacityInfo GetPoolMaxCapacityForType<T>(int @default)
        {
            return GetPoolMaxCapacityForType(typeof(T), @default);
        }

        public static CapacityInfo GetPoolMaxCapacityForType(Type type, int @default)
        {
            try
            {
                if (_Sizes.TryGetValue(type.FullNiceName(), out var size))
                    return new CapacityInfo(size, size / 10);
                Pool.AddPoolWithDefaultCapacity(type);
            }
            catch (Exception e)
            {
                Pool.Logger.IfWarn()?.Exception(e).Write();
            }
            return new CapacityInfo(@default, 0);
        }

        public readonly struct CapacityInfo
        {
            public readonly int MaxCapacity;
            public readonly int InitialCapacity;

            public CapacityInfo(int maxCapacity, int initialCapacity)
            {
                MaxCapacity = maxCapacity;
                InitialCapacity = initialCapacity;
            }
        }
    }
}