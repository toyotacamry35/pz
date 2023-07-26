using System;
using System.Linq.Expressions;

namespace Uins
{
    public readonly struct PropertyBinder<TOwner, TProp> where TOwner : INotifyPropertyChangedExt
    {
        public readonly string Name;
        public readonly Action<TOwner, TProp> Setter;

        public PropertyBinder(string propertyName)
        {
            Name = propertyName;

            var propertyInfo = typeof(TOwner).GetProperty(propertyName);
            if (propertyInfo != null)
            {
                var setterInfo = propertyInfo.SetMethod ?? throw new Exception($"{typeof(TOwner).Name}.{propertyName} has no setter");
                var setter = (Action<TOwner, TProp>) Delegate.CreateDelegate(typeof(Action<TOwner, TProp>), setterInfo);
                Setter = setter;
            }
            else
                throw new Exception(
                    $"Bind<{typeof(TProp).NiceName()}, {typeof(TOwner).NiceName()}>() Can't get {nameof(propertyInfo)} from {nameof(Name)} '{Name}'");
        }
    }

    public static class PropertyBinder<TOwner> where TOwner : class, INotifyPropertyChangedExt
    {
        public static PropertyBinder<TOwner, TProp> Create<TProp>(Expression<Func<TOwner, TProp>> getter) =>
            new PropertyBinder<TOwner, TProp>(((MemberExpression) getter.Body).Member.Name);
    }
}