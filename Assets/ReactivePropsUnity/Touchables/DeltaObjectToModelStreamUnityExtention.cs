using NLog;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static ReactivePropsNs.Touchables.DeltaObjectPropertyToStreamExtention;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectToModelStreamUnityExtention {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Порождает на основе DeltaObject.Child его синхронное представление в тредпуле, и перекладывает его в Stream Выводя его в UnityThread
        /// </summary>
        public static IStream<TPresentation> ToModelStream<TDelta, TModel, TPresentation>(
            this ITouchable<TDelta> source,
            ICollection<IDisposable> disposables,
            Expression<Func<TDelta, TModel>> getValueExpression,
            Func<TModel, Func<ITouchable<TModel>>, TPresentation> presentationFactry,
            string detaliedLogPrefix = null) where TDelta : class, IDeltaObject where TModel : class, IDeltaObject where TPresentation : IDisposable
        {
            DisposableComposite innerDisposables = new DisposableComposite();
            disposables.Add(innerDisposables);
            Func<string, string> _createLog = null;
            var output = new UnityThreadStream<TPresentation>(prefix => _createLog(prefix));
            innerDisposables.Add(output);
            ITouchable<TModel> TouchableFactory() => source.Child(innerDisposables, getValueExpression);

            DeltaObjectPropertyToStream<TDelta, TModel> dopts = null;
            dopts = new DeltaObjectPropertyToStream<TDelta, TModel>(
                new ActionBasedListener<TModel>(child => output.OnNext(presentationFactry(child, TouchableFactory)),
                () => innerDisposables.Dispose()
                ), null, getValueExpression, detaliedLogPrefix);
            _createLog = prefix => $"{prefix}ToModelStream<{typeof(TModel).NiceName()}, {typeof(TPresentation).NiceName()}>({presentationFactry}) \n{dopts.Log(prefix + '\t')}";
            innerDisposables.Add(dopts);
            innerDisposables.Add(source.Subscribe(dopts));

            // Старые модели в output вежливо будет диспоузить, аотому что они на кластер подписываются. Пока добавляю тут кусок монструозного кода. Работать будет, да и хрен с ним покаместь.
            async void lateDispose(TPresentation toDispose) {
                await Task.Delay(TimeSpan.FromSeconds(5));
                toDispose.Dispose();
                // PZ-15197 //Logger.IfInfo()?.Message($"$$$$$$$$$$ lateDispose({toDispose})").Write();
            }
            TPresentation previouseValue = default;
            output.Action(innerDisposables, value => { // Всё это происходит сугубо внутри UnityThread поэтому локи не нужны.
                if (previouseValue != null)
                    lateDispose(previouseValue);
                previouseValue = value;
            });

            return output;
        }
    }
}
