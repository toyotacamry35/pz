using UnityEngine;

namespace Assets.Src.Character.Events
{
    public class DestroyFXPlayer : MonoBehaviour
    {
        public GameObject _onDestroyFx = default(GameObject);
        public Vector3 _shift = default(Vector3);
        public float _destroyDelay = 10;

        private void OnDestroy()
        {
            if (_onDestroyFx != null)
            {
                var gO = Instantiate(_onDestroyFx, transform.position + _shift, _onDestroyFx.transform.rotation * transform.rotation);
                if (gO != null)
                {
                    gO.SetActive(true);
                    Destroy(gO, _destroyDelay);
                }
            }
        }
    }
}
