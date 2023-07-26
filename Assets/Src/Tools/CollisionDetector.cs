using Assets.Src.Inventory;
using GeneratedCode.DeltaObjects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.SpawnSystem;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Assets.Src.Tools
{
    public class CollisionDetector : MonoBehaviour
    {
        public OuterRef<IEntity> Owner;
        public BaseItemResource StaticStatsItem;
        public ItemSpecificStats ItemBaseStats; 
        public StatModifier[]  ItemStats;
        public IEntitiesRepository Repository;

        public event CollisionHandler CollisionEnter;
        public event CollisionHandler CollisionStay;
        public event CollisionHandler CollisionExit;

        public delegate void CollisionHandler(Collision collision, GameObject ownerObject, CollisionDetector collisionDetector);

        public void SetCollisionEnterTimeout(float timeout, CollisionHandler eventHendler = null)
        {
            this.StartInstrumentedCoroutine(EventCoroutine(eventHendler ?? CollisionEnter, timeout));
        }

        public void SetCollisionStayTimeout(float timeout, CollisionHandler eventHendler = null)
        {
            this.StartInstrumentedCoroutine(EventCoroutine(eventHendler ?? CollisionStay, timeout));
        }

        public void SetCollisionExitTimeout(float timeout, CollisionHandler eventHendler = null)
        {
            this.StartInstrumentedCoroutine(EventCoroutine(eventHendler ?? CollisionExit, timeout));
        }

        IEnumerator EventCoroutine(CollisionHandler eventhandler, float timeout)
        {
            yield return new WaitForSeconds(timeout);
            eventhandler?.Invoke(null, gameObject, this);
            eventhandler = null;
        }

        void OnCollisionEnter(Collision collision)
        {
            CollisionEnter?.Invoke(collision, gameObject, this);
        }

        void OnCollisionStay(Collision collision)
        {
            CollisionStay?.Invoke(collision, gameObject, this);
        }

        void OnCollisionExit(Collision collision)
        {
            CollisionExit?.Invoke(collision, gameObject, this);
        }
    }
}
