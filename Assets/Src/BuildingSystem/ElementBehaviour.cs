using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public abstract class ElementBehaviour<ElementDataType> : MonoBehaviour
        where ElementDataType : ElementData
    {
        private ElementDataType elementData = null;

        protected bool IsClient { get; private set; } = false;
        protected bool IsServer { get; private set; } = false;

        public void SetData(ElementDataType data)
        {
            elementData = data;
            if (elementData != null)
            {
                elementData.BindPropertyChanged += Data_BindPropertyChanged;
                elementData.PlaceholderChanged += Data_PlaceholderChanged;
                elementData.ValidChanged += Data_ValidChanged;
                elementData.SelectedChanged += Data_SelectedChanged;
                elementData.VisibleChanged += Data_VisibleChanged;
                elementData.UnbindFinished += Data_UnbindFinished;
            }
        }

        public ElementDataType GetData()
        {
            return elementData;
        }

        // Utity methods --------------------------------------------------------------------------
        protected void Awake()
        {
            AwakeElement();
        }

        protected void OnDestroy()
        {
            DestroyHits();
            if (elementData != null)
            {
                DestroyElement(elementData);
            }
            elementData = null;
        }

        protected void Update()
        {
            UpdateElement(false);
            CheckHits();
        }

        // Hit management -------------------------------------------------------------------------
        internal class Hit
        {
            public DelayTimer Timer { get; set; } = new DelayTimer();
            public GameObject GameObject { get; set; } = null;
        }
        private List<Hit> hits = new List<Hit>();

        private void AddHit()
        {
            if (elementData != null)
            {
                var prefab = elementData.BuildRecipeDef.Visual.HitPrefab;
                if (prefab != null)
                {
                    var newHit = new Hit();
                    newHit.Timer.Set(elementData.BuildRecipeDef.Visual.HitTime);
                    newHit.GameObject = Instantiate(prefab.Target, gameObject.transform);
                    hits.Add(newHit);
                }
            }
        }

        private void CheckHits()
        {
            for (var index = 0; index < hits.Count; ++index)
            {
                if (!hits[index].Timer.IsInProgress())
                {
                    Destroy(hits[index].GameObject);
                    hits[index] = null;
                }
            }
            hits.RemoveAll(hit => hit == null);
        }

        private void DestroyHits()
        {
            for (var index = 0; index < hits.Count; ++index)
            {
                Destroy(hits[index].GameObject);
                hits[index] = null;
            }
            hits.Clear();
        }

        // public Unity thread methods ------------------------------------------------------------
        public void GotClient()
        {
            IsClient = true;
            if (elementData != null)
            {
                CreateVisual(elementData);
            }
        }

        public void GotServer()
        {
            IsServer = true;
            if (elementData != null)
            {
                CreateServer(elementData);
            }
        }

        public void LostClient()
        {
            IsClient = false;
            if (elementData != null)
            {
                DestroyVisual(elementData);
            }
        }

        public void LostServer()
        {
            IsServer = false;
            if (elementData != null)
            {
                DestroyServer(elementData);
            }
        }

        public bool DestroyGameObject()
        {
            bool result = false;
            if ((elementData != null) && !elementData.Placeholder) // check for placeholder right now
            {
                result = DestroyGameObject(elementData);
            }
            return result;
        }

        public void UpdateElement(bool force)
        {
            if (elementData != null)
            {
                UpdateElement(elementData, force, IsServer, IsClient);
            }
        }

        // Unity thread methods -------------------------------------------------------------------
        private void Data_UnbindFinished(object sender, EventArgs e)
        {
            if (elementData != null)
            {
                elementData.BindPropertyChanged -= Data_BindPropertyChanged;
                elementData.PlaceholderChanged -= Data_PlaceholderChanged;
                elementData.ValidChanged -= Data_ValidChanged;
                elementData.SelectedChanged -= Data_SelectedChanged;
                elementData.VisibleChanged -= Data_VisibleChanged;
                elementData.UnbindFinished -= Data_UnbindFinished;
            }
        }

        private void Data_BindPropertyChanged(object sender, PropertyData.PropertyArgs propertyArgs)
        {
            if (propertyArgs.PropertyName.Equals("Health"))
            {
                AddHit();
            }
            if ((elementData != null) && (elementData == sender))
            {
                BindPropertyChanged(elementData, propertyArgs);
            }
        }

        private void Data_PlaceholderChanged(object sender, PropertyData.PropertyArgs propertyArgs)
        {
            if ((elementData != null) && (elementData == sender))
            {
                PlaceholderChanged(elementData, propertyArgs);
            }
        }

        private void Data_ValidChanged(object sender, PropertyData.PropertyArgs propertyArgs)
        {
            if ((elementData != null) && (elementData == sender))
            {
                ValidChanged(elementData, propertyArgs);
            }
        }

        private void Data_SelectedChanged(object sender, PropertyData.PropertyArgs propertyArgs)
        {
            if ((elementData != null) && (elementData == sender))
            {
                SelectedChanged(elementData, propertyArgs);
            }
        }

        private void Data_VisibleChanged(object sender, PropertyData.PropertyArgs propertyArgs)
        {
            if ((elementData != null) && (elementData == sender))
            {
                VisibleChanged(elementData, propertyArgs);
            }
        }
        // abstract methods -----------------------------------------------------------------------
        protected abstract void AwakeElement();

        protected abstract void DestroyElement(ElementDataType data);

        // return false if you want to destroy game object right now by building system
        protected abstract bool DestroyGameObject(ElementDataType data);

        protected abstract void UpdateElement(ElementDataType data, bool force, bool isServer, bool isClient);

        protected abstract void CreateServer(ElementDataType data);

        protected abstract void DestroyServer(ElementDataType data);

        protected abstract void CreateVisual(ElementDataType data);

        protected abstract void DestroyVisual(ElementDataType data);

        protected abstract void BindPropertyChanged(ElementDataType data, PropertyData.PropertyArgs propertyArgs);

        protected abstract void PlaceholderChanged(ElementDataType data, PropertyData.PropertyArgs propertyArgs);

        protected abstract void ValidChanged(ElementDataType data, PropertyData.PropertyArgs propertyArgs);

        protected abstract void SelectedChanged(ElementDataType data, PropertyData.PropertyArgs propertyArgs);

        protected abstract void VisibleChanged(ElementDataType data, PropertyData.PropertyArgs propertyArgs);
    }
}