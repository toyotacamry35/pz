#if UNITY_BUILD || DEVELOPMENT_BUILD
    #define PROPERTIES_CHECK
#endif
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ReactivePropsNs;

namespace Uins
{
    public static class BindingExtensions
    {
        public static string GetVariableName<T>(Expression<Func<T>> expr)
        {
            var memberExpression = (MemberExpression) expr.Body;
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Связывание стрима stream T-данных со свойством объекта propertyOwner через property-expression
        /// Вариант не требующий доп. объектов обработчиков, но вызывающий reflection на каждое изменение значения.
        /// </summary>
        /// <typeparam name="T">Тип обрабатываемых данных</typeparam>
        /// <typeparam name="TOwner">Тип объекта, содержащего свойство</typeparam>
        /// <param name="stream">Стрим данных</param>
        /// <param name="disposables">Коллекция разрушаемых связей</param>
        /// <param name="propertyOwner">Объект, содержащий свойство</param>
        /// <param name="propertyGetterExpr">property-expression вида "() => PropertyName"</param>
        /// <param name="onCompleteValue">Значение, устанавливаемое на OnCompleted() стрима</param>
        public static void Bind<T, TOwner>(
            this IStream<T> stream,
            ICollection<IDisposable> disposables,
            TOwner propertyOwner,
            Expression<Func<T>> propertyGetterExpr,
            T onCompleteValue = default) where TOwner
            : class, INotifyPropertyChangedExt
        {
            if (propertyOwner.AssertIfNull(nameof(propertyOwner)) ||
                stream.AssertIfNull(nameof(stream)) ||
                propertyGetterExpr.AssertIfNull(nameof(propertyGetterExpr)))
                return;

            var propertyName = GetVariableName(propertyGetterExpr);
            var propertyInfo = propertyOwner.GetType().GetProperty(propertyName);
            // var comparer = EqualityComparer<T>.Default;
            if (propertyInfo != null)
            {
                disposables.Add(
                    stream.Subscribe(
                        disposables,
                        t =>
                        {
                            propertyInfo.SetValue(propertyOwner, t);
                            propertyOwner.NotifyPropertyChanged(propertyName);
                        },
                        () =>
                        {
                            propertyInfo.SetValue(propertyOwner, onCompleteValue);
                            propertyOwner.NotifyPropertyChanged(propertyName);
                        }));
            }
            else
            {
                UI.Logger.Error(
                    $"Bind<{typeof(T).NiceName()}, {typeof(TOwner).NiceName()}>() " +
                    $"Can't get {nameof(propertyInfo)} from {nameof(propertyName)} '{propertyName}'");
            }
        }

        /// <summary>
        /// Связывание стрима T-данных stream со свойством класса TOwner через propertyBinder, имеющий сеттер и имя свойства
        /// Более оптимальный по производительности вариант: в нем не вызывается reflection на каждое изменение значения.
        /// Но для использования требуется предварительно создать propertyBinder
        /// </summary>
        /// <typeparam name="T">Тип обрабатываемых данных</typeparam>
        /// <typeparam name="TOwner">Тип объекта, содержащего свойство</typeparam>
        /// <param name="stream">Стрим данных</param>
        /// <param name="disposables">Коллекция разрушаемых связей</param>
        /// <param name="propertyOwner">Объект, содержащий свойство</param>
        /// <param name="propertyBinder">Информация с именем свойства и его сеттером</param>
        /// <param name="onCompleteValue">Значение, устанавливаемое на OnCompleted() стрима</param>
        public static void Bind<T, TOwner>(
            this IStream<T> stream,
            ICollection<IDisposable> disposables,
            TOwner propertyOwner,
            PropertyBinder<TOwner, T> propertyBinder,
            T onCompleteValue = default)
            where TOwner : class, INotifyPropertyChangedExt
        {
            if (propertyOwner.AssertIfNull(nameof(propertyOwner)) ||
                stream.AssertIfNull(nameof(stream)) ||
                propertyBinder.Setter.AssertIfNull(nameof(propertyBinder.Setter)) ||
                propertyBinder.Name.AssertIfNull(nameof(propertyBinder.Name)))
                return;

            disposables.Add(
                stream.Subscribe(
                    disposables,
                    t =>
                    {
                        propertyBinder.Setter(propertyOwner, t);
                        propertyOwner.NotifyPropertyChanged(propertyBinder.Name);
                    },
                    () =>
                    {
                        propertyBinder.Setter(propertyOwner, onCompleteValue);
                        propertyOwner.NotifyPropertyChanged(propertyBinder.Name);
                    }));
        }
    }
}