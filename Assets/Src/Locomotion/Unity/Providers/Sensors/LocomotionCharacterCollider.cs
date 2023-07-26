using System;
using System.Collections.Generic;
using UnityEngine;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.Unity.LocomotionColliderUtils;

namespace Src.Locomotion.Unity
{
    public class LocomotionCharacterCollider : MonoBehaviour, ILocomotionCollider, ILocomotionDebugable, ILocomotionCollisionsReceiver
    {
        private CharacterController _collider;
        private LayerMask _groundLayerMask;
        private LayerMask _actorLayerMask;
        private readonly List<ContactPoint> _contacts = new List<ContactPoint>(8);
        private int _gatherContactsFrame;

        public float Radius => _collider.radius;

        public int GroundLayerMask => _groundLayerMask;

        public LocomotionVector OriginPoint => _collider.transform.TransformPoint(OriginPointOffset).ToLocomotion();

        public float OriginOffset => _collider.center.y - Mathf.Max(_collider.height * 0.5f, _collider.radius) - _collider.skinWidth;

        public List<ContactPoint> Contacts => _contacts;
        
        public void Init(CharacterController collider, LayerMask groundLayer, LayerMask actorLayer)
        {
            _collider = collider ?? throw new ArgumentNullException(nameof(collider));
            _groundLayerMask = groundLayer;
            _actorLayerMask = actorLayer;
        }

        private Vector3 OriginPointOffset => new Vector3(_collider.center.x, OriginOffset, _collider.center.z);

        public void GatherDebug(ILocomotionDebugAgent agent)
        {
            if(agent!=null)
            {
                for (int i=0; i < _contacts.Count && i < DebugTags.Contacts.Length; ++i)
                    agent.Set(DebugTags.Contacts[i], _contacts[i].Point);
                agent.Set(ColliderPosition, OriginPoint);
            }            
        }

        public bool IsSame(object collider)
        {
            return collider is Collider c && (_collider == c || _collider.transform.root == c.transform.root);
        }

        private void LateUpdate()
        {
            if (Time.frameCount > _gatherContactsFrame)
                _contacts.Clear();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            OnCollision(
                hit.point.ToLocomotion(),
                hit.normal.ToLocomotion(),
                GetObjectTypeByLayer(hit.gameObject.layer, _groundLayerMask, _actorLayerMask),
                hit.collider.transform.position.ToLocomotion());
        }

        public void OnCollision(CollisionInfo nfo)
        {
            OnCollision(
                nfo.Point, 
                nfo.Normal, 
                GetObjectTypeByLayer(nfo.ObjectLayer, _groundLayerMask, _actorLayerMask), 
                nfo.ObjectPosition);
        }
        
        private void OnCollision(LocomotionVector point, LocomotionVector normal, ContactPointObjectType objectType, LocomotionVector objectPosition)
        {
            var localPoint = (_collider.transform.InverseTransformPoint(point.ToWorld().ToUnity()) - OriginPointOffset).ToLocomotion();

            ContactPointLocation location;
            if (localPoint.Vertical <= _collider.radius)
                location = ContactPointLocation.Bottom;
            else
                location = ContactPointLocation.Side;
            
            if (_gatherContactsFrame != Time.frameCount)
            {
                _gatherContactsFrame = Time.frameCount;
                _contacts.Clear();
            }
            
            _contacts.Add(new ContactPoint(point, localPoint, normal, location, objectType, objectPosition));
        }
    }
}