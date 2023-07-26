using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ReactivePropsNs;
using SharedCode.Utils;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    public class BindingViewModel : MonoBehaviour, INotifyPropertyChangedExt
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected DisposableComposite D = new DisposableComposite();

        

        //=== Protected ===========================================================

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Bind с собственным D, версия с Expression
        /// </summary>
        protected void Bind<T>(IStream<T> stream, Expression<Func<T>> propertyGetterExpr, T onCompleteValue = default)
        {
            stream.Bind(D, this, propertyGetterExpr, onCompleteValue);
        }

        /// <summary>
        /// Bind с собственным D, версия с PropertyBinder
        /// </summary>
        protected void Bind<T, TOwner>(IStream<T> stream, PropertyBinder<TOwner, T> binder, T onCompleteValue = default) where TOwner : BindingViewModel
        {
            stream.Bind(D, (TOwner) this, binder, onCompleteValue);
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

        protected virtual void OnDestroy()
        {
            D.Dispose();
        }

        public static string TransfprmPath(MonoBehaviour monoBeh) {
            var sb = StringBuildersPool.Get;
            Transform target = monoBeh.transform;
            sb.Append(target.gameObject.name);
            target = target.parent;
            for (int i = 0; target != null && i < 10; i++, target = target.parent)
                if (target == null) {
                    return sb.ToStringAndReturn();
                } else {
                    sb.Append("<<").Append(target.gameObject.name);
                }
            return sb.Append("<<...").ToStringAndReturn();
        }
    }
}