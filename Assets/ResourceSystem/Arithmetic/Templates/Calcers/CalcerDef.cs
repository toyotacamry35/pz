using System;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public abstract class CalcerDef : BaseResource
    {
        public string __DebugName => (this as IResource).Address.Root != null ? ____GetDebugShortName() : GetType().Name;
    }

    [CanBeCreatedFromAliasedPrimitive(typeof(object), nameof(CreateFromPrimitive))]
    // ReSharper disable once UnusedTypeParameter
    public abstract class CalcerDef<ReturnType> : CalcerDef
    {
        [UsedImplicitly]
        public static CalcerDef<ReturnType> CreateFromPrimitive(object value)
        {
            var type = typeof(CalcerConstantDef<ReturnType>);
            var def = (CalcerConstantDef<ReturnType>)Activator.CreateInstance(type, Convert.ChangeType(value, typeof(ReturnType)));
            return def;
        }
    }
}
