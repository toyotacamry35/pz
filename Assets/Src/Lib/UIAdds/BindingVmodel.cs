using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    public interface IHasDisposableComposite
    {
        DisposableComposite DisposableComposite { get; }
    }


    public class BindingVmodel : INotifyPropertyChangedExt, IIsDisposed, IHasDisposableComposite
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected DisposableComposite D => ((IHasDisposableComposite)this).DisposableComposite;
        DisposableComposite IHasDisposableComposite.DisposableComposite { get; } = new DisposableComposite();


        //=== Props ===============================================================

        public bool IsDisposed { get; private set; }


        //=== Public ==============================================================

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //=== Protected ===========================================================

        protected void Bind<T>(IStream<T> stream, Expression<Func<T>> propertyGetterExpr, T onCompleteValue = default)
        {
            stream.Bind(D, this, propertyGetterExpr, onCompleteValue);
        }

        /// <summary>
        /// Вариант временной подписки, когда хотим время от времени чистить disposables (и Bind-связи, соответственно) 
        /// </summary>
        protected void Bind<T>(DisposableComposite disposables, IStream<T> stream, Expression<Func<T>> propertyGetterExpr, T onCompleteValue = default)
        {
            D.Add(disposables);
            stream.Bind(disposables, this, propertyGetterExpr, onCompleteValue);
        }

        protected void Bind<T>(IListStream<T> listStream, ObservableList<T> observableList, DisposableComposite d = null)
        {
            if (d == null)
                d = D;
            else
                d.Add(D);
            listStream.ChangeStream.Action(d, changeEvent => observableList[changeEvent.Index] = changeEvent.NewItem);
            listStream.InsertStream.Action(d, insertEvent => observableList.Insert(insertEvent.Index, insertEvent.Item));
            listStream.RemoveStream.Action(d, removeEvent => observableList.RemoveAt(removeEvent.Index));
        }

        public virtual void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            D.Clear();
        }
    }
}