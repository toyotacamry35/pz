using UnityEngine;

namespace Src.Effects.Specific
{
    public class RaycastBulletShotFX : RaycastShotFX
    {
        public GameObject Bullet;
        public GameObject FxImpactPrefab;
        public float AfterImpactLifetTime; 
        public float Speed = 5;

        private float _time;
        private float _duration;

        private void Start()
        {
            var dist = (EndPoint - StartPoint).magnitude;
            _duration = dist / Speed;
        }
        
        private void Update()
        {
            var newtime = _time + Time.deltaTime;
            Bullet.transform.position = Vector3.Lerp(StartPoint, EndPoint, Mathf.Clamp01(_time / _duration));
            if (_time < _duration && newtime >= _duration)
            {
                Instantiate(FxImpactPrefab, EndPoint, Quaternion.LookRotation(EndPoint - StartPoint, transform.up), transform);
                Bullet.SetActive(false);
            }
            else if (newtime >= _duration + AfterImpactLifetTime)
            {
                Destroy(gameObject);
            }
            _time = newtime;
        }
    }
}
