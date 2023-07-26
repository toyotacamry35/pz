using UnityEngine;
using UnityUpdate;

namespace Assets.Src.BuildingSystem
{
    public class ElementGameObject
    {
        private GameObject gameObject = null;
        private bool enable = false;

        private Vector3 position = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private float time = 0.0f;

        private bool positionSet = false;
        private bool rotationSet = false;

        private Vector3 Move( Vector3 current, Vector3 desired, float lerpFactor)
        {
            if (positionSet || ((current - desired).sqrMagnitude <= SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticMinSqrDistance))
            {
                positionSet = true;
                return desired;
            }
            else
            {
                return Vector3.Lerp(current, desired, lerpFactor);
            }
        }

        private Quaternion Rotate(Quaternion current, Quaternion desired, float lerpFactor)
        {
            if (rotationSet || (Quaternion.Angle(current, desired) <= SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticMinAngle))
            {
                rotationSet = true;
                return desired;
            }
            else
            {
                return Quaternion.Lerp(current, desired, lerpFactor);
            }
        }

        private void OnUpdate()
        {
            if (gameObject != null)
            {
                if (enable)
                {
                    var currentTime = Time.time;
                    var deltaTime = currentTime - time;
                    if (!positionSet || !rotationSet)
                    {
                        var lerpFactor = Mathf.Clamp(SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticLerpFactor * deltaTime / SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticDeltaTime, SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticMinLerpFactor, SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticMaxLerpFactor);
                        gameObject.transform.SetPositionAndRotation(Move(gameObject.transform.transform.position, position, lerpFactor), Rotate(gameObject.transform.transform.rotation, rotation, lerpFactor));
                    }
                    time = currentTime;
                }
                else
                {
                    gameObject.transform.SetPositionAndRotation(position, rotation);
                    positionSet = true;
                    rotationSet = true;
                }
            }
        }

        public ElementGameObject(GameObject _gameObject, bool _enable)
        {
            gameObject = _gameObject;
            if (gameObject != null)
            {
                enable = _enable;
                position = gameObject.transform.position;
                rotation = gameObject.transform.rotation;
                positionSet = true;
                rotationSet = true;
                time = Time.time;
                if (enable)
                {
                    UnityUpdateDelegate.OnUpdate += OnUpdate;
                }
            }
        }

        public void Destroy()
        {
            if (gameObject != null)
            {
                if (enable)
                {
                    UnityUpdateDelegate.OnUpdate -= OnUpdate;
                }
                UnityEngine.Object.Destroy(gameObject);
                gameObject = null;
                enable = false;
            }
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }

        public T GetComponent<T>() where T : Component
        {
            if (gameObject != null)
            {
                return gameObject.GetComponent<T>();
            }
            return null;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Quaternion GetRotation()
        {
            return rotation;
        }

        public void SetPositionAndRotation(Vector3 _position, Quaternion _rotation)
        {
            if (gameObject != null)
            {
                position = _position;
                rotation = _rotation;
                positionSet = false;
                rotationSet = false;
                OnUpdate();
            }
        }
    }
}
