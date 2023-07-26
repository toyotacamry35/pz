using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ReactivePropsNs;

namespace Uins
{
    /// <summary>
    /// Контроллер [Binding] свойства типа T. Содержит значение Value этого свойства.
    /// Cлужит для поддержки двусторонней связи свойства через TwoWayPropertyBinding с внешним стримом stream следующим образом:
    /// 1) Создаем экземпляр контроллера, связывая с Binding-свойством, на которое смотрит TwoWayPropertyBinding
    /// 2) Делаем Connect() связывая контроллер прямой (_outCallback) и обратной (stream) связью с внешним источником
    /// 3) При получении значения из TwoWayPropertyBinding свойство через сеттер вызывает OnChange() т.е. _outCallback, который в самом простом случае
    /// устанавливает значение у источника стрима. При этом Value не трогается во избежание паразитной связи
    /// 4) Связь от стрима: при получении измененного значения выставляем Value и сигнализируем об изменении для Weld через NotifyPropertyChanged()
    ///
    /// Итого: The_true источник значения - это наружный `stream`(см.арг.`Connect`а). Контроллер занимается тем, что передаёт от Weld'а наружу (outCallback) команду
    ///   на изменение знач-я и сам его не меняет. Устанавливает своё знач-е только от стрима.
    /// 
    /// </summary>
    /// <typeparam name="T">Произвольный класс/структура данных</typeparam>
    /// <typeparam name="M">Класс, в экземпляре которого находится связуемое свойство, которое хотим синхронизировать </typeparam>
    //Типовое создание контроллера в классе SomeClass:
    //  _twoWayPropController = new TwoWayPropController<bool, SomeClass>(() => SomeProp, this);
    // Типовое содержимое свойства:
    //  [Binding]
    //  public bool SomeProp
    //  {
    //      get => _twoWayPropController?.Value ?? false;
    //      set => _twoWayPropController?.OnChange(value);
    //  }
    public class TwoWayPropController<T, M> where M : class, INotifyPropertyChangedExt
    {
        private Action<T> _outCallback;
        private EqualityComparer<T> _equalityComparer = EqualityComparer<T>.Default;
        private string _propertyName;
        private PropertyInfo _propertyInfo;
        private M _propertyHolder;
        private T _onCompleteValue;

        /// <summary>
        /// 1) Если планируется жизнь с одним коннектом вначале и желаем автодисконнект от стрима, то в Connect() даем внешний общий источник,
        /// который где-то вовне застрелится.
        /// 2) Если же хотим делать множественные коннекты/дисконнекты, то внешний источник disposables не передаем, он создастся
        /// внутри данного экземпляра и по Disconnect() будет чистить только наши связи, созданные в Connect()
        /// </summary>
        private ICollection<IDisposable> _d;

        public T Value;

        public TwoWayPropController(Expression<Func<T>> propertyGetterExpr, M propertyHolder, T onCompleteValue = default)
        {
            if (propertyHolder.AssertIfNull(nameof(propertyHolder)) ||
                propertyGetterExpr.AssertIfNull(nameof(propertyGetterExpr)))
                return;

            _onCompleteValue = onCompleteValue;
            _propertyHolder = propertyHolder;
            _propertyName = BindingExtensions.GetVariableName(propertyGetterExpr);
            _propertyInfo = typeof(M).GetProperty(_propertyName);
            if (_propertyInfo == null)
                UI.Logger.Error($"{nameof(TwoWayPropController<T, M>)}<{typeof(T).NiceName()}, {typeof(M).NiceName()}> '{_propertyName}' " +
                                $"{nameof(_propertyInfo)} is null");
        }

        public void Connect(IStream<T> stream, Action<T> outCallback, ICollection<IDisposable> disposables = null)
        {
            if (stream.AssertIfNull(nameof(stream)) ||
                outCallback.AssertIfNull(nameof(outCallback)))
                return;

            _outCallback = outCallback;
            if (_propertyInfo == null)
                return;

            _d = disposables ?? new DisposableComposite();
            stream.Subscribe(_d, t =>
                {
                    //#Note: Решено не ограничивать значение тут. Т.к. значение (`Value`) тут - это лишь отражение значения (а не само значение). Мы лишь тусклый отблеск сияющей звезды the_true value в источнике.
                    Value = t;
                    // it's for Weld's binding work:
                    _propertyHolder.NotifyPropertyChanged(_propertyName);
                },
                _equalityComparer.Equals(_onCompleteValue, default)
                    ? default(Action)
                    : () => _propertyInfo.SetValue(_propertyHolder, _onCompleteValue));
        }

        public void Disconnect()
        {
            _d.Clear();
        }

        public virtual void OnChange(T val)
        {
            _outCallback?.Invoke(val);
        }
    }
}