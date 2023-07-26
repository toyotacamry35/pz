using System;
using Assets.Src.Aspects.Impl;
using Plugins.DebugDrawingExtension;
using Src.Tools;
using UnityEngine;

namespace Assets.Src.Target
{
    public class ConeInfo : MonoBehaviour
    {
        private static readonly DebugDraw Drawer = DebugDraw.Manager.GetDrawer("ConeInfo");
        
        public Type type;
        public Transform Cone => _cone;
        public Vector3 BigCentrePosition => _cone ? _cone.TransformPoint(_positionBig) : transform.position;

        public Vector3 SmallCentrePosition => _cone ? _cone.TransformPoint(_positionSmall) : transform.position;


        public float angleCamera = 30f;
        [SerializeField] int _countPoints = 6;
        [SerializeField] Vector3 _positionBig = Vector3.zero;
        [SerializeField] float _height = 6;
        [SerializeField] float _angle = 0;
        [SerializeField] float _bigRadiusHorizontal = 5;
        [SerializeField] float _bigRadiusVertical = 5;
        [SerializeField] float _smallRadiusHorizontal = 2;
        [SerializeField] float _smallRadiusVertical = 2;

        private Vector3 _positionSmall;
        private Transform _cone;
        
        public enum Type {
            general
        };
        
        public int Overlap(Collider[] result, float delay)
        {
            var bgn = BigCentrePosition; //transform.TransformPoint(_positionBig);
            var end = SmallCentrePosition; //transform.TransformPoint(_positionBig + new Vector3(0, -_height * Mathf.Tan(Mathf.Deg2Rad*_angle), _height));
            Drawer.Debug?.Capsule(bgn, end, _bigRadiusHorizontal, Color.green, delay);
            return Physics.OverlapCapsuleNonAlloc(bgn, end, _bigRadiusHorizontal, result, Int32.MaxValue, QueryTriggerInteraction.Collide);
        }
        
        
        private void Awake()
        {
            Create();
        }

        public void Create()
        {
            _positionSmall = _positionBig + new Vector3(0, -_height * Mathf.Tan(Mathf.Deg2Rad*_angle), _height);
            GameObject go = new GameObject(type.ToString());
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            _cone = go.transform;
        }
    }
}
