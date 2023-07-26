using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Assets.Src.GameObjectAssembler
{
    internal class DelegateCreator
    {
        static public Func<TType, TReturn> CreateGetPropertyDelegate<TType, TReturn>(PropertyInfo peropertyInfo)
        {
            var getMethod = peropertyInfo.GetGetMethod();

            var delType = typeof(Func<TType, TReturn>);
            var del = (Func<TType, TReturn>)Delegate.CreateDelegate(delType, getMethod);

            return del;
        }

        // I2CPP Incompatible
        static public Action<TType, TValue> CreateSetFieldDelegate<TType, TValue>(FieldInfo fieldInfo)
        {
            var targetExp = Expression.Parameter(typeof(TType), "target");
            var valueExp = Expression.Parameter(typeof(TValue), "value");

            var fieldExp = Expression.Field(targetExp, fieldInfo);
            try
            { 
                var assignExp = Expression.Assign(fieldExp, Expression.Convert(valueExp, fieldExp.Type));
                return Expression.Lambda<Action<TType, TValue>>(assignExp, targetExp, valueExp).Compile();
            } 
            catch(Exception)
            {
                throw new InvalidOperationException($"Cannot create Setter for field ({fieldInfo.FieldType}){fieldInfo.DeclaringType.FullName}.{fieldInfo.Name}, call instance type {typeof(TType).FullName}, call value type {typeof(TValue).FullName}");
            }
        }

        static public Action<TType, TValue> CreateSetPropertyDelegate<TType, TValue>(PropertyInfo peropertyInfo)
        {
            var setMethod = peropertyInfo.GetSetMethod();

            var del = (Action<TType, TValue>)Delegate.CreateDelegate(typeof(Action<TType, TValue>), setMethod);

            return del;
        }

        static private Action<TTypeOuter, TValueOuter> CreateSetPropertyDelegateExact<TTypeOuter, TTypeInner, TValueOuter, TValueInner>(PropertyInfo peropertyInfo) where TValueInner : TValueOuter where TTypeInner : TTypeOuter
        {
            var setMethod = peropertyInfo.GetSetMethod();

            var del = (Action<TTypeInner, TValueInner>)Delegate.CreateDelegate(typeof(Action<TTypeInner, TValueInner>), setMethod);

            Action<TTypeOuter, TValueOuter> del2 = (TTypeOuter obj, TValueOuter valIn) => del((TTypeInner)obj, (TValueInner)valIn);

            return del2;
        }

        static public Action<TType, TValue> CreateSetPropertyDelegateInexact<TType, TValue>(PropertyInfo peropertyInfo)
        {
            var setMethod = peropertyInfo.GetSetMethod();
            if (setMethod == null)
                return null;
            var type = setMethod.GetParameters()[0].ParameterType;

            var generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateSetPropertyDelegateExact), BindingFlags.Static | BindingFlags.NonPublic);
            var strict = generic.MakeGenericMethod(typeof(TType), peropertyInfo.DeclaringType, typeof(TValue), peropertyInfo.PropertyType);

            var result = (Action<TType, TValue>)strict.Invoke(null, new object[] { peropertyInfo });

            return result;
        }
    }
}
