using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using Uins.Settings;
using UnityEngine;

namespace Uins
{
    public class BindingControllersPool<T>
    {
        protected readonly BindingController<T> prefab;
        protected readonly Transform rootTransform;
        protected int controllersCount;

        private List<BindingController<T>> _controllers = new List<BindingController<T>>();
        private DisposableComposite _bindD = new DisposableComposite();
        private readonly bool _doFirstOrLastValueSet;


        //=== Ctor ============================================================

        public BindingControllersPool(Transform rootTransform, BindingController<T> prefab)
        {
            this.prefab = prefab;
            this.rootTransform = rootTransform;
            if (prefab == null || rootTransform == null)
            {
                UI.Logger.Error(
                    $"{GetType().NiceName()} Something is null: {nameof(this.prefab)}={this.prefab}, {nameof(this.rootTransform)}={this.rootTransform}");
                return;
            }

            _doFirstOrLastValueSet = prefab is IHasFirstAndLastRp;
        }


        //=== Public ==========================================================

        public void Connect(IListStream<T> listStream)
        {
            if (listStream == null)
                return;

            listStream.InsertStream.Subscribe(_bindD, OnInsert, Disconnect);
            listStream.RemoveStream.Subscribe(_bindD, OnRemove, Disconnect);
            listStream.ChangeStream.Subscribe(_bindD, OnChange, Disconnect);
        }

        public void Disconnect()
        {
            _bindD.Clear();
            while (_controllers.Count > 0)
                ReleaseLast();
        }

        public IEnumerable<BindingController<T>> GetEnumerable => _controllers;

        public override string ToString()
        {
            return $"[{GetType().NiceName()}: {nameof(prefab)}={prefab}, {nameof(rootTransform)}={rootTransform?.FullName()}]";
        }


        //=== Protected =======================================================

        /// <summary>
        /// Instantiate new controller
        /// </summary>
        protected virtual BindingController<T> GetController()
        {
            var controller = Object.Instantiate(prefab, rootTransform);
            controller.name = $"{prefab.name}_{++controllersCount}";
            return controller;
        }

        protected virtual void ReleaseController(BindingController<T> controller)
        {
            Object.Destroy(controller.gameObject);
        }


        //=== Private =========================================================

        private void OnChange(ChangeEvent<T> changeEvent)
        {
            var index = changeEvent.Index;
            if (index < 0 || index >= _controllers.Count)
            {
                UI.Logger.IfError()?.Message($"Index out of range: {nameof(index)}={index} count={_controllers?.Count} -- {this}").Write();
                return;
            }

            _controllers[index].SetVmodel(changeEvent.NewItem);
        }

        private void OnRemove(RemoveEvent<T> removeEvent)
        {
            Release(removeEvent.Index);
            SetFirstAndLastIfNeed();
        }

        private void OnInsert(InsertEvent<T> insertEvent)
        {
            InsertContr(insertEvent.Index, insertEvent.Item);
            SetFirstAndLastIfNeed();
        }

        private void InsertContr(int index, T vmodel)
        {
            var contr = GetController();
            if (contr == null)
            {
                UI.Logger.IfError()?.Message($"Unable to instantiate prefab -- {this}").Write();
                return;
            }

            _controllers.Insert(index, contr);
            contr.transform.SetSiblingIndex(index);

            contr.SetVmodel(vmodel);
        }

        private void ReleaseLast()
        {
            if (_controllers.Count > 0)
                Release(_controllers.Count - 1);
        }

        private void Release(int index)
        {
            if (index < 0 || index >= _controllers.Count)
            {
                UI.Logger.IfError()?.Message($"Index out of range: {nameof(index)}={index} count={_controllers?.Count} -- {this}").Write();
                return;
            }

            var contr = _controllers[index];
            _controllers.RemoveAt(index);
            ReleaseController(contr);
        }

        private void SetFirstAndLastIfNeed()
        {
            if (!_doFirstOrLastValueSet)
                return;

            for (int i = 0, count = _controllers.Count; i < count; i++)
            {
                var hasFirstAndLastRp = _controllers[i] as IHasFirstAndLastRp;
                if (hasFirstAndLastRp == null)
                    continue;

                hasFirstAndLastRp.IsFirstRp.Value = i == 0;
                hasFirstAndLastRp.IsLastRp.Value = i + 1 == count;
            }
        }
    }
}