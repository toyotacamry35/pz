using System.Collections;
using Assets.Src.Effects.FX;
using Assets.Src.Shared;
using UnityEngine;

namespace Assets.Src.FX.Decals
{
    public class BloodDecalSpawner : MonoBehaviour
    {
        public GameObject[] BloodDecals;

        // ## User defined constants (SI units) ##
        private const float _velocity = 0.5f;

        private const float _acceleration = 9.8f;

        // Transversal raycast parameter
        private const float _raycastShift = -0.5f;

        // Longtitudal raycast parameter
        private const float _longtitudalRaycastLength = 2.5f;

        private const float _angleInDeg = 30f;
        // ##

        private static Quaternion _defaultRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);

        // # Precomputed constants
        private static readonly float _tgA = Mathf.Tan(Mathf.Deg2Rad * _angleInDeg);

        private static readonly float _trRaycastLen = Mathf.Sqrt(3 * _raycastShift);
        
        // #
        private GameObjectPoolSettings _settings;


        private void Start()
        {
            //DebugExtension.DebugArrow(gameObject.transform.position, gameObject.transform.forward, Color.blue, 10f, false);
            //DebugExtension.DebugArrow(gameObject.transform.position, gameObject.transform.up, Color.red, 10f, false);
            //DebugExtension.DebugArrow(gameObject.transform.position, gameObject.transform.right, Color.green, 10f, false);
            var v = gameObject.transform.forward * _velocity;
            var v_xz = Vector3.ProjectOnPlane(v, Vector3.up);

            var dirTransversal = v_xz.SetY(_raycastShift);
            RaycastHit[] rHit = new RaycastHit[1];
            var rHitTransversal = Physics.RaycastNonAlloc(gameObject.transform.position, dirTransversal, rHit, _trRaycastLen, PhysicsLayers.TerrainMask);
            if (rHitTransversal > 0)
            {
                CalculateParametersAndPlaceDecal(v, rHit[0]);
            }
            else
            {
                var dirLongtitudal = (v_xz.normalized * _tgA).SetY(-1f);
                var rHitLongtitudal = Physics.RaycastNonAlloc(gameObject.transform.position, dirLongtitudal, rHit, _longtitudalRaycastLength, PhysicsLayers.TerrainMask);
                if (rHitLongtitudal > 0)
                {
                    CalculateParametersAndPlaceDecal(v, rHit[0]);
                }
            }

            _settings = GameObjectPoolSettingsLevels.NonMandatory_0_1_200;
        }

        private void CalculateParametersAndPlaceDecal(Vector3 velocity, RaycastHit hitPoint)
        {
            var verticalDelta = (gameObject.transform.position - hitPoint.point).y;
            var D = Mathf.Pow(velocity.y, 2) +
                    2 * _acceleration * verticalDelta; // velocyty.y is equivalent to Vector3.Project(v, Vector3.up).magnitude since Y axis and gravity force vector are collinear
            if (D < 0)
                return;
            var t = (-_velocity + Mathf.Sqrt(D)) / _acceleration;
            var forwardRotation = Quaternion.LookRotation(hitPoint.normal, velocity);
            var rotation = forwardRotation * _defaultRotation;
            var position = hitPoint.point;
            this.StartInstrumentedCoroutine(PlaceDecalWithDelay(position, rotation, null, t));
        }

        private IEnumerator PlaceDecalWithDelay(Vector3 position, Quaternion rotation, Transform parent, float time)
        {
            yield return new WaitForSeconds(time);
            var bloodDecal = BloodDecals[Mathf.RoundToInt(Random.value * (BloodDecals.Length - 1))];
            if (bloodDecal == default(GameObject))
                Debug.LogWarning($"No decal in BloodDecalSpawner on {gameObject.name} gameobject");
            else
            {
                //GameObjectPool.Instance.Get(bloodDecal);
                GameObjectPool.Instance.DelayedSpawn<FXPoolElement>(_settings, bloodDecal, position, rotation);
                //Instantiate(bloodDecal, position, rotation, null);
                /*if (decal != default(GameObject))
                {
                    decal.transform.position = position;
                    decal.transform.rotation = rotation;
                }*/
            }
        }
    }
}