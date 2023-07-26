using Assets.Src.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
	public class FXSpawn : MonoBehaviour
	{
        public const float _raycastDistance = 50f;

        void Update()
		{
            if (_fx == null || _fx.Length == 0)
            {
                return;
            }

			_current += Time.deltaTime;
			if (_current > _delta)
			{
                _current = 0;
                _delta = Random.Range(_deltaTimeRandomMin, _deltaTimeRandomMax);

                RaycastHit[] hit = Physics.RaycastAll(gameObject.transform.position, Vector3.down, PhysicsChecker.CheckDistance(_raycastDistance, "FXSpawn"), Assets.Src.Shared.PhysicsLayers.CheckIsGroundedAndWaterMask);
                if (hit == null || hit.Length == 0)
                {
                    return;
                }

                float minDistance = hit[0].distance;
                Vector3 position = hit[0].point;
                for (int i = 1; i < hit.Length; i++)
                {
                    if (hit[i].distance < minDistance)
                    {
                        minDistance = hit[i].distance;
                        position = hit[i].point;
                    }
                }

                FXQueue.Instance.Get(_fx[Random.Range(0, _fx.Length)], position, Quaternion.identity); 
			}
		}

		[SerializeField] float _deltaTimeRandomMin;
		[SerializeField] float _deltaTimeRandomMax;
		[SerializeField] GameObject[] _fx;

		float _delta = 0;
		float _current = 0;
	}
}
