using System.Collections.Generic;
using UnityEngine;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.Unity.LocomotionColliderUtils;

namespace Src.Locomotion.Unity
{
    public class LocomotionCollider : MonoBehaviour, ILocomotionCollider, ILocomotionDebugable
    {
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private float _dynamicFriction = 0;
        [SerializeField] private float _staticFriction = 0;
        [SerializeField] private PhysicMaterialCombine _frictionCombine = PhysicMaterialCombine.Minimum;

        private readonly List<Contact> _contacts = new List<Contact>(8);
        private readonly List<ContactPoint> _points = new List<ContactPoint>(8);
        private PhysicMaterial _colliderMaterial;
        private int _groundLayerMask;
        private int _actorLayerMask;
        private bool _pointsInvalidated;

        public float Radius => _collider.radius;

        public int GroundLayerMask => _groundLayerMask;

        public LocomotionVector OriginPoint => LocomotionHelpers.WorldToLocomotionVector(_collider.transform.TransformPoint(BottomPointOffset));

        public float OriginOffset => _collider.center.y - Mathf.Max(_collider.height * 0.5f, _collider.radius);

        public List<ContactPoint> Contacts
        {
            get
            {
                if (_pointsInvalidated)
                {
                    _pointsInvalidated = false;
                    _points.Clear();
                    foreach (var cnt in _contacts)
                        _points.Add(cnt.ContactPoint);
                }
                return _points;
            }
        }

        public void Init(int groundLayer, int actorLayerMask)
        {
            _groundLayerMask = groundLayer;
            _actorLayerMask = actorLayerMask;
            _contacts.Clear();
            _points.Clear();
        }

        public bool IsSame(object collider)
        {
            return collider is Collider c && _collider.transform.root == c.transform.root;
        }

        void Awake()
        {
            _collider.material = _colliderMaterial = new PhysicMaterial
            {
                dynamicFriction = _dynamicFriction,
                staticFriction = _staticFriction,
                frictionCombine = _frictionCombine,
                bounciness = 0,
                bounceCombine = PhysicMaterialCombine.Minimum
            };
        }

        void OnDestroy()
        {
            if (_colliderMaterial)
                Destroy(_colliderMaterial);
        }

        private void Update()
        {
            for (int i = _contacts.Count - 1; i >= 0; --i)
            {
                var oc = _contacts[i].OtherCollider;
                if (!oc || !oc.gameObject || !oc.enabled || !oc.gameObject.activeInHierarchy)
                {
                    _contacts.RemoveAt(i);
                    _pointsInvalidated = true;
                }
            }
        }

        private Vector3 BottomPointOffset => new Vector3(_collider.center.x, OriginOffset, _collider.center.z);

        private void OnCollisionStay(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & GroundLayerMask) != 0)
            {
                if (_contacts.RemoveAll(x => x.OtherCollider == collision.collider) > 0)
                    _pointsInvalidated = true;
                var shift = _collider.attachedRigidbody.velocity *
                            Time.fixedDeltaTime; // позиции контактов вычисляются до перемещения body и для вычисления их локальных координат их надо смещать на шаг 
                for (var i = 0; i < collision.contacts.Length; ++i)
                {
                    var contact = collision.contacts[i];
                    var localPt = _collider.transform.InverseTransformPoint(contact.point + shift) - BottomPointOffset;

                    ContactPointLocation location;
                    if (localPt.y <= _collider.radius)
                        location = ContactPointLocation.Bottom;
                    else
                        location = ContactPointLocation.Side;
                    
                    var objectType = GetObjectTypeByLayer(contact.otherCollider.gameObject.layer, _groundLayerMask, _actorLayerMask);
                    var objectPosition = contact.otherCollider.transform.position.ToLocomotion();

                    _contacts.Add(new Contact
                    {
                        ContactPoint = new ContactPoint(
                            point: LocomotionHelpers.WorldToLocomotionVector(contact.point),
                            localPoint: LocomotionHelpers.WorldToLocomotionVector(localPt),
                            normal: LocomotionHelpers.WorldToLocomotionVector(contact.normal),
                            objectPosition: objectPosition,
                            location: location,
                            objectType: objectType
                        ),
                        OtherCollider = collision.collider
                    });
                    _pointsInvalidated = true;
                    break;
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (_contacts.RemoveAll(x => x.OtherCollider == other.collider) > 0)
                _pointsInvalidated = true;
        }

        private struct Contact
        {
            public ContactPoint ContactPoint;
            public Collider OtherCollider;
        }

        public void GatherDebug(ILocomotionDebugAgent agent)
        {
            if (agent != null)
            {
                for (int i=0; i < _contacts.Count && i < DebugTags.Contacts.Length; ++i)
                    agent.Set(DebugTags.Contacts[i], _contacts[i].ContactPoint.Point);
                agent.Set(ColliderPosition, OriginPoint);
            }
        }
    }
}