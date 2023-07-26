using System;
using System.Linq;
using System.Linq.Expressions;

namespace ReactivePropsNs
{
    public static class ExpressionPropertyGetterExtention
    {
        public static string GetMemberName<TParent, TProperty>(this Expression<Func<TParent, TProperty>> propertyGetterExpr)
        {
            if (!(propertyGetterExpr.Body is MemberExpression memberExpression))
                throw new Exception("Wrong UseCase. Must be: p => p.Property");
            string propertyName = memberExpression.Member.Name;
            if (typeof(TParent).GetProperty(propertyName) == null &&
                typeof(TParent).GetInterfaces().All(implementedInterface => implementedInterface.GetProperty(propertyName) == null)
            ) // Пофиг на оверхед, потому что выполняется один раз при создании
                throw new Exception($"{typeof(TParent).Name} has no definition for {propertyName} of type {typeof(TParent)}");
            return propertyName;
        }
    }
}
